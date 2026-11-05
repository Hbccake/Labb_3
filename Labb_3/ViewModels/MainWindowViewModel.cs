using Labb_3.Command;
using Labb_3.Dialogs;
using Labb_3.Models;
using System.Collections.ObjectModel;
using System.Windows;




namespace Labb_3.ViewModels;

class MainWindowViewModel : ViewModelBase
{

    public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();
    public RelayCommand CreateNewPackCommand { get; }
    public RelayCommand SelectQuestionPackCommand { get; }
    public RelayCommand DeleteQuestionPackCommand { get; }
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

    private QuestionPackViewModel _activePack;
    public QuestionPackViewModel ActivePack
    {
        get => _activePack;
        set
        {
            if (SetProperty(ref _activePack, value))
                ConfigurationViewModel?.RaiseActivePackChanged();

        }
    }
    private QuestionPackViewModel _newPack;
    public QuestionPackViewModel NewPack
    {
        get => _newPack;
        set
        {
            _newPack = value;

        }

    }
    //public Array MessageBoxButtons
    //{
    //    get => Enum.GetValues(typeof(MessageBoxButton));
    //}

    private void DeletePack()
    {
        //var message = $"Are you sure you want to delete {ActivePack.Name}";
        //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
        //var result = MessageBox.Show($"Are you sure you want to delete {ActivePack.Name}", buttons);
        //if (result == MessageBoxResult.Yes)


        ActivePack = SelectedPack;
        Packs.Remove(SelectedPack);
        RaisePropertyChanged();

    }

    private void CreateNewPack()
    {

        var newPack = new QuestionPackViewModel(new QuestionPack(NewPack.Name));
        ActivePack = newPack;

        Packs.Add(newPack);

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

        CreateNewPackCommand = new RelayCommand(_ => CreateNewPack());
        DeleteQuestionPackCommand = new RelayCommand(_ => DeletePack());
        SelectQuestionPackCommand = new RelayCommand(SelectedQuestionPack);
        ConfigurationViewModel.OpenDialog += OpenDialog;

    }

    private void SelectedQuestionPack(object? obj)
    {
        if (obj is QuestionPackViewModel selectedPack)
        {
            SelectedPack = selectedPack;
            ActivePack = SelectedPack;

        }
    }

    private Question _activeQuestion;
    public Question ActiveQuestion
    {
        get => _activeQuestion;

        set => SetProperty(ref _activeQuestion, value);

    }


    private void OpenDialog(ConfigurationViewModel.DialogType dialogType)
    {
        if (dialogType == ConfigurationViewModel.DialogType.CreateNewPack)
        {
            NewPack = new QuestionPackViewModel(new QuestionPack("<PackName>"));
        }

        Window dialog = dialogType switch
        {
            ConfigurationViewModel.DialogType.PackOptions => new PackOptionsDialog(),
            ConfigurationViewModel.DialogType.CreateNewPack => new CreateNewPackDialog(),
            _ => throw new NotImplementedException(),
        };
        dialog.DataContext = this;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog();

    }


}
