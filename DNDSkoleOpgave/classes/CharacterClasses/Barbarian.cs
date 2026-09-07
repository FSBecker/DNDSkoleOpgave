using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class Barbarian : CharacterClass
{
    public Barbarian() : base("Barbarian", "A strong close-combat fighter.", 12, 7)
    {
        StatBuffs[CharacterStat.Strength] = 2;
        LevelLockedActions[1] = "axe-swing";
    }
}
