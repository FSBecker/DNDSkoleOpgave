using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.UI;
using Spectre.Console;

namespace DNDSkoleOpgave.Characters;

public sealed class ConsoleCharacterMenu
{
    public PlayerCharacter CreateOrLoad(CharacterJsonRepository repository)
    {
        IReadOnlyList<string> savedIds = repository.GetSavedCharacterIds();
        if (savedIds.Count == 0)
            return CreateCharacter();

        string choice = GameMenu.Choose("Karakter", "Opret ny karakter", "Indlæs karakter");
        if (choice == "Opret ny karakter")
            return CreateCharacter();
        return LoadCharacter(repository, savedIds);
    }

    private static PlayerCharacter CreateCharacter() => new ConsoleCharacterCreator().Create();

    private static PlayerCharacter LoadCharacter(CharacterJsonRepository repository, IReadOnlyList<string> savedIds)
    {
        List<PlayerCharacter> characters = new();
        foreach (string id in savedIds)
            characters.Add(repository.Load(id));
        return GameMenu.Choose("Vælg gemt karakter", characters, DescribeCharacter);
    }

    private static string DescribeCharacter(PlayerCharacter player)
    {
        string status = player.IsDefeated ? "BESEJRET" : $"næste wave {player.NextWave}";
        return Markup.Escape($"{player.CharacterName} · Level {player.Level} · {player.CurrentHealth}/{player.MaximumHealth} HP · {status}");
    }
}
