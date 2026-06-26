using System.Text.Json;
using SecurityControlEvidenceTracker.Models;

namespace SecurityControlEvidenceTracker.Services;

public class FileStorage
{
    private readonly string _filePath = "controls.json";

    public List<SecurityControl> LoadControls()
    {
        if (!File.Exists(_filePath))
        {
            return new List<SecurityControl>();
        }

        string json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<SecurityControl>();
        }

        return JsonSerializer.Deserialize<List<SecurityControl>>(json) ?? new List<SecurityControl>();
    }

    public void SaveControls(List<SecurityControl> controls)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(controls, options);
        File.WriteAllText(_filePath, json);
    }
}