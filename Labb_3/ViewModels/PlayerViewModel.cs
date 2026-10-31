using Labb_3.Models;
using System.Diagnostics;
using System.Windows.Threading;

namespace Labb_3.ViewModels;

class PlayerViewModel : ViewModelBase
{
    string _answer1;
    string _answer2;
    string _answer3;
    string _answer4;
    Question _currentQuestion;
    int _questionCount = 0;
    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;


        DispatcherTimer timer = new();
        _stopwatch.Start();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        timer.Start();
    }
    private int? _counter
    {
        get => _mainWindowViewModel?.ActivePack.TimeLimitInSeconds;
        set => _counter = value;
    }


    private DispatcherTimer timer;
    private Stopwatch _stopwatch = new();
    public void Timer_Tick(object? sender, EventArgs e)
    {
        if (RemainingTime > 1) RemainingTime--;
        else
        {
            _stopwatch.Stop();

        }
    }
    private int _remainingTime;
    public int RemainingTime
    {
        get => _remainingTime;
        set
        {
            _remainingTime = value;
            RaisePropertyChanged();
        }
    }



    public Question CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            RaisePropertyChanged();
        }
    }
    public string Answer1
    {
        get => _answer1;
        set
        {
            _answer1 = value;
            RaisePropertyChanged();
        }
    }
    public string Answer2
    {
        get => _answer2;
        set
        {
            _answer2 = value;
            RaisePropertyChanged();
        }
    }
    public string Answer3
    {
        get => _answer3;
        set
        {
            _answer3 = value;
            RaisePropertyChanged();
        }
    }
    public string Answer4
    {
        get => _answer4;
        set
        {
            _answer4 = value;
            RaisePropertyChanged();
        }
    }
}
