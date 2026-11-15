
using System.Text.Json.Serialization;

namespace Labb_3.Models;

class Question
{
    public Question(string query, string correctAnswer, string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
    {
        Query = query;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = new string[] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };

    }

    [JsonPropertyName("question")]
    public string Query { get; set; }
    [JsonPropertyName("correct_answer")]
    public string CorrectAnswer { get; set; }
    [JsonPropertyName("incorrect_answers")]
    public string[] IncorrectAnswers { get; set; }
    [JsonIgnore]
    public bool HasValidAnswers
    {
        get =>
            !string.IsNullOrWhiteSpace(CorrectAnswer) &&
            IncorrectAnswers.Length is 3 &&
            IncorrectAnswers.Any(answer => !string.IsNullOrWhiteSpace(answer));
    }
    public Question()
    {
        IncorrectAnswers = Array.Empty<string>();

    }

}
