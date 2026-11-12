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
    private bool IsCorrectAnswer(string selectedAnswer)
    {
        ShuffleAnswers answer = _shuffledAnswers.First(option => option.Answer == selectedAnswer);
        return answer.IsCorrect;
    }
    private void StartQuiz()
    {
        if (ActivePack is null || ActivePack.Questions.Count == 0) return;
        _mainWindowViewModel.ShowPlayerView();
        RaisePropertyChanged(nameof(ScoreCounter));
        timer.Stop();
        _currentScore = 0;
        _currentQuestionIndex = 0;
        HasAnswered = false;

        _shuffledQuestions = ActivePack.Questions.ToList();
        Shuffle(_shuffledQuestions);

        NextQuestion();
        StartTimer();

    }
    private string? _selectedAnswer;
    public string? SelectedAnswer
    {
        get => _selectedAnswer;
        set => SetProperty(ref _selectedAnswer, value);
    }

    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
        stopwatch = new Stopwatch();

        _shuffledQuestions = new List<Question>();
        StartQuizCommand = new RelayCommand(_ => StartQuiz());
        AnswerHandlerCommand = new AsyncRelayCommand(parameter => AnswerHandler(parameter?.ToString() ?? string.Empty));
    }

    private bool _hasAnswered;
    public bool HasAnswered
    {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    private async Task AnswerHandler(string selectedAnswer)
    {
        if (HasAnswered) return;
        SelectedAnswer = selectedAnswer;
        HasAnswered = true;

        if (IsCorrectAnswer(selectedAnswer))
        {
            _currentScore++;
            RaisePropertyChanged(nameof(ScoreCounter));

        }

        await Task.Delay(3000);
        NextQuestion();
    }
    public string CorrectAnswer => _shuffledAnswers.FirstOrDefault(answer => answer.IsCorrect).Answer;

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

    private DispatcherTimer timer;
    private Stopwatch stopwatch;
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
            _mainWindowViewModel.EndQuiz();
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
        stopwatch.Reset();
        if (_currentQuestionIndex >= _shuffledQuestions.Count)
        {
            timer.Stop();

            _mainWindowViewModel.EndQuiz();
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

    }

}
