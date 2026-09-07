using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Races;

public sealed class Human : CharacterRace
{
    public Human() : base("Human", "A versatile and adaptable race.")
    {
        StatBuffs[CharacterStat.Strength] = 1;
        StatBuffs[CharacterStat.Dexterity] = 1;
        StatBuffs[CharacterStat.Constitution] = 1;
    }
}
