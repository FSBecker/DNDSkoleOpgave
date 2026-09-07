using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.UI;
using Spectre.Console;

namespace DNDSkoleOpgave.Characters;

public sealed class ConsoleCharacterCreator
{
    public PlayerCharacter Create()
    {
        GameMenu.ShowTitle("Opret karakter");
        string name = AskName();
        string race = ChooseRace();
        string characterClass = ChooseClass();
        PlayerCharacter player = CharacterCreator.Create(name, characterClass, race);
        WaveRewards.GiveStartingSupplies(player);
        return player;
    }

    private static string AskName() => AnsiConsole.Prompt(
        new TextPrompt<string>("Karakterens navn:")
            .Validate(name => string.IsNullOrWhiteSpace(name)
                ? ValidationResult.Error("Skriv et navn.")
                : ValidationResult.Success()));

    private static string ChooseRace() =>
        GameMenu.Choose("Vælg race", CharacterCreator.AvailableRaces, Markup.Escape);

    private static string ChooseClass() =>
        GameMenu.Choose("Vælg klasse", CharacterCreator.AvailableClasses, Markup.Escape);
}
