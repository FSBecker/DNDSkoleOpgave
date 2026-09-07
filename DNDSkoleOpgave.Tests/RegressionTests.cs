using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.Exceptions;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Tests;

public static class RegressionTests
{
    private static int _checks;

    public static void Run()
    {
        GameDefinitions definitions = GameDefinitionsCreator.Create();
        GameCatalog.Load(definitions);
        TestHealthAndLevels();
        TestWavesAndInitiative(definitions);
        TestHealingBudget();
        TestPotionsAndRewards(definitions);
        TestMissAndDefeat(definitions);
        TestOngoingEffects();
        TestPersistence(definitions);
        Console.WriteLine($"Alle {_checks} kontroller bestået.");
    }

    private static PlayerCharacter CreatePlayer(string characterClass = "Wizard") =>
        CharacterCreator.Create("Test [helt]", characterClass, "Human");

    private static void TestHealthAndLevels()
    {
        PlayerCharacter player = CreatePlayer();
        player.TakeDamage(8);
        int healthBefore = player.CurrentHealth;
        int maximumBefore = player.MaximumHealth;
        player.LevelUp();
        Check(player.CurrentHealth == healthBefore, "Level-up giver ingen healing");
        Check(player.MaximumHealth > maximumBefore, "Level-up øger maksimum");
        player.Heal(int.MaxValue);
        Check(player.CurrentHealth == player.MaximumHealth, "Heal er begrænset uden integer overflow");
        player.TakeDamage(int.MaxValue);
        Check(player.CurrentHealth == 0, "HP stopper ved nul");
        ExpectException<ArgumentOutOfRangeException>(() => player.TakeDamage(-1));
        ExpectException<ArgumentOutOfRangeException>(() => player.Heal(-1));
    }

    private static void TestWavesAndInitiative(GameDefinitions definitions)
    {
        for (int count = 1; count <= 3; count++)
        {
            WaveCreator creator = new(definitions, new FixedDiceRoller(count));
            foreach (int wave in new[] { 1, 2, 4, 100 })
            {
                List<EnemyCharacter> enemies = creator.CreateWave(wave);
                Check(enemies.Count == count, "Wave kan have 1, 2 eller 3 fjender");
                Check(enemies.Select(enemy => enemy.ID).Distinct().Count() == count, "Unikke fjender i samme wave");
                Check(enemies[0].Level == 1 + (wave - 1) / Difficulty.WavesPerLevel, "Fjender skalerer også efter wave 3");
            }
        }

        PlayerCharacter player = CreatePlayer();
        EnemyCharacter fast = new("fast", "Hurtig", 1, new Barbarian(), new Human(), [10, 18, 10, 10]);
        EnemyCharacter slow = new("slow", "Langsom", 1, new Barbarian(), new Human(), [10, 8, 10, 10]);
        CombatInstance battle = new(player, new[] { slow, fast }, new FixedDiceRoller(10));
        Check(battle.TurnOrder[0].Character == fast, "En fjende med højt initiativ kan starte");
        Check(battle.TurnOrder[^1].Character == slow, "Laveste initiativ slutter");
        Check(battle.TurnOrder.Select(value => value.Initiative).SequenceEqual(
            battle.TurnOrder.Select(value => value.Initiative).OrderDescending()), "Turorden er faldende");
        ExpectException<ArgumentException>(() => new CombatInstance(player, Array.Empty<EnemyCharacter>(), new FixedDiceRoller(1)));
        ExpectException<ArgumentException>(() => new CombatInstance(player, new[] { fast, fast }, new FixedDiceRoller(1)));
        ExpectException<ArgumentOutOfRangeException>(() => new WaveCreator(definitions, new FixedDiceRoller(1)).CreateWave(0));
    }

    private static void TestHealingBudget()
    {
        PlayerCharacter player = CreatePlayer();
        player.TakeDamage(10);
        CombatParticipant participant = new(player, 10);
        participant.StartTurn();
        CombatChoice healing = CombatChoices.GetAvailable(participant).First(choice => choice.Name == "Healing Hand");
        CombatActionExecutor executor = new(new FixedDiceRoller(4));
        executor.Execute(participant, healing, player);
        Check(participant.HealingAbilitiesLeft == 0, "Healing bruger kampens ene healing-evne");
        int health = player.CurrentHealth;
        int ap = participant.RemainingActionPoints;
        ExpectException<InvalidOperationException>(() => executor.Execute(participant, healing, player));
        Check(player.CurrentHealth == health && participant.RemainingActionPoints == ap, "Afvist healing ændrer ingen ressourcer");
        participant.StartTurn();
        Check(!participant.CanUse(healing), "En ny tur giver ikke ny healing");
        CombatParticipant nextBattle = new(player, 10);
        nextBattle.StartTurn();
        Check(nextBattle.CanUse(healing), "En ny kamp giver én healing-evne igen");
        Check(player.CurrentHealth == health, "En ny kamp healer ikke");
    }

    private static void TestPotionsAndRewards(GameDefinitions definitions)
    {
        PlayerCharacter player = CreatePlayer();
        WaveRewards.GiveStartingSupplies(player);
        Check(WaveRewards.CountPotions(player) == 1, "Én startpotion");
        Check(WaveRewards.TryGivePotion(player), "Plads til potion nummer to");
        Check(!WaveRewards.TryGivePotion(player), "Ingen potion nummer tre");

        player.TakeDamage(10);
        CombatParticipant participant = new(player, 1);
        participant.StartTurn();
        CombatChoice potion = CombatChoices.GetAvailable(participant).First(choice => choice.Item is not null);
        CombatActionExecutor executor = new(new FixedDiceRoller(1));
        executor.Execute(participant, potion, player);
        Check(WaveRewards.CountPotions(player) == 1, "Præcis én potion forbruges");
        Check(participant.RemainingActionPoints == 1, "Potion koster 1 AP");
        Check(participant.HealingAbilitiesLeft == 1, "Potion bruger ikke healing-evnen");
        ExpectException<InvalidOperationException>(() => executor.Execute(participant, potion, player));

        PlayerCharacter resting = CreatePlayer();
        resting.TakeDamage(10);
        int health = resting.CurrentHealth;
        List<EnemyCharacter> enemies = new WaveCreator(definitions, new FixedDiceRoller(1)).CreateWave(1);
        WaveRewards.RewardVictory(resting, 1, enemies, new FixedDiceRoller(100));
        Check(resting.CurrentHealth == health + 2, "Hvil giver kun 2 HP");
        Check(WaveRewards.CountPotions(resting) == 0, "Ingen automatisk potion efter wave 1");
        WaveRewards.RewardVictory(resting, 3, enemies, new FixedDiceRoller(100));
        Check(WaveRewards.CountPotions(resting) == 1, "Potion efter hver tredje wave");
    }

    private static void TestMissAndDefeat(GameDefinitions definitions)
    {
        PlayerCharacter player = CreatePlayer("Barbarian");
        List<EnemyCharacter> enemies = new WaveCreator(definitions, new FixedDiceRoller(3)).CreateWave(1);
        CombatInstance battle = new(player, enemies, new FixedDiceRoller(10));
        CombatParticipant participant = battle.TurnOrder.Single(value => value.Character == player);
        participant.StartTurn();
        int health = enemies[0].CurrentHealth;
        CombatChoice attack = CombatChoices.GetAvailable(participant).First(choice => choice.NeedsEnemyTarget);
        new CombatActionExecutor(new FixedDiceRoller(1)).Execute(participant, attack, enemies[0]);
        Check(enemies[0].CurrentHealth == health, "Et naturligt 1-tal rammer ikke");
        Check(participant.RemainingActionPoints == 1, "Et misset angreb bruger AP");
        enemies[0].TakeDamage(int.MaxValue);
        Check(battle.CheckEndState() == CombatState.Ongoing, "Kampen slutter ikke ved første besejrede fjende");
        foreach (EnemyCharacter enemy in enemies)
            enemy.TakeDamage(int.MaxValue);
        Check(battle.CheckEndState() == CombatState.PlayerWon, "Alle fjender skal besejres");
        CombatParticipant emptyTurn = new(player, 1);
        ExpectException<InsufficientActionPointsException>(() => emptyTurn.SpendResources(attack));
        player.TakeDamage(int.MaxValue);
        ExpectException<CharacterIsDefeatedException>(() => participant.SpendResources(attack));
        Check(battle.CheckEndState() == CombatState.EnemyWon, "Besejret spiller taber");
    }

    private static void TestOngoingEffects()
    {
        PlayerCharacter player = CreatePlayer();
        int health = player.CurrentHealth;
        player.AddActiveEffect(new ActionEffect(ActionEffectType.Damage, 2,
            EffectTarget.Enemy, EffectTiming.OverTime, 2));
        player.ProcessActiveEffects();
        Check(player.CurrentHealth == health - 2, "Effekt virker på første tur");
        player.ProcessActiveEffects();
        Check(player.CurrentHealth == health - 4 && player.ActiveEffects.Count == 0, "Effekt stopper efter sin varighed");
    }

    private static void TestPersistence(GameDefinitions definitions)
    {
        string folder = Path.Combine(Path.GetTempPath(), "dnd-tests-" + Guid.NewGuid().ToString("N"));
        try
        {
            GameDefinitionsJsonRepository contentRepository = new();
            string definitionsPath = Path.Combine(folder, "game-definitions.json");
            contentRepository.Save(definitionsPath, definitions);
            GameDefinitions loadedContent = contentRepository.Load(definitionsPath);
            GameCatalog.Load(loadedContent);
            Check(loadedContent.AllRaces.Count == definitions.AllRaces.Count, "Alle racer gemmes i JSON");
            Check(ItemCreator.Create("small-potion") is ConsumableItem, "Potion oprettes fra JSON");

            PlayerCharacter player = CreatePlayer();
            player.AddItem(ItemCreator.Create("crystal-staff"));
            WaveRewards.GiveStartingSupplies(player);
            player.TakeDamage(7);
            player.NextWave = 7;
            CharacterJsonRepository saves = new(Path.Combine(folder, "characters"));
            saves.Save(player);
            PlayerCharacter loaded = saves.Load(player.ID);
            Check(loaded.CurrentHealth == player.CurrentHealth, "Indlæsning giver ingen gratis healing");
            Check(loaded.NextWave == 7, "Wave gemmes");
            Check(loaded.Inventory.Count == player.Inventory.Count, "Ekstra kopi af udstyret våben beholdes i tasken");
            Check(loaded.Equipment.AllEquipment.Count == player.Equipment.AllEquipment.Count, "Udstyr gendannes");
            Check(loaded.Actions.Select(action => action.ID).SequenceEqual(player.Actions.Select(action => action.ID)), "Actions gendannes");
        }
        finally
        {
            GameCatalog.Load(definitions);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }
    }

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
            throw new InvalidOperationException("Test fejlede: " + message);
    }

    private static void ExpectException<T>(Action action) where T : Exception
    {
        _checks++;
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException("Forventede " + typeof(T).Name);
    }
}
