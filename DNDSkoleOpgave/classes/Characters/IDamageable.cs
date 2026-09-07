namespace DNDSkoleOpgave.Characters;

public interface IDamageable
{
    int CurrentHealth { get; }
    int MaximumHealth { get; }

    void TakeDamage(int amount);
    void Heal(int amount);
    void ResetHealth();
}
