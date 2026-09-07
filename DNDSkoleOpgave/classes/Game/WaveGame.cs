using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.UI;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Game;

public sealed class WaveGame
{
    private readonly PlayerCharacter _player;
    private readonly CharacterJsonRepository _saves;
    private readonly WaveCreator _waves;
    private readonly IDiceRoller _dice;

    public WaveGame(PlayerCharacter player, CharacterJsonRepository saves, GameDefinitions definitions, IDiceRoller dice)
    {
        _player = player;
        _saves = saves;
        _waves = new WaveCreator(definitions, dice);
        _dice = dice;
    }

    public void Run()
    {
        while (!_player.IsDefeated)
        {
            List<EnemyCharacter> enemies = _waves.CreateWave(_player.NextWave);
            CombatState result = FightWave(enemies);
            if (result == CombatState.EnemyWon)
            {
                SaveDefeat();
                return;
            }

            RewardVictory(enemies);
            SaveProgress();
            if (!ChooseToContinue())
                return;
        }
    }

    private CombatState FightWave(List<EnemyCharacter> enemies)
    {
        CombatInstance battle = new(_player, enemies, _dice);
        CombatRunner combat = new(battle, _dice);
        return combat.Run();
    }

    private void RewardVictory(List<EnemyCharacter> enemies)
    {
        GameMenu.Pause("Sejr! Tryk Enter for at se belønningen");
        GameMenu.ShowTitle($"Wave {_player.NextWave} gennemført");
        List<string> rewards = WaveRewards.RewardVictory(_player, _player.NextWave, enemies, _dice);
        foreach (string reward in rewards)
            GameMenu.ShowMessage(reward);
        _player.NextWave++;
        CharacterSheet.Show(_player);
    }

    private void SaveProgress() => _saves.Save(_player);

    private bool ChooseToContinue()
    {
        while (true)
        {
            string choice = GameMenu.Choose("Hvad nu?", "Næste wave", "Skift udstyr", "Gem og afslut");
            if (choice == "Næste wave")
                return true;
            if (choice == "Gem og afslut")
                return false;
            EquipmentMenu.Run(_player);
            SaveProgress();
        }
    }

    private void SaveDefeat()
    {
        _player.ActiveEffects.Clear();
        SaveProgress();
        GameMenu.ShowMessage($"Du nåede til wave {_player.NextWave}. Karakteren er besejret.");
        GameMenu.Pause();
    }
}
