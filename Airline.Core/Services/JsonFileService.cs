using System.Text.Json;

namespace Airline.Core.Services;

public class JsonFileService<T>
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public JsonFileService(string filePath)
    {
        _filePath = filePath;
    }

    public void Save(List<T> data)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        string json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(_filePath, json);
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
    }
}