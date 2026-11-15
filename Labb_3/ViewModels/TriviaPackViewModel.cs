using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;

namespace Labb_3.ViewModels;

class TriviaPackViewModel : ViewModelBase
{
    public ObservableCollection<Trivia.TriviaCategory> Categories { get; } = new();
    public ObservableCollection<Question> Questions { get; } = new();
    private int _numberOfQuestions = 1;
    public int NumberOfQuestions
    {
        get => _numberOfQuestions;
        set => SetProperty(ref _numberOfQuestions, value);

    }
    private Difficulty _triviaDifficulty;
    public Difficulty TriviaDifficulty
    {
        get => _triviaDifficulty;
        set => SetProperty(ref _triviaDifficulty, value);

    }
    private int _category;
    public int Category
    {
        get => _category;
        set => SetProperty(ref _category, value);

    }

    private Trivia.TriviaCategory? _selectedCategory;
    public Trivia.TriviaCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            Category = value?.Id ?? 9;

            RaisePropertyChanged(nameof(SelectedCategory));
        }
    }
    public TriviaPackViewModel(int amount = 1, int category = 9, Difficulty difficulty = Difficulty.Medium)
    {
        NumberOfQuestions = amount;
        Category = category;
        TriviaDifficulty = difficulty;

    }
}
