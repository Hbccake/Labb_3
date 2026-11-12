using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{
    public enum DialogType
    {
        PackOptions,
        CreateNewPack
    }
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private readonly JSON _jSON = new();
    public ObservableCollection<QuestionPackViewModel> Packs { get; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
    public RelayCommand AddQuestionCommand { get; }
    public RelayCommand RemoveQuestionCommand { get; }
    public RelayCommand OpenPackOptionsDialogCommand { get; }
    public RelayCommand OpenCreateNewPackDialogCommand { get; }
    public RelayCommand CreateNewPackCommand { get; }
    public RelayCommand SelectQuestionPackCommand { get; }
    public RelayCommand DeleteQuestionPackCommand { get; }
    public AsyncRelayCommand SaveQuestionPackCommand { get; }
    public AsyncRelayCommand LoadQuestionPackCommand { get; }
    public Visibility HasSelectedQuestion => SelectedQuestion is not null ? Visibility.Visible : Visibility.Hidden;
    public bool CanRemoveQuestion => SelectedQuestion is not null && ActivePack.Questions.Contains(SelectedQuestion);
    public bool CanDeleteQuestionPack => ActivePack is not null;


    private QuestionPackViewModel? _activePack;
    public QuestionPackViewModel? ActivePack
    {
        get => _activePack;
        set
        {
            if (SetProperty(ref _activePack, value))
            {
                _mainWindowViewModel?.SetActivePack(value);
                RaisePropertyChanged(nameof(CanRemoveQuestion));
                RemoveQuestionCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Difficulty SelectedDifficulty
    {
        get => ActivePack.Difficulty;
        set
        {
            ActivePack.Difficulty = value;

            RaisePropertyChanged();
        }
    }
    public Difficulty[] Difficulties
    {
        get => Enum.GetValues<Difficulty>();
    }
    private QuestionPackViewModel _selectedPack { get; set; }
    public QuestionPackViewModel SelectedPack
    {
        get => _selectedPack;

        set
        {
            _selectedPack = value;
            RaisePropertyChanged();
        }
    }
    public event Action<DialogType> OpenDialog;
    private void RaiseOpenDialog(DialogType dialogtype)
    {
        OpenDialog?.Invoke(dialogtype);
    }

    private Question? _selectedQuestion;
    public Question? SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            if (SetProperty(ref _selectedQuestion, value))
            {
                RaisePropertyChanged(nameof(HasSelectedQuestion));
                RaisePropertyChanged(nameof(CanRemoveQuestion));
                RemoveQuestionCommand.RaiseCanExecuteChanged();
            }
        }
    }
    private QuestionPackViewModel? _newPack;
    public QuestionPackViewModel? NewPack
    {
        get => _newPack;
        set
        {
            _newPack = value;

        }
    }

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Packs = mainWindowViewModel?.Packs ?? new ObservableCollection<QuestionPackViewModel>();

        OpenPackOptionsDialogCommand = new RelayCommand(_ => RaiseOpenDialog(DialogType.PackOptions));
        OpenCreateNewPackDialogCommand = new RelayCommand(_ => RaiseOpenDialog(DialogType.CreateNewPack));
        AddQuestionCommand = new RelayCommand(_ => AddNewQuestion());
        RemoveQuestionCommand = new RelayCommand(_ => RemoveQuestion(), _ => CanRemoveQuestion);
        CreateNewPackCommand = new RelayCommand(_ => CreateNewPack());
        DeleteQuestionPackCommand = new RelayCommand(_ => DeletePack(), _ => CanDeleteQuestionPack);
        SelectQuestionPackCommand = new RelayCommand(SelectedQuestionPack);
        SaveQuestionPackCommand = new AsyncRelayCommand(_ => Save());
        LoadQuestionPackCommand = new AsyncRelayCommand(_ => LoadSavedPacks());

    }

    private static string FolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizPacks");

    private async Task LoadSavedPacks()
    {
        var dialog = new OpenFileDialog()
        {
            InitialDirectory = Directory.Exists(FolderPath) ? FolderPath : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Filter = "Question Packs |*.json*"
        };

        dialog.ShowDialog();
        QuestionPack? pack = await _jSON.LoadAsync(filePath: dialog.FileName);
        if (pack is null) return;
        var loadedPack = new QuestionPackViewModel(pack);

        Packs.Add(loadedPack);
        ActivePack = loadedPack;
    }
    private async Task Save()
    {
        if (ActivePack is null) return;

        Directory.CreateDirectory(FolderPath);
        string filePath = Path.Combine(FolderPath, $"{ActivePack.Name}.json");
        var pack = new QuestionPack(ActivePack.Name, ActivePack.Difficulty, ActivePack.TimeLimitInSeconds);
        await _jSON.SaveAsync(pack, filePath);
    }

    private void RemoveQuestion()
    {
        if (!CanRemoveQuestion) return;
        ActivePack?.Questions.Remove(SelectedQuestion);
        SelectedQuestion = null;
    }
    private void DeletePack()
    {
        if (ActivePack is null) return;
        var deletePack = ActivePack;

        var result = MessageBox.Show($"Are you sure you want to delete {deletePack.Name}?", "DeletePack Question Pack", MessageBoxButton.YesNo);

        if (result != MessageBoxResult.Yes) return;

        string filePath = Path.Combine(FolderPath, $"{deletePack.Name}.json");

        Packs.Remove(deletePack);
        if (File.Exists(filePath))
            File.Delete(filePath);

        ActivePack = Packs.FirstOrDefault();
        DeleteQuestionPackCommand.RaiseCanExecuteChanged();
    }
    private void AddNewQuestion()
    {
        if (ActivePack.Questions.Any(question => !question.HasValidAnswers))
        {
            MessageBox.Show("Complete the current question with one correct and three incorrect answers before continuing", "Incomplete Question", MessageBoxButton.OK);
            return;
        }

        ActivePack?.Questions.Add(new Question("", "", "", "", ""));

    }
    private void CreateNewPack()
    {
        if (NewPack is null) return;
        var newPack = new QuestionPackViewModel(new QuestionPack(NewPack.Name));
        ActivePack = newPack;

        Packs.Add(newPack);
        _ = Save();

    }

    private void SelectedQuestionPack(object? obj)
    {
        if (obj is QuestionPackViewModel selectedPack)
        {
            SelectedPack = selectedPack;
            ActivePack = SelectedPack;

        }
    }

}
