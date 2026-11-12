using Labb_3.Models;
using System.IO;
using System.Text.Json;

namespace Labb_3.Services;

class JSON
{
    private static JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(QuestionPack pack, string filePath)
    {

        await using FileStream createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(createStream, pack, SerializerOptions);

    }
    public async Task<QuestionPack?> LoadAsync(string filePath)
    {
        await using FileStream stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<QuestionPack>(stream);
    }

    public QuestionPack? Load(string filePath)
    {
        return JsonSerializer.Deserialize<QuestionPack>(File.ReadAllText(filePath));
    }

}
