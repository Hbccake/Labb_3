using Labb_3.Models;

namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{

    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));


    //public bool IsSelectedQuestion => SelectedQuestion != null;

    private Question _selectedQuestion;
    public Question SelectedQuestion
    {
        get => _selectedQuestion;

        set => SetProperty(ref _selectedQuestion, value);

    }
    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

    }
}
