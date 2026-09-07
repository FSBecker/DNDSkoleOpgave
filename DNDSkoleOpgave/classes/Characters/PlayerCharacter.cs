using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public sealed class PlayerCharacter : CoreCharacter
{
    public int NextWave { get; set; } = 1;

    public PlayerCharacter(
        string id,
        string characterName,
        CharacterClass characterClass,
        CharacterRace characterRace,
        int[]? baseStats = null)
        : base(id, characterName, 1, characterClass, characterRace, baseStats)
    {
        ActionPoints = Game.Difficulty.PlayerActionPoints;
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
        ActionPoints = Game.Difficulty.PlayerActionPoints;
    }
}
