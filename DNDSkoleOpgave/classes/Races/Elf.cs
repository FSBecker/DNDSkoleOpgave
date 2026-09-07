using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Races;

public sealed class Elf : CharacterRace
{
    public Elf() : base("Elf", "A graceful race with keen intellect.")
    {
        StatBuffs[CharacterStat.Dexterity] = 2;
        StatBuffs[CharacterStat.Intelligence] = 1;
    }
}
