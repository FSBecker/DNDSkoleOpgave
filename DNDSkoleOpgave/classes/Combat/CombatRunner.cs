using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Game;
using DNDSkoleOpgave.Exceptions;
using DNDSkoleOpgave.UI;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatRunner
{
    private readonly CombatInstance _battle;
    private readonly IDiceRoller _dice;
    private readonly CombatActionExecutor _executor;

    public CombatRunner(CombatInstance battle, IDiceRoller dice)
    {
        _battle = battle;
        _dice = dice;
        _executor = new CombatActionExecutor(dice);
    }

    public CombatState Run()
    {
        ShowBattleStart();
        while (_battle.CheckEndState() == CombatState.Ongoing)
        {
            PlayRound();
            if (_battle.CheckEndState() == CombatState.Ongoing)
                _battle.StartNextRound();
        }
        ShowBattleResult();
        return _battle.CheckEndState();
    }

    private void PlayRound()
    {
        foreach (CombatParticipant participant in _battle.TurnOrder)
        {
            if (!participant.Character.IsDefeated)
                PlayTurn(participant);
            if (_battle.CheckEndState() != CombatState.Ongoing)
                return;
        }
    }

    private void PlayTurn(CombatParticipant participant)
    {
        participant.StartTurn();
        ProcessActiveEffects(participant);

        while (participant.CanAct && _battle.CheckEndState() == CombatState.Ongoing)
        {
            BattleHud.Render(_battle, participant);
            PauseForEnemy(participant);
            CombatChoice? choice = ChooseAction(participant);
            if (choice is null)
            {
                CombatLog.Write($"{participant.Character.CharacterName} afslutter sin tur.");
                return;
            }

            CoreCharacter? target = ChooseTarget(participant, choice);
            if (target is null)
                continue;
            if (!ExecuteAction(participant, choice, target))
                return;
        }
    }

    private CombatChoice? ChooseAction(CombatParticipant participant)
    {
        if (participant.Character is PlayerCharacter)
            return CombatMenu.ChooseAction(participant);
        return EnemyDecision.ChooseAction(participant, _dice);
    }

    private CoreCharacter? ChooseTarget(CombatParticipant participant, CombatChoice choice)
    {
        if (!choice.NeedsEnemyTarget)
            return participant.Character;
        if (participant.Character is EnemyCharacter)
            return _battle.Player;
        return CombatMenu.ChooseTarget(_battle);
    }

    private bool ExecuteAction(CombatParticipant participant, CombatChoice choice, CoreCharacter target)
    {
        try
        {
            CombatLog.Write(_executor.Execute(participant, choice, target));
            BattleHud.Render(_battle, participant, target);
            PauseForEnemy(participant);
            return true;
        }
        catch (CharacterIsDefeatedException exception)
        {
            ShowActionError(exception, participant);
            return false;
        }
        catch (InsufficientActionPointsException exception)
        {
            ShowActionError(exception, participant);
            return false;
        }
        catch (InvalidOperationException exception)
        {
            ShowActionError(exception, participant);
            return false;
        }
    }

    private void ShowActionError(Exception exception, CombatParticipant participant)
    {
        CombatLog.Write(exception.Message);
        BattleHud.Render(_battle, participant);
        GameMenu.Pause();
    }

    private void ProcessActiveEffects(CombatParticipant participant)
    {
        CoreCharacter character = participant.Character;
        if (character.ActiveEffects.Count == 0)
            return;
        int healthBefore = character.CurrentHealth;
        character.ProcessActiveEffects();
        CombatLog.Write($"{character.CharacterName}: aktive effekter ændrer HP fra {healthBefore} til {character.CurrentHealth}.");
        if (character.IsDefeated)
            CombatLog.Write($"{character.CharacterName} er besejret af en effekt.");
        BattleHud.Render(_battle, participant);
        Thread.Sleep(Difficulty.EnemyTurnDelay);
    }

    private static void PauseForEnemy(CombatParticipant participant)
    {
        if (participant.Character is EnemyCharacter)
            Thread.Sleep(Difficulty.EnemyTurnDelay);
    }

    private void ShowBattleStart()
    {
        CombatLog.Clear();
        CombatLog.Write("Initiativ = d20 + Dexterity. Rækkefølgen gælder hele kampen.");
        BattleHud.Render(_battle);
        GameMenu.Pause("Tryk Enter for at starte kampen");
    }

    private void ShowBattleResult()
    {
        string result = _battle.CheckEndState() == CombatState.PlayerWon ? "Sejr!" : "Game over!";
        CombatLog.Write(result);
        BattleHud.Render(_battle);
    }
}
