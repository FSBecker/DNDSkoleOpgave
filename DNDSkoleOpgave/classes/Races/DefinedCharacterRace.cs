using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Races;

public sealed class DefinedCharacterRace : CharacterRace
{
    public DefinedCharacterRace(RaceDefinition definition) : base(definition.Name, definition.Description)
    {
        foreach (var buff in definition.StatBuffs) StatBuffs[buff.Key] = buff.Value;
    }
}
