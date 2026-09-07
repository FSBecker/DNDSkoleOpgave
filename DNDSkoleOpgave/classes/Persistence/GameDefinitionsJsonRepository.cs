using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DNDSkoleOpgave.Persistence;

public sealed class GameDefinitionsJsonRepository
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented,
        MissingMemberHandling = MissingMemberHandling.Error,
        Converters = { new StringEnumConverter() }
    };

    public void Save(string path, GameDefinitions definitions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(definitions);
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, JsonConvert.SerializeObject(definitions, Settings));
    }

    public GameDefinitions Load(string path)
    {
        string json = File.ReadAllText(Path.GetFullPath(path));
        return JsonConvert.DeserializeObject<GameDefinitions>(json, Settings)
            ?? throw new JsonSerializationException("The game definitions file was empty.");
    }

    public void CreateAndSave(string path) => Save(path, GameDefinitionsCreator.Create());

    public GameDefinitions LoadOrCreate(string path)
    {
        if (File.Exists(path))
        {
            return Load(path);
        }

        GameDefinitions definitions = GameDefinitionsCreator.Create();
        Save(path, definitions);
        return definitions;
    }
}
