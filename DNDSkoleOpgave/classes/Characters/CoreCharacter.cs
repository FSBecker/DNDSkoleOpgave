using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public abstract class CoreCharacter : IDamageable
{
    protected CoreCharacter(
        string id,
        string characterName,
        int level,
        CharacterClass characterClass,
        CharacterRace characterRace,
        int[]? baseStats = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(characterName);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);

        ID = id;
        CharacterName = characterName;
        Level = level;
        CharacterClass = characterClass ?? throw new ArgumentNullException(nameof(characterClass));
        CharacterRace = characterRace ?? throw new ArgumentNullException(nameof(characterRace));
        StatArray = baseStats is null ? [10, 10, 10, 10] : [.. baseStats];

        if (StatArray.Length != Enum.GetValues<CharacterStat>().Length)
        {
            throw new ArgumentException("One value is required for each character stat.", nameof(baseStats));
        }

        ApplyStatBuffs(CharacterClass.StatBuffs);
        ApplyStatBuffs(CharacterRace.StatBuffs);
        MaximumHealth = CalculateHealth();
        CurrentHealth = MaximumHealth;
    }

    public string ID { get; }
    public string CharacterName { get; set; }
    public int Level { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaximumHealth { get; private set; }
    public int DeathRolls { get; private set; }
    public int ActionPoints { get; set; } = 1;
    public int[] StatArray { get; }
    public CharacterClass CharacterClass { get; }
    public CharacterRace CharacterRace { get; }
    public List<CoreItem> Inventory { get; } = [];
    public EquipmentSlots Equipment { get; } = new();
    public bool IsDefeated => CurrentHealth <= 0;

    public int CalculateArmorClass() =>
        10 + GetStatModifier(CharacterStat.Dexterity) + Equipment.GetTotalArmorClass();

    public int CalculateHealth() =>
        Math.Max(1, CharacterClass.BaseHealth + ((Level - 1) * CharacterClass.HealthPerLevel)
            + GetStatModifier(CharacterStat.Constitution));

    public int CalculateDamage(CombatAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return action.RollDamage() + GetStatModifier(action.PrimaryStat);
    }

    public int GetStat(CharacterStat stat) => StatArray[(int)stat];

    public int GetStatModifier(CharacterStat stat) =>
        (int)Math.Floor((GetStat(stat) - 10) / 2.0);

    public void TakeDamage(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
        if (CurrentHealth == 0)
        {
            DeathRolls++;
        }
    }

    public void Heal(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        CurrentHealth = Math.Min(MaximumHealth, CurrentHealth + amount);
    }

    public void ResetHealth()
    {
        MaximumHealth = CalculateHealth();
        CurrentHealth = MaximumHealth;
        DeathRolls = 0;
    }

    public void LevelUp()
    {
        Level++;
        MaximumHealth = CalculateHealth();
        CurrentHealth = MaximumHealth;
    }

    private void ApplyStatBuffs(IReadOnlyDictionary<CharacterStat, int> buffs)
    {
        foreach ((CharacterStat stat, int buff) in buffs)
        {
            StatArray[(int)stat] += buff;
        }
    }
}
