using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.Exceptions;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatParticipant
{
    public CombatParticipant(CoreCharacter character, int initiative)
    {
        Character = character;
        Initiative = initiative;
    }

    public CoreCharacter Character { get; }
    public int Initiative { get; }
    public int RemainingActionPoints { get; private set; }
    public int HealingAbilitiesLeft { get; private set; } = Difficulty.HealingAbilitiesPerBattle;
    public bool CanAct => !Character.IsDefeated && RemainingActionPoints > 0;

    public void StartTurn() => RemainingActionPoints = Character.ActionPoints;

    public bool CanUse(CombatChoice choice)
    {
        if (!CanAct || choice.ActionCost > RemainingActionPoints)
            return false;
        if (choice.UsesHealingAbility && HealingAbilitiesLeft == 0)
            return false;
        return true;
    }

    public void SpendResources(CombatChoice choice)
    {
        if (Character.IsDefeated)
            throw new CharacterIsDefeatedException(Character.CharacterName);
        if (choice.ActionCost > RemainingActionPoints)
            throw new InsufficientActionPointsException(choice.ActionCost, RemainingActionPoints);
        if (!CanUse(choice))
            throw new InvalidOperationException("Handlingen kræver flere AP eller en ubrugt healing-evne.");

        RemainingActionPoints -= choice.ActionCost;
        if (choice.UsesHealingAbility)
            HealingAbilitiesLeft--;
    }
}
