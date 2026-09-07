using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Game;

public sealed class WaveCreator
{
    private readonly GameDefinitions _definitions;
    private readonly IDiceRoller _dice;

    public WaveCreator(GameDefinitions definitions, IDiceRoller dice)
    {
        _definitions = definitions;
        _dice = dice;
    }

    public List<EnemyCharacter> CreateWave(int wave)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(wave, 1);
        if (_definitions.AllEnemies.Count == 0)
            throw new InvalidOperationException("Der skal være mindst én fjende i game-definitions.json.");

        int enemyCount = _dice.Roll(Difficulty.MaximumEnemies);
        List<EnemyCharacter> enemies = new();

        for (int number = 1; number <= enemyCount; number++)
            enemies.Add(CreateEnemy(wave, number));

        return enemies;
    }

    private EnemyCharacter CreateEnemy(int wave, int number)
    {
        int index = _dice.Roll(_definitions.AllEnemies.Count) - 1;
        EnemyDefinition definition = _definitions.AllEnemies[index];
        int level = definition.Level + (wave - 1) / Difficulty.WavesPerLevel;
        string id = $"{definition.Id}-wave-{wave}-{number}";
        string name = $"{definition.Name} {number}";

        EnemyCharacter enemy = EnemyCreator.Create(definition, _definitions, level, id, name);
        enemy.ActionPoints = Difficulty.EnemyActionPoints;
        enemy.AttackBonus = (wave - 1) / 3;
        return enemy;
    }
}
