using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using Spectre.Console;

namespace DNDSkoleOpgave.UI;

public static class CombatMenu
{
    public static CombatChoice? ChooseAction(CombatParticipant participant)
    {
        List<CombatChoice> choices = CombatChoices.GetAvailable(participant);
        List<string> labels = new();
        for (int i = 0; i < choices.Count; i++)
            labels.Add($"{i + 1}. {DescribeAction(choices[i])}");
        labels.Add("Afslut tur");

        string chosen = GameMenu.Choose("Vælg handling", labels, Markup.Escape);
        int index = labels.IndexOf(chosen);
        return index < choices.Count ? choices[index] : null;
    }

    public static EnemyCharacter? ChooseTarget(CombatInstance battle)
    {
        List<EnemyCharacter> enemies = battle.Enemies.Where(enemy => !enemy.IsDefeated).ToList();
        if (enemies.Count == 1)
            return enemies[0];

        List<string> labels = enemies.Select(enemy =>
            $"{enemy.CharacterName} · {enemy.CurrentHealth}/{enemy.MaximumHealth} HP · AC {enemy.CalculateArmorClass()}").ToList();
        labels.Add("Tilbage til handlinger");

        string chosen = GameMenu.Choose("Vælg fjende", labels, Markup.Escape);
        int index = labels.IndexOf(chosen);
        return index < enemies.Count ? enemies[index] : null;
    }

    private static string DescribeAction(CombatChoice choice)
    {
        string description = $"{choice.Name} · {choice.ActionCost} AP";
        if (choice.Action is not null)
            description += $" · {choice.Action.DiceAmount}d{choice.Action.DiceSides} + {choice.Action.PrimaryStat}";
        if (choice.UsesHealingAbility)
            description += " · bruger kampens healing-evne";
        if (choice.Item is not null)
            description += " · forbruges";
        return description;
    }
}
