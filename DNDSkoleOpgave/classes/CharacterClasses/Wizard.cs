using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class Wizard : CharacterClass
{
    public Wizard() : base("Wizard", "A spellcaster who uses intelligence.", 6, 4)
    {
        StatBuffs[CharacterStat.Intelligence] = 2;
        StartingItemIds.AddRange(["crystal-staff", "wizard-robes", "orb"]);
        LevelLockedActions[1] = ["staff-strike", "healing-hand", "weak-fireball", "witch-bolt"];
    }
}
