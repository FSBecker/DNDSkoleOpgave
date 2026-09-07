using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Races;

public sealed class Dwarf : CharacterRace
{
    public Dwarf() : base("Dwarf", "A sturdy and powerful race.")
    {
        StatBuffs[CharacterStat.Constitution] = 2;
        StatBuffs[CharacterStat.Strength] = 1;
    }
}
