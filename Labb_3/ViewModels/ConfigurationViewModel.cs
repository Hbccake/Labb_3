namespace Labb_3.ViewModels;

class ConfigurationViewModel : ViewModelBase
{

    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

}
