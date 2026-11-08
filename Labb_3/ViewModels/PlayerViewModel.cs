using Labb_3.Command;
using Labb_3.Models;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace Labb_3.ViewModels;

class PlayerViewModel : ViewModelBase
{
    string _answer1;
    string _answer2;
    string _answer3;
    string _answer4;
    private readonly Random _random = new();
    public RelayCommand AnswerHandlerCommand { get; }

    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
    private List<Question> _shuffledQuestions;
    private int _currentQuestionIndex;
    public string QuestionCount => $"Question {_currentQuestionIndex} of {_mainWindowViewModel?.ActivePack.Questions.Count}";
    public Question _currentQuestion;
    public Question CurrentQuestion
    {
        get => _currentQuestion;
        set
        {
            _currentQuestion = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(QuestionCount));
        }
    }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        stopwatch = new Stopwatch();
        _shuffledQuestions = new List<Question>();
        AnswerHandlerCommand = new RelayCommand(_ => AnswerHandler());
    }
    private SolidColorBrush _buttonColour = new SolidColorBrush(Colors.LightGray);
    public SolidColorBrush ButtonColour
    {
        get => _buttonColour;
        set
        {
            _buttonColour = value;
            RaisePropertyChanged();
        }
    }


    private void AnswerHandler()
    {
        if (SelectedAnswer == CurrentQuestion.CorrectAnswer)
        {

        }
        else
        {

        }


    }

    public void ShuffleQuestion()
    {
        if (ActivePack?.Questions is null) return;

        _shuffledQuestions = ActivePack.Questions.ToList();
        Shuffle(_shuffledQuestions);
        _currentQuestionIndex = 0;

    }
    public void ShuffleAnswer(Question current)
    {
        var answerList = new List<string>
        {
            current.CorrectAnswer,
            current.IncorrectAnswers[0],
            current.IncorrectAnswers[1],
            current.IncorrectAnswers[2]
        };

        Shuffle(answerList);
        Answer1 = answerList[0];
        Answer2 = answerList[1];
        Answer3 = answerList[2];
        Answer4 = answerList[3];
    }

    private void Shuffle<T>(List<T> items)
    {
        for (int i = items.Count - 1; i > 0 - 1; i--)
        {
            int r = _random.Next(i + 1);
            (items[i], items[r]) = (items[r], items[i]);
        }
    }


    public DispatcherTimer timer;
    public Stopwatch stopwatch;
    private TimeSpan countdownTime;
    public void StartTimer()
    {
        if (ActivePack is null) return;

        timer.Stop();
        stopwatch.Reset();
        countdownTime = TimeSpan.FromSeconds(ActivePack.TimeLimitInSeconds);
        RemainingTime = (int)countdownTime.TotalSeconds;
        stopwatch.Start();
        timer.Start();
    }
    public void Timer_Tick(object? sender, EventArgs e)
    {
        TimeSpan remainingTime = countdownTime - stopwatch.Elapsed;
        if (remainingTime <= TimeSpan.Zero)
        {
            timer.Stop();
            stopwatch.Stop();
            RemainingTime = 0;
            return;
        }
        RemainingTime = (int)remainingTime.TotalSeconds;
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

    public void NextQuestion()
    {
        if (_currentQuestionIndex >= _shuffledQuestions.Count)
        {
            return;
        }
        CurrentQuestion = _shuffledQuestions[_currentQuestionIndex++];
        ShuffleAnswer(CurrentQuestion);


        RaisePropertyChanged(nameof(QuestionCount));
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
