using Labb_3.Models;
using Newtonsoft.Json;
using System.Net.Http;
namespace Labb_3.Services;

class Trivia
{
    private class CategoryResponse
    {
        [JsonProperty("trivia_categories")]
        public List<TriviaCategory> Categories { get; set; } = new();

    }
    public class TriviaCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> GetTriviaQuestionsAsync(int amount = 1, int category = 9, string difficulty = "Medium")
    {
        HttpClient client = new HttpClient();

        string url = $"https://opentdb.com/api.php?amount={amount}&category={category}&difficulty={difficulty}&type=multiple";

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();


    }


    public async Task<List<QuestionPack>> FetchQuestions()
    {
        string json = await GetTriviaQuestionsAsync();
        return JsonConvert.DeserializeObject<List<QuestionPack>>(json);
    }

    public async Task<List<TriviaCategory>> ImportCategoryAsync()
    {
        string url = "https://opentdb.com/api_category.php";
        string json = await client.GetStringAsync(url);
        return JsonConvert.DeserializeObject<CategoryResponse>(json)?.Categories ?? new();
    }

}
