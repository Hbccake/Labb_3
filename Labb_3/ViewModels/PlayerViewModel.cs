using Labb_3.Command;
using Labb_3.Models;
using System.Diagnostics;
using System.Windows.Threading;

namespace Labb_3.ViewModels;

class PlayerViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
    private readonly Random _random = new();
    public RelayCommand StartQuizCommand { get; }
    public AsyncRelayCommand AnswerHandlerCommand { get; }
    private DispatcherTimer timer;
    private Stopwatch stopwatch;
    private int _countdown;

    public string ScoreCounter => $"You got {_currentScore} out of {_mainWindowViewModel?.ActivePack.Questions.Count} answers correct!";
    public string QuestionCount => $"Question {_currentQuestionIndex} of {_mainWindowViewModel?.ActivePack.Questions.Count}";
    private List<ShuffleAnswers> _shuffledAnswers = [
        new ShuffleAnswers(string.Empty,false),
        new ShuffleAnswers(string.Empty,false),
        new ShuffleAnswers(string.Empty,false),
        new ShuffleAnswers(string.Empty,false)

        ];
    public List<ShuffleAnswers> ShuffledAnswers => _shuffledAnswers;
    private List<Question> _shuffledQuestions = new();
    private int _currentQuestionIndex;
    private int _currentScore;
    public string CorrectAnswer => _shuffledAnswers.FirstOrDefault(answer => answer.IsCorrect).Answer;
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
    private string? _selectedAnswer;
    public string? SelectedAnswer
    {
        get => _selectedAnswer;
        set => SetProperty(ref _selectedAnswer, value);
    }
    private bool _hasAnswered;
    public bool HasAnswered
    {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }

    private bool IsCorrectAnswer(string selectedAnswer)
    {
        ShuffleAnswers answer = _shuffledAnswers.First(option => option.Answer == selectedAnswer);
        return answer.IsCorrect;
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

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        timer.Tick += Timer_Tick;
        stopwatch = new Stopwatch();

        _shuffledQuestions = new List<Question>();
        StartQuizCommand = new RelayCommand(_ => StartQuiz());
        AnswerHandlerCommand = new AsyncRelayCommand(parameter => AnswerHandler(parameter?.ToString() ?? string.Empty));

    }

    private void StartQuiz()
    {
        if (ActivePack is null || ActivePack.Questions.Count == 0) return;
        _mainWindowViewModel.ChangeViewCommand.Execute(ViewType.PlayerView);
        RaisePropertyChanged(nameof(ScoreCounter));
        timer.Stop();
        _currentScore = 0;
        _currentQuestionIndex = 0;
        HasAnswered = false;

        _shuffledQuestions = ActivePack.Questions.ToList();
        Shuffle(_shuffledQuestions);

        NextQuestion();

    }

    private async Task AnswerHandler(string selectedAnswer)
    {
        if (HasAnswered) return;
        SelectedAnswer = selectedAnswer;
        HasAnswered = true;
        timer.Stop();
        stopwatch.Stop();
        if (IsCorrectAnswer(selectedAnswer))
        {
            _currentScore++;
            RaisePropertyChanged(nameof(ScoreCounter));

        }

        await Task.Delay(3000);
        NextQuestion();
    }

    public struct ShuffleAnswers
    {
        public string Answer { get; }
        public bool IsCorrect { get; }
        public ShuffleAnswers(string answer, bool iscorrect)
        {
            Answer = answer;
            IsCorrect = iscorrect;
        }
        public string Image => IsCorrect ? "/Assets/Correct.png" : "/Assets/Incorrect.png";
    }

    private void Shuffle<T>(List<T> items)
    {
        for (int i = items.Count - 1; i > 0 - 1; i--)
        {
            int r = _random.Next(i + 1);
            (items[i], items[r]) = (items[r], items[i]);
        }
    }

    public void Timer_Tick(object? sender, EventArgs e)
    {
        int elapsedSeconds = (int)Math.Floor(stopwatch.Elapsed.TotalSeconds);
        int remainingSeconds = Math.Max(0, _countdown - elapsedSeconds);
        if (RemainingTime != remainingSeconds)
            RemainingTime = remainingSeconds;


        if (elapsedSeconds >= _countdown)
        {
            timer.Stop();
            stopwatch.Stop();

            HasAnswered = true;
            NextQuestion();
        }

    }
    public void StartTimer()
    {
        if (ActivePack is null) return;

        timer.Stop();
        stopwatch.Restart();
        _countdown = ActivePack.TimeLimitInSeconds;
        RemainingTime = _countdown;
        timer.Start();
    }

    public void NextQuestion()
    {

        if (_currentQuestionIndex >= _shuffledQuestions.Count)
        {
            timer.Stop();

            _mainWindowViewModel.ChangeViewCommand.Execute(ViewType.EndView);
            return;
        }
        HasAnswered = false;
        SelectedAnswer = null;
        CurrentQuestion = _shuffledQuestions[_currentQuestionIndex++];
        _shuffledAnswers = [
            new ShuffleAnswers(CurrentQuestion.CorrectAnswer,true),
            new ShuffleAnswers(CurrentQuestion.IncorrectAnswers[0], false),
            new ShuffleAnswers(CurrentQuestion.IncorrectAnswers[1], false),
            new ShuffleAnswers(CurrentQuestion.IncorrectAnswers[2], false)
            ];
        Shuffle(ShuffledAnswers);
        RaisePropertyChanged(nameof(ShuffledAnswers));
        RaisePropertyChanged(nameof(QuestionCount));
        StartTimer();
    }

}
