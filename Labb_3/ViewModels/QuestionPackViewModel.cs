using Labb_3.Models;
using System.Collections.ObjectModel;
namespace Labb_3.ViewModels;

class QuestionPackViewModel : ViewModelBase
{
    public ObservableCollection<Question> Questions { get; }
    private readonly QuestionPack _model;

    public QuestionPackViewModel(QuestionPack model)
    {
        _model = model;
        Questions = new ObservableCollection<Question>(_model.Questions);

    }


    public string Name
    {
        get => _model.Name;
        set
        {
            if (_model.Name == value) return;
            _model.Name = value;
            RaisePropertyChanged();
        }
    }
    public Difficulty Difficulty
    {
        get => _model.Difficulty;
        set
        {
            if (_model.Difficulty == value) return;
            _model.Difficulty = value;
            RaisePropertyChanged();
        }
    }
    public int TimeLimitInSeconds
    {
        get => _model.TimeLimitInSeconds;
        set
        {
            if (_model.TimeLimitInSeconds == value) return;
            _model.TimeLimitInSeconds = value;
            RaisePropertyChanged();
        }
    }






}
