using Labb_3.Command;
using Labb_3.Models;

namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{
    public enum DialogType
    {
        PackOptions,
        CreateNewPack
    }
    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));

    public RelayCommand AddQuestionCommand { get; }
    public RelayCommand RemoveQuestionCommand { get; }

    //public bool IsSelectedQuestion => SelectedQuestion != null;

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

    private Question _selectedQuestion;
    public Question SelectedQuestion
    {
        get => _selectedQuestion;

        set
        {
            _selectedQuestion = value;
            RaisePropertyChanged();

        }

    }

    public RelayCommand OpenPackOptionsDialogCommand { get; }
    public RelayCommand OpenCreateNewPackDialogCommand { get; }

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        OpenPackOptionsDialogCommand = new RelayCommand(_ => RaiseOpenDialog(DialogType.PackOptions));
        OpenCreateNewPackDialogCommand = new RelayCommand(_ => RaiseOpenDialog(DialogType.CreateNewPack));
        AddQuestionCommand = new RelayCommand(_ => AddNewQuestion());
        RemoveQuestionCommand = new RelayCommand(_ => RemoveQuestion());

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

}
