using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public static class EnemyDecision
{
    public static CombatChoice? ChooseAction(CombatParticipant participant, IDiceRoller dice)
    {
        List<CombatChoice> choices = CombatChoices.GetAvailable(participant);
        bool needsHealing = participant.Character.CurrentHealth < participant.Character.MaximumHealth / 2;

        if (needsHealing)
        {
            CombatChoice? healing = choices.FirstOrDefault(choice => !choice.NeedsEnemyTarget);
            if (healing is not null)
                return healing;
        }

        List<CombatChoice> attacks = choices.Where(choice => choice.NeedsEnemyTarget).ToList();
        if (attacks.Count == 0)
            return null;
        return attacks[dice.Roll(attacks.Count) - 1];
    }
}
