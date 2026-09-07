using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class Ranger : CharacterClass
{
    public Ranger() : base("Ranger", "A quick and resourceful wilderness fighter.", 8, 5)
    {
        StatBuffs[CharacterStat.Dexterity] = 2;
        LevelLockedActions[1] = "bow-shot";
    }
}
