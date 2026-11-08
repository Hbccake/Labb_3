using Labb_3.Command;
using Labb_3.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{
    public enum DialogType
    {
        PackOptions,
        CreateNewPack
    }
    private readonly MainWindowViewModel? _mainWindowViewModel;
    public ObservableCollection<QuestionPackViewModel> Packs { get; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
    public RelayCommand AddQuestionCommand { get; }
    public RelayCommand RemoveQuestionCommand { get; }
    public RelayCommand OpenPackOptionsDialogCommand { get; }
    public RelayCommand OpenCreateNewPackDialogCommand { get; }
    public RelayCommand CreateNewPackCommand { get; }
    public RelayCommand SelectQuestionPackCommand { get; }
    public RelayCommand DeleteQuestionPackCommand { get; }

    //public bool IsSelectedQuestion => SelectedQuestion != null;
    private QuestionPackViewModel? _activePack;
    public QuestionPackViewModel? ActivePack
    {
        get => _activePack;
        set
        {
            if (SetProperty(ref _activePack, value)) _mainWindowViewModel?.SetActivePack(value);
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
    public Array Difficulties
    {
        get => Enum.GetValues(typeof(Difficulty));
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

    private Question? _selectedQuestion;
    public Question? SelectedQuestion
    {
        get => _selectedQuestion;

        set
        {
            _selectedQuestion = value;
            RaisePropertyChanged();

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
        RemoveQuestionCommand = new RelayCommand(_ => RemoveQuestion());
        CreateNewPackCommand = new RelayCommand(_ => CreateNewPack());
        DeleteQuestionPackCommand = new RelayCommand(_ => DeletePack());
        SelectQuestionPackCommand = new RelayCommand(SelectedQuestionPack);

    }
    public event Action<DialogType> OpenDialog;
    private void RaiseOpenDialog(DialogType dialogtype)
    {
        OpenDialog?.Invoke(dialogtype);
    }

    private void AddNewQuestion()
    {
        ActivePack?.Questions.Add(new Question("", "", "", "", ""));
    }

    private void RemoveQuestion()
    {
        ActivePack?.Questions.Remove(SelectedQuestion);
    }
    private void DeletePack()
    {
        if (ActivePack is null) return;
        var deletePack = ActivePack;

        var result = MessageBox.Show($"Are you sure you want to delete {deletePack.Name}?", "DeletePack Question Pack", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            Packs.Remove(deletePack);
            ActivePack = Packs.FirstOrDefault();
        }

    }

    private void CreateNewPack()
    {
        if (NewPack is null) return;
        var newPack = new QuestionPackViewModel(new QuestionPack(NewPack.Name));
        ActivePack = newPack;

        Packs.Add(newPack);

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
