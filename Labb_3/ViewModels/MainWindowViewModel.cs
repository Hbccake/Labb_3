using Labb_3.Command;
using Labb_3.Dialogs;
using Labb_3.Models;
using System.Collections.ObjectModel;
using System.Windows;




namespace Labb_3.ViewModels;

public enum ViewType
{
    ConfigurationView,
    PlayerView,
    EndView
}
class MainWindowViewModel : ViewModelBase
{

    public RelayCommand ChangeViewCommand { get; }
    public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();


    public void ChangeView(ViewType viewType)
    {

        IsConfigurationViewVisible = viewType == ViewType.ConfigurationView;
        IsActiveQuiz = viewType == ViewType.PlayerView;
        ShowEndView = viewType == ViewType.EndView;

    }


    private bool _showEndView;
    public bool ShowEndView
    {
        get => _showEndView;
        set => SetProperty(ref _showEndView, value);

    }

    private bool _isActiveQuiz;
    public bool IsActiveQuiz
    {
        get => _isActiveQuiz;
        set => SetProperty(ref _isActiveQuiz, value);

    }


    private bool _isConfigurationViewVisible;
    public bool IsConfigurationViewVisible
    {
        get => _isConfigurationViewVisible;
        set => SetProperty(ref _isConfigurationViewVisible, value);

    }



    private QuestionPackViewModel _activePack;
    public QuestionPackViewModel ActivePack
    {
        get => _activePack;
        set
        {
            if (SetProperty(ref _activePack, value))
                ConfigurationViewModel?.RaiseActivePackChanged();
            PlayerViewModel?.RaiseActivePackChanged();
        }
    }

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public MainWindowViewModel()
    {
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);
        //ActivePack = new QuestionPackViewModel(new QuestionPack("Default Question Pack"));
        var questionPack = new QuestionPack("Mina frågor!", Difficulty.Medium);
        questionPack.Questions.Add(new Question("Vad heter sveriges huvudstad?", "Stockholm", "Oslo", "Göteborg", "London"));
        questionPack.Questions.Add(new Question("Vad är 2 + 3?", "5", "10", "8", "1"));

        ActivePack = new QuestionPackViewModel(questionPack);
        Packs.Add(ActivePack);
        ConfigurationViewModel.ActivePack = ActivePack;
        ConfigurationViewModel.OpenDialog += OpenDialog;
        //_ = ConfigurationViewModel.LoadSavedPacks();
        ChangeViewCommand = new RelayCommand(view =>
        {
            if (view is ViewType viewType) ChangeView(viewType);
        });

        ChangeView(ViewType.ConfigurationView);
    }

    public void SetActivePack(QuestionPackViewModel? setPack)
    {
        if (setPack is not null) ActivePack = setPack;
    }

    private async void OpenDialog(ConfigurationViewModel.DialogType dialogType)
    {
        if (dialogType == ConfigurationViewModel.DialogType.CreateNewPack)
        {
            ConfigurationViewModel?.NewPack = new QuestionPackViewModel(new QuestionPack("<PackName>"));
        }
        if (dialogType == ConfigurationViewModel.DialogType.ImportTrivia)
            await ConfigurationViewModel.LoadCategories();

        Window dialog = dialogType switch
        {
            ConfigurationViewModel.DialogType.PackOptions => new PackOptionsDialog(),
            ConfigurationViewModel.DialogType.CreateNewPack => new CreateNewPackDialog(),
            ConfigurationViewModel.DialogType.ImportTrivia => new ImportTriviaDbDialog(),
            _ => throw new NotImplementedException(),
        };
        dialog.DataContext = this;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog();
        _ = ConfigurationViewModel.Save();
    }


}
