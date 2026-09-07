using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public sealed class PlayerCharacter : CoreCharacter
{
    public PlayerCharacter(
        string id,
        string characterName,
        CharacterClass characterClass,
        CharacterRace characterRace,
        int[]? baseStats = null)
        : base(id, characterName, 1, characterClass, characterRace, baseStats)
    {
    }

    public PlayerCharacter(
        string id,
        string characterName,
        int level,
        CharacterClass characterClass,
        CharacterRace characterRace,
        int[]? baseStats = null)
        : base(id, characterName, level, characterClass, characterRace, baseStats)
    {
    }
}
