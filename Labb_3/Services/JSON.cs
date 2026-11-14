using Labb_3.Models;
using Labb_3.ViewModels;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Labb_3.Services;

class JSON
{


    private static JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        IncludeFields = true,
    };
    private static string FolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizPacks");
    private static string FilePath => Path.Combine(FolderPath, "QuestionPacks.json");
    public async Task SaveAsync(IEnumerable<QuestionPackViewModel> packs)
    {
        Directory.CreateDirectory(FolderPath);

        var models = packs.Select(pack => pack.ToModel()).ToList();

        var json = JsonSerializer.Serialize(models, SerializerOptions);

        await File.WriteAllTextAsync(FilePath, json);

    }
    public async Task<List<QuestionPackViewModel>> LoadAsync()
    {

        if (!File.Exists(FilePath)) return new List<QuestionPackViewModel>();

        var json = await File.ReadAllTextAsync(FilePath);

        var models = JsonSerializer.Deserialize<List<QuestionPack>>(json) ?? new List<QuestionPack>();

        return models.Select(model => new QuestionPackViewModel(model)).ToList();



    }


}
