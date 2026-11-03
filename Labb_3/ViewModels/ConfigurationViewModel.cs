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
        get => _mainWindowViewModel.ActivePack.Difficulty;
        set
        {
            _mainWindowViewModel.ActivePack.Difficulty = value;
            RaisePropertyChanged();
        }
    }
    private Question _selectedQuestion;
    public Question SelectedQuestion
    {
        get => _selectedQuestion;

        set => SetProperty(ref _selectedQuestion, value);

    }

    public event Action<DialogType> OpenDialog;
    public RelayCommand OpenPackOptionsDialogCommand { get; }
    public RelayCommand OpenCreateNewPackDialogCommand { get; }

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        OpenPackOptionsDialogCommand = new RelayCommand(_ => RaiseOpenSettings(DialogType.PackOptions));
        OpenCreateNewPackDialogCommand = new RelayCommand(_ => RaiseOpenSettings(DialogType.CreateNewPack));
        AddQuestionCommand = new RelayCommand(_ => AddNewQuestion());
    }
    private void RaiseOpenSettings(DialogType dialogtype)
    {
        OpenDialog?.Invoke(dialogtype);
    }

    private void AddNewQuestion()
    {

        ActivePack.Questions.Add(new Question("", "", "", "", ""));

    }
}
