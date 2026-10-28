using Labb_3.Models;
using System.Collections.ObjectModel;

namespace Labb_3.ViewModels;

class MainWindowViewModel : ViewModelBase
{

    public ObservableCollection<QuestionPackViewModel> Packs { get; }

    private QuestionPackViewModel _activePack;
    public QuestionPackViewModel ActivePack
    {
        get => _activePack;
        set => SetProperty(ref _activePack, value); //ersätter nedan

        //if (_activePack == value) return;
        //_activePack = value;
        //RaisePropertyChanged();
    }
    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public MainWindowViewModel()
    {
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);
        ActivePack = new QuestionPackViewModel(new QuestionPack("Default Question Pack"));
    }

}
