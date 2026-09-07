using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatInstance
{
    public CombatInstance(PlayerCharacter player, IReadOnlyList<EnemyCharacter> enemies, IDiceRoller dice)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        ArgumentNullException.ThrowIfNull(enemies);
        if (enemies.Count < 1 || enemies.Count > Difficulty.MaximumEnemies)
            throw new ArgumentException("En kamp skal have 1–3 fjender.", nameof(enemies));
        if (enemies.Any(enemy => enemy is null) || enemies.Distinct().Count() != enemies.Count)
            throw new ArgumentException("Hver fjende skal være et separat objekt.", nameof(enemies));

        Enemies = enemies.ToList().AsReadOnly();
        TurnOrder = CreateTurnOrder(dice).AsReadOnly();
    }

    public PlayerCharacter Player { get; }
    public IReadOnlyList<EnemyCharacter> Enemies { get; }
    public IReadOnlyList<CombatParticipant> TurnOrder { get; }
    public int Round { get; private set; } = 1;

    public void StartNextRound() => Round++;

    private List<CombatParticipant> CreateTurnOrder(IDiceRoller dice)
    {
        List<CoreCharacter> characters = new() { Player };
        characters.AddRange(Enemies);
        List<CombatParticipant> participants = new();

        foreach (CoreCharacter character in characters)
        {
            int initiative = dice.Roll(20) + character.GetStatModifier(CharacterStat.Dexterity);
            participants.Add(new CombatParticipant(character, initiative));
        }

        return participants.OrderByDescending(participant => participant.Initiative).ToList();
    }

    public CombatState CheckEndState()
    {
        if (Player.IsDefeated)
        {
            return CombatState.EnemyWon;
        }

        return Enemies.All(enemy => enemy.IsDefeated) ? CombatState.PlayerWon : CombatState.Ongoing;
    }
}
