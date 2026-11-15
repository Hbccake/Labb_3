using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{
    public enum DialogType
    {
        PackOptions,
        CreateNewPack,
        ImportTrivia
    }
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private readonly JSON _jSON = new();
    private readonly Trivia _trivia = new();
    public ObservableCollection<QuestionPackViewModel> Packs { get; }
    public TriviaPackViewModel TriviaPack { get; } = new();
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));

    public RelayCommand AddQuestionCommand { get; }
    public RelayCommand RemoveQuestionCommand { get; }


    public RelayCommand OpenPackOptionsDialogCommand { get; }
    public RelayCommand OpenCreateNewPackDialogCommand { get; }
    public RelayCommand OpenImportTriviaDbDialogCommand { get; }

    public RelayCommand CreateNewPackCommand { get; }
    public RelayCommand SelectQuestionPackCommand { get; }
    public RelayCommand DeleteQuestionPackCommand { get; }
    public AsyncRelayCommand SaveQuestionPackCommand { get; }
    public AsyncRelayCommand LoadQuestionPackCommand { get; }

    public AsyncRelayCommand ImportTriviaCommand { get; }

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
        OpenImportTriviaDbDialogCommand = new RelayCommand(_ => RaiseOpenDialog(DialogType.ImportTrivia));

        AddQuestionCommand = new RelayCommand(_ => AddNewQuestion());
        RemoveQuestionCommand = new RelayCommand(_ => RemoveQuestion(), _ => CanRemoveQuestion);

        CreateNewPackCommand = new RelayCommand(_ => CreateNewPack());
        DeleteQuestionPackCommand = new RelayCommand(_ => DeletePack(), _ => CanDeleteQuestionPack);

        SelectQuestionPackCommand = new RelayCommand(SelectedQuestionPack);
        SaveQuestionPackCommand = new AsyncRelayCommand(_ => Save());
        LoadQuestionPackCommand = new AsyncRelayCommand(_ => LoadSavedPacks());
        ImportTriviaCommand = new AsyncRelayCommand(_ => ImportTriviaQuestions());
    }



    private static string FolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizPacks");

    public async Task LoadSavedPacks(bool showFileDialog = true)
    {
        try
        {
            if (showFileDialog)
            {
                var dialog = new OpenFileDialog()
                {
                    InitialDirectory = Directory.Exists(FolderPath) ? FolderPath : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Filter = "Question Packs |*.json*",
                    FileName = "QuestionPacks.json",
                    ValidateNames = true

                };

                if (dialog.ShowDialog() != DialogResult.OK) return;
            }
            var loadedPacks = await _jSON.LoadAsync();
            Packs.Clear();
            foreach (var pack in loadedPacks)
                Packs.Add(pack);

            ActivePack = Packs.FirstOrDefault();
        }
        catch (JsonException)
        {
            MessageBox.Show("The file you are trying to open is invalid", "Load failed", MessageBoxButton.OK, MessageBoxImage.Error);

        }

    }

    private async Task ImportTriviaQuestions()
    {
        if (ActivePack is null) return;
        try
        {
            var questions = await _trivia.FetchQuestionsAsync(TriviaPack.NumberOfQuestions, TriviaPack.Category, TriviaPack.TriviaDifficulty);

            TriviaPack.Questions.Clear();
            foreach (var question in questions)
            {
                TriviaPack.Questions.Add(question);
                ActivePack.Questions.Add(question);
            }
            ActivePack.Difficulty = TriviaPack.TriviaDifficulty;
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("Questions could not be imported. Trivia service unavailable", "Import Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (JsonException)
        {
            MessageBox.Show("The Trivia Service returned an invalid response", "Import Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task LoadCategories()
    {

        var categories = await _trivia.ImportCategoryAsync();
        TriviaPack.Categories.Clear();

        foreach (var category in categories)
            TriviaPack.Categories.Add(category);

        TriviaPack.SelectedCategory = TriviaPack.Categories
        .FirstOrDefault(category => category.Id == TriviaPack.Category)
        ?? TriviaPack.Categories.FirstOrDefault();

    }
    public async Task Save()
    {
        if (ActivePack is null) return;
        try
        {
            await _jSON.SaveAsync(Packs);
        }
        catch (JsonException)
        {
            MessageBox.Show("The Questionpacks could not be serialized", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DirectoryNotFoundException e)
        {
            MessageBox.Show(e.Message);

        }
        catch (FileNotFoundException e)
        {
            MessageBox.Show(e.Message);
        }

    }

    private void RemoveQuestion()
    {
        if (!CanRemoveQuestion) return;
        ActivePack?.Questions.Remove(SelectedQuestion);
        SelectedQuestion = null;
        _ = Save();
    }
    private void DeletePack()
    {
        if (ActivePack is null) return;
        var deletePack = ActivePack;

        var result = MessageBox.Show($"Are you sure you want to delete {deletePack.Name}?", "DeletePack Question Pack", MessageBoxButton.YesNo);

        if (result != MessageBoxResult.Yes) return;

        Packs.Remove(deletePack);

        ActivePack = Packs.FirstOrDefault();
        DeleteQuestionPackCommand.RaiseCanExecuteChanged();

        _ = Save();
    }
    private void AddNewQuestion()
    {

        if (ActivePack.Questions.Any(question => !question.HasValidAnswers))
        {
            MessageBox.Show("Complete the current question with one correct and three incorrect answers before continuing", "Incomplete Question", MessageBoxButton.OK);
            return;
        }

        ActivePack?.Questions.Add(new Question("", "", "", "", ""));
        _ = Save();
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

            ActivePack = selectedPack;

        }
    }

}
