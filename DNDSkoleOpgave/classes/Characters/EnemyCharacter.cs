using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public sealed class EnemyCharacter : CoreCharacter
{
    public EnemyCharacter(
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
