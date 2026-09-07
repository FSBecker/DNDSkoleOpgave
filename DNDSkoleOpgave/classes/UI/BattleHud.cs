using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using Spectre.Console;

namespace DNDSkoleOpgave.UI;

public static class BattleHud
{
    public static void Render(CombatInstance battle, CombatParticipant? active = null, CoreCharacter? target = null)
    {
        AnsiConsole.Clear();
        ShowTurnOrder(battle, active);
        ShowEnemies(battle, active, target);
        AnsiConsole.Write(CreateCharacterPanel(battle.Player, active, target));
        ShowMessages();
    }

    private static void ShowTurnOrder(CombatInstance battle, CombatParticipant? active)
    {
        List<string> labels = new();
        foreach (CombatParticipant participant in battle.TurnOrder)
        {
            string name = Markup.Escape(participant.Character.CharacterName);
            string label = $"{name} ({participant.Initiative})";
            if (participant.Character.IsDefeated)
                label = $"[grey strikethrough]{label}[/]";
            else if (participant == active)
                label = $"[bold yellow]▶ {label}[/]";
            labels.Add(label);
        }

        AnsiConsole.Write(new Rule($"[bold]Wave {battle.Player.NextWave} · Runde {battle.Round}[/]"));
        AnsiConsole.MarkupLine("[grey]Turorden:[/] " + string.Join(" → ", labels));
        if (active is not null)
            AnsiConsole.MarkupLine($"[bold yellow]AKTIV: {Markup.Escape(active.Character.CharacterName)}[/]  " +
                $"{active.RemainingActionPoints} AP · Healing-evner tilbage: {active.HealingAbilitiesLeft}");
        AnsiConsole.MarkupLine("[grey]Gul = aktiv figur · Blå = valgt mål · Besejrede springes over[/]");
    }

    private static void ShowEnemies(CombatInstance battle, CombatParticipant? active, CoreCharacter? target)
    {
        List<Panel> panels = new();
        foreach (EnemyCharacter enemy in battle.Enemies)
            panels.Add(CreateCharacterPanel(enemy, active, target));
        AnsiConsole.Write(new Columns(panels).Collapse());
    }

    private static Panel CreateCharacterPanel(CoreCharacter character, CombatParticipant? active, CoreCharacter? target)
    {
        bool isActive = active?.Character == character;
        Color color = character is PlayerCharacter ? Color.Green : Color.Red;
        string header = Markup.Escape(character.CharacterName);
        if (character == target)
        {
            color = Color.Blue;
            header += " · MÅL";
        }
        if (isActive)
        {
            color = Color.Yellow;
            header += " · AKTIV";
        }
        if (character.IsDefeated)
        {
            color = Color.Grey;
            header += " · BESEJRET";
        }

        Rows body = new(
            new Text(CharacterArt.GetArt(character), new Style(foreground: color)),
            new Markup(HealthBar(character)),
            new Text($"Level {character.Level} · AC {character.CalculateArmorClass()}"));

        return new Panel(body).Header(header).Border(BoxBorder.Rounded).BorderColor(color);
    }

    private static void ShowMessages()
    {
        string messages = string.Join("\n", CombatLog.RecentMessages);
        AnsiConsole.Write(new Panel(new Text(messages)).Header("Kampens log").Border(BoxBorder.Rounded));
    }

    public static string HealthBar(IDamageable character)
    {
        double fraction = (double)character.CurrentHealth / character.MaximumHealth;
        int filled = (int)Math.Round(fraction * 12);
        string color = fraction > 0.5 ? "green" : fraction > 0.25 ? "yellow" : "red";
        return $"[{color}]{new string('█', filled)}[/][grey]{new string('░', 12 - filled)}[/] " +
            $"{character.CurrentHealth}/{character.MaximumHealth} HP";
    }
}
