using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Game;

public static class WaveRewards
{
    public static void GiveStartingSupplies(PlayerCharacter player)
    {
        for (int i = 0; i < Difficulty.StartingPotions; i++)
            TryGivePotion(player);
    }

    public static List<string> RewardVictory(PlayerCharacter player, int wave,
        IReadOnlyList<EnemyCharacter> enemies, IDiceRoller dice)
    {
        List<string> messages = new();
        RecoverSomeHealth(player, messages);
        GiveLevelIfEarned(player, wave, messages);
        CollectLoot(player, wave, enemies, dice, messages);
        player.ActiveEffects.Clear();
        return messages;
    }

    private static void RecoverSomeHealth(PlayerCharacter player, List<string> messages)
    {
        int healthBefore = player.CurrentHealth;
        player.Heal(Difficulty.RecoveryAfterWave);
        messages.Add($"Kort hvil: +{player.CurrentHealth - healthBefore} HP.");
    }

    private static void GiveLevelIfEarned(PlayerCharacter player, int wave, List<string> messages)
    {
        if (wave % Difficulty.WavesPerLevel != 0)
            return;
        player.LevelUp();
        messages.Add($"Level {player.Level}! Dit maksimum er nu {player.MaximumHealth} HP. Level-up giver ingen healing.");
    }

    private static void CollectLoot(PlayerCharacter player, int wave,
        IReadOnlyList<EnemyCharacter> enemies, IDiceRoller dice, List<string> messages)
    {
        bool potionReceived = false;
        foreach (EnemyCharacter enemy in enemies)
        {
            foreach (CoreItem item in enemy.RollLoot(dice))
            {
                if (item is ConsumableItem potion && potion.IsHealing)
                {
                    if (potionReceived || CountPotions(player) >= Difficulty.MaximumPotions)
                        continue;
                    potionReceived = true;
                }
                player.AddItem(item);
                messages.Add($"Loot: {item.Name}.");
            }
        }

        if (!potionReceived && wave % Difficulty.PotionEveryWaves == 0 && TryGivePotion(player))
            messages.Add("Du finder en Small Potion (+6 HP). Den koster 1 AP at bruge.");
    }

    public static bool TryGivePotion(PlayerCharacter player)
    {
        if (CountPotions(player) >= Difficulty.MaximumPotions)
            return false;
        player.AddItem(ItemCreator.Create("small-potion"));
        return true;
    }

    public static int CountPotions(PlayerCharacter player)
    {
        int count = 0;
        foreach (CoreItem item in player.Inventory)
        {
            if (item is ConsumableItem potion && potion.IsHealing)
                count++;
        }
        return count;
    }
}
