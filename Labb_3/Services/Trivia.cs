using Labb_3.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Labb_3.Services;

class Trivia
{
    private class CategoryResponse
    {
        [JsonPropertyName("trivia_categories")]
        public List<TriviaCategory> Categories { get; set; } = new();
    }
    public class TriviaCategory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }

    }
    private class QuestionResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }
        [JsonPropertyName("results")]
        public List<Question> Results { get; set; } = new();
    }

    private static readonly HttpClient client = new HttpClient();


    public async Task<List<Question>> FetchQuestionsAsync(int amount = 1, int category = 9, Difficulty difficulty = Difficulty.Medium)
    {
        string url = $"https://opentdb.com/api.php?amount={amount}&category={category}&difficulty={difficulty.ToString().ToLowerInvariant()}&type=multiple&encode=url3986";

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<QuestionResponse>(json);

        if (result is null || result.ResponseCode != 0) return new List<Question>();


        return result.Results.Where(item => item.IncorrectAnswers is { Length: >= 3 })
            .Select(item => new Question(DecodeEncodedText(item.Query), DecodeEncodedText(item.CorrectAnswer), DecodeEncodedText(item.IncorrectAnswers[0]), DecodeEncodedText(item.IncorrectAnswers[1]), DecodeEncodedText(item.IncorrectAnswers[2]))).ToList();

    }

    public async Task<List<TriviaCategory>> ImportCategoryAsync()
    {
        string url = "https://opentdb.com/api_category.php";
        string json = await client.GetStringAsync(url);
        return JsonSerializer.Deserialize<CategoryResponse>(json)?.Categories ?? new();


    }

    private static string DecodeEncodedText(string code)
    {
        return Uri.UnescapeDataString(code);
    }
}
