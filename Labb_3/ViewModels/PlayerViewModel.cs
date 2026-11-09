using Labb_3.Command;
using Labb_3.Models;
using System.Diagnostics;
using System.Windows.Threading;

namespace Labb_3.ViewModels;

class PlayerViewModel : ViewModelBase
{

    private readonly Random _random = new();
    public RelayCommand StartQuizCommand { get; }
    public AsyncRelayCommand AnswerHandlerCommand { get; }
    public string ScoreCounter => $"You got {_currentScore} out of {_mainWindowViewModel?.ActivePack.Questions.Count} answers correct!";
    private List<ShuffleAnswers> _shuffledAnswers = new();
    public List<ShuffleAnswers> ShuffledAnswers => _shuffledAnswers;

    private readonly MainWindowViewModel? _mainWindowViewModel;
    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
    private List<Question> _shuffledQuestions = new();
    private int _currentQuestionIndex;
    private int _currentScore;
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
    private bool IsCorrectAnswer(string selectedAnswer)
    {
        ShuffleAnswers answer = _shuffledAnswers.First(option => option.Answer == selectedAnswer);
        return answer.IsCorrect;
    }
    private void StartQuiz()
    {
        if (ActivePack is null || ActivePack.Questions.Count == 0) return;
        timer.Stop();
        _currentScore = 0;
        _currentQuestionIndex = 0;

        _shuffledQuestions = ActivePack.Questions.ToList();
        Shuffle(_shuffledQuestions);

        NextQuestion();
        StartTimer();

    }
    public string SelectedAnswer
    {
        get => _selectedAnswer;
        set
        {
            _selectedAnswer = value;
            RaisePropertyChanged();
        }
    }
    public string WrongAnswerButton
    {
        get => _wrongAnswerButton;
        set
        {
            _wrongAnswerButton = value;
            RaisePropertyChanged();
        }
    }
    public string CorrectAnswerButton
    {
        get => _correctAnswerButton;
        set
        {
            _correctAnswerButton = value;
            RaisePropertyChanged();
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
        StartQuizCommand = new RelayCommand(_ => StartQuiz());
        AnswerHandlerCommand = new AsyncRelayCommand(parameter => AnswerHandler(parameter?.ToString() ?? string.Empty));
    }



    private async Task AnswerHandler(string selectedAnswer)
    {


        if (IsCorrectAnswer(selectedAnswer))
        {
            _currentScore++;
            CorrectAnswerButton = selectedAnswer;
        }
        else
        {
            WrongAnswerButton = selectedAnswer;
            CorrectAnswerButton = _shuffledAnswers.First(option => option.IsCorrect).Answer;
        }
        await Task.Delay(1000);
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
            return;
        }
        RemainingTime = (int)remainingTime.TotalSeconds;
    }
    private int _remainingTime;
    private string _selectedAnswer;
    private string _wrongAnswerButton;
    private string _correctAnswerButton;

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
