using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class Ranger : CharacterClass
{
    public Ranger() : base("Ranger", "A quick and resourceful wilderness fighter.", 20, 3)
    {
        StatBuffs[CharacterStat.Dexterity] = 2;
        StartingItemIds.AddRange(["light-leather-armor", "bow", "short-sword"]);
        LevelLockedActions[1] = ["bow-shot", "short-sword-strike"];
    }
}
