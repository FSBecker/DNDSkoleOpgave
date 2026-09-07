using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Races;

public abstract class CharacterRace
{
    protected CharacterRace(string raceName, string description)
    {
        RaceName = raceName;
        Description = description;
    }

    public string RaceName { get; }
    public string Description { get; }
    public Dictionary<CharacterStat, int> StatBuffs { get; } = [];
}
