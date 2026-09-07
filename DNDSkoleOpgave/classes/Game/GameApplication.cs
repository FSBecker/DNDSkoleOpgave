using DNDSkoleOpgave.Admin;
using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.UI;
using DNDSkoleOpgave.Utilities;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Game;

public sealed class GameApplication
{
    private readonly IDiceRoller _dice;
    private readonly string _dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
    private readonly GameDefinitionsJsonRepository _definitionsRepository = new();

    public GameApplication(IDiceRoller dice) => _dice = dice;

    public void Run()
    {
        try
        {
            GameDefinitions definitions = LoadGameContent();
            GameMenu.ShowTitle("DnD · Overlev så mange waves som muligt");
            string choice = GameMenu.Choose("Hovedmenu", "Spil", "Admin-editor", "Afslut");
            if (choice == "Spil")
                StartGame(definitions);
            if (choice == "Admin-editor")
                OpenAdminEditor(definitions);
        }
        catch (Exception exception) when (exception is IOException or JsonException or
            InvalidOperationException or ArgumentException or KeyNotFoundException)
        {
            GameMenu.ShowMessage($"Spillet kunne ikke fortsætte: {exception.Message}");
            GameMenu.Pause();
        }
    }

    private GameDefinitions LoadGameContent()
    {
        string path = Path.Combine(_dataDirectory, "game-definitions.json");
        GameDefinitions definitions = _definitionsRepository.LoadOrCreate(path);
        GameCatalog.Load(definitions);
        return definitions;
    }

    private void StartGame(GameDefinitions definitions)
    {
        CharacterJsonRepository saves = new(Path.Combine(_dataDirectory, "characters"));
        ConsoleCharacterMenu menu = new();
        PlayerCharacter player = menu.CreateOrLoad(saves);
        if (player.IsDefeated)
        {
            GameMenu.ShowMessage("Denne karakter er besejret. Opret en ny karakter for at spille igen.");
            GameMenu.Pause();
            return;
        }

        saves.Save(player);
        CharacterSheet.Show(player);
        GameMenu.ShowMessage("1–3 fjender pr. wave. Én healing-evne pr. kamp. Kun 2 HP fra hvilet efter en sejr.");
        GameMenu.Pause();
        WaveGame game = new(player, saves, definitions, _dice);
        game.Run();
    }

    private void OpenAdminEditor(GameDefinitions definitions)
    {
        string path = Path.Combine(_dataDirectory, "game-definitions.json");
        AdminCreator admin = new(Console.In, Console.Out);
        admin.Run(definitions, _definitionsRepository, path);
    }
}
