using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatActionExecutor
{
    private readonly IDiceRoller _dice;

    public CombatActionExecutor(IDiceRoller dice) => _dice = dice;

    public List<string> Execute(CombatParticipant participant, CombatChoice choice, CoreCharacter target)
    {
        ValidateChoice(participant, choice, target);
        participant.SpendResources(choice);

        CoreCharacter actor = participant.Character;
        List<string> messages = new() { $"{actor.CharacterName} bruger {choice.Name} på {target.CharacterName}." };

        if (choice.Item is not null)
            UseItem(actor, choice, target, messages);
        else
            UseAction(actor, choice.Action!, target, messages);

        if (target.IsDefeated)
            messages.Add($"{target.CharacterName} er besejret!");
        return messages;
    }

    private static void ValidateChoice(CombatParticipant participant, CombatChoice choice, CoreCharacter target)
    {
        bool owned = CombatChoices.GetKnownChoices(participant.Character).Any(known =>
            known.Action == choice.Action && known.Item == choice.Item);
        if (!owned)
            throw new InvalidOperationException("Karakteren har ikke adgang til den handling eller det item.");
        if (target.IsDefeated)
            throw new InvalidOperationException("Målet er allerede besejret.");
        if (choice.NeedsEnemyTarget && ReferenceEquals(target, participant.Character))
            throw new InvalidOperationException("Vælg en fjende som mål.");
        if (!choice.NeedsEnemyTarget && !ReferenceEquals(target, participant.Character))
            throw new InvalidOperationException("Denne handling bruges på dig selv.");
    }

    private static void UseItem(CoreCharacter actor, CombatChoice choice, CoreCharacter target, List<string> messages)
    {
        int healthBefore = target.CurrentHealth;
        choice.Item!.Use(actor, target);
        messages.Add($"HP: {healthBefore} → {target.CurrentHealth}. Itemet er brugt op.");
    }

    private void UseAction(CoreCharacter actor, CombatAction action, CoreCharacter target, List<string> messages)
    {
        if (action is SpellAction healingSpell && healingSpell.TargetsAlly)
        {
            HealCharacter(actor, action, messages);
        }
        else
        {
            if (!AttackHits(actor, action, target, messages))
                return;
            DealDamage(actor, action, target, messages);
            ApplyBurning(action, target, messages);
        }

        ApplyExtraEffects(actor, action, target);
    }

    private bool AttackHits(CoreCharacter actor, CombatAction action, CoreCharacter target, List<string> messages)
    {
        if (action is SpellAction spell && spell.SavingThrowDifficulty is int difficulty)
        {
            int savingThrow = _dice.Roll(20) + target.GetStatModifier(CharacterStat.Dexterity);
            bool failedSave = savingThrow < difficulty;
            messages.Add($"Redningsslag: {savingThrow} mod {difficulty}. " + (failedSave ? "Ramt!" : "Undveget!"));
            return failedSave;
        }

        int roll = _dice.Roll(20);
        int attack = roll + actor.GetStatModifier(action.PrimaryStat) + actor.AttackBonus;
        bool hit = roll == 20 || (roll != 1 && attack >= target.CalculateArmorClass());
        messages.Add($"Angreb: {attack} mod AC {target.CalculateArmorClass()}. " + (hit ? "Ramt!" : "Forbi!"));
        return hit;
    }

    private void DealDamage(CoreCharacter actor, CombatAction action, CoreCharacter target, List<string> messages)
    {
        int damage = Math.Max(1, action.RollDamage(_dice) + actor.GetStatModifier(action.PrimaryStat));
        int healthBefore = target.CurrentHealth;
        target.TakeDamage(damage);
        messages.Add($"{target.CharacterName} mister {healthBefore - target.CurrentHealth} HP.");
    }

    private void HealCharacter(CoreCharacter actor, CombatAction action, List<string> messages)
    {
        int healing = Math.Max(1, action.RollDamage(_dice) + actor.GetStatModifier(action.PrimaryStat));
        int healthBefore = actor.CurrentHealth;
        actor.Heal(healing);
        messages.Add($"{actor.CharacterName} får {actor.CurrentHealth - healthBefore} HP tilbage. Healing-evnen er brugt for denne kamp.");
    }

    private void ApplyBurning(CombatAction action, CoreCharacter target, List<string> messages)
    {
        if (target.IsDefeated || action is not SpellAction spell)
            return;
        if (spell.BurningDamageDiceSides is not int damageSides || spell.BurningDurationDiceSides is not int durationSides)
            return;

        int damage = _dice.Roll(damageSides);
        int turns = _dice.Roll(durationSides);
        target.AddActiveEffect(new ActionEffect(ActionEffectType.Damage, damage,
            EffectTarget.Enemy, EffectTiming.OverTime, turns));
        messages.Add($"Ild: {damage} skade i starten af målets næste {turns} ture.");
    }

    private static void ApplyExtraEffects(CoreCharacter actor, CombatAction action, CoreCharacter target)
    {
        foreach (ActionEffect effect in action.Effects)
        {
            if (effect.Target == EffectTarget.Self || !target.IsDefeated)
                effect.Apply(actor, target);
        }
    }
}
