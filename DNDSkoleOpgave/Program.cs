using DNDSkoleOpgave.Admin;
using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Persistence;

string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
string definitionsPath = Path.Combine(dataDirectory, "game-definitions.json");
GameDefinitionsJsonRepository definitionsRepository = new();
GameDefinitions definitions = definitionsRepository.LoadOrCreate(definitionsPath);

Console.Write("Start game or open admin creator? [G/A]: ");
if (string.Equals(Console.ReadLine()?.Trim(), "A", StringComparison.OrdinalIgnoreCase))
{
    AdminCreator adminCreator = new(Console.In, Console.Out);
    adminCreator.Run(definitions, definitionsRepository, definitionsPath);
    return;
}

CharacterJsonRepository characterRepository = new(Path.Combine(dataDirectory, "characters"));
ConsoleCharacterMenu characterMenu = new(Console.In, Console.Out);
PlayerCharacter player = characterMenu.CreateOrLoad(characterRepository);
string savePath = characterRepository.Save(player);
Console.WriteLine($"Character saved to {savePath}");
