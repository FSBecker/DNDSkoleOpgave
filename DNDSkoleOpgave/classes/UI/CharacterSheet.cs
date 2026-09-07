using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Game;
using Spectre.Console;

namespace DNDSkoleOpgave.UI;

public static class CharacterSheet
{
    public static void Show(PlayerCharacter player)
    {
        AnsiConsole.MarkupLine($"[bold]{Markup.Escape(player.CharacterName)}[/] · Level {player.Level} · " +
            Markup.Escape($"{player.CharacterRace.RaceName} {player.CharacterClass.ClassName}"));
        AnsiConsole.MarkupLine(BattleHud.HealthBar(player));
        AnsiConsole.WriteLine($"AC {player.CalculateArmorClass()} · {player.ActionPoints} AP · " +
            $"Potions {WaveRewards.CountPotions(player)}/{Difficulty.MaximumPotions} · Næste wave {player.NextWave}");

        Table stats = new Table().AddColumn("Stat").AddColumn("Værdi").AddColumn("Modifier");
        foreach (CharacterStat stat in Enum.GetValues<CharacterStat>())
            stats.AddRow(stat.ToString(), player.GetStat(stat).ToString(), player.GetStatModifier(stat).ToString("+0;-0;0"));
        AnsiConsole.Write(stats);
        AnsiConsole.WriteLine("Udstyr: " + string.Join(", ", player.Equipment.AllEquipment.Values.Select(item => item.Name)));
        AnsiConsole.WriteLine("Taske: " + string.Join(", ", player.Inventory.Select(item => item.Name)));
    }
}
