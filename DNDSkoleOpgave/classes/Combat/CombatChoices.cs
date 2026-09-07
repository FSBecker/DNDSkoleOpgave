using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Items;

namespace DNDSkoleOpgave.Combat;

public static class CombatChoices
{
    public static List<CombatChoice> GetAvailable(CombatParticipant participant)
    {
        List<CombatChoice> choices = GetKnownChoices(participant.Character);
        return choices.Where(participant.CanUse).ToList();
    }

    public static List<CombatChoice> GetKnownChoices(CoreCharacter character)
    {
        List<CombatChoice> choices = new();
        HashSet<string> addedActions = new(StringComparer.OrdinalIgnoreCase);

        foreach (CombatAction action in character.Actions)
        {
            if (addedActions.Add(action.ID))
                choices.Add(new CombatChoice(action));
        }

        foreach (EquipmentItem equipment in character.Equipment.AllEquipment.Values)
        {
            if (equipment is not WeaponItem weapon)
                continue;
            foreach (CombatAction action in weapon.GetActions())
            {
                if (addedActions.Add(action.ID))
                    choices.Add(new CombatChoice(action));
            }
        }

        foreach (CoreItem item in character.Inventory)
        {
            if (item is ConsumableItem consumable)
                choices.Add(new CombatChoice(consumable));
        }

        return choices;
    }
}
