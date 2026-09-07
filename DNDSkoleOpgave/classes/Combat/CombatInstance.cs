using DNDSkoleOpgave.Characters;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatInstance
{
    public CombatInstance(PlayerCharacter player, EnemyCharacter enemy)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
    }

    public PlayerCharacter Player { get; }
    public EnemyCharacter Enemy { get; }

    public CombatState CheckEndState()
    {
        if (Player.IsDefeated)
        {
            return CombatState.EnemyWon;
        }

        return Enemy.IsDefeated ? CombatState.PlayerWon : CombatState.Ongoing;
    }
}
