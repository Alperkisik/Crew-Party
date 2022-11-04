using System;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

public class LevelControl : Actor<LevelManager>
{
    public static LevelControl instance { get; private set; }

    public event EventHandler OnLevelStarted;
    public event EventHandler OnCombatStarted;
    public event EventHandler OnCombatFinish;
    public event EventHandler OnFinishLevelStarted;
    public event EventHandler OnLevelEnd;
    public event EventHandler OnRePositionFinished;
    public event EventHandler OnBossFightStarted;
    public event EventHandler OnBossFightFinished;
    public event EventHandler OnEnemyWin;
    public event EventHandler OnPlayerWin;
    public event EventHandler OnFinishRePositionFinished;

    public event EventHandler<OnStickmanRePositionDoneEventArgs> OnStickmanRePositionDone;
    public class OnStickmanRePositionDoneEventArgs : EventArgs
    {
        public Vector3 position;
        public GameObject stickman;
    }

    public event EventHandler<OnStickmanReachedBlockEventArgs> OnStickmanReachedBlock;
    public class OnStickmanReachedBlockEventArgs : EventArgs
    {
        public Vector3 position;
    }

    public event EventHandler OnFinishCameraRePositionFinished;

    public bool IsGameStarted = false;

    LevelControl()
    {
        instance = this;
    }

    private void Start()
    {
        LevelStarted();
    }

    public void FinishCameraRePositionFinished()
    {
        OnFinishCameraRePositionFinished?.Invoke(this, EventArgs.Empty);
    }

    public void FinishRePositionFinished()
    {
        OnFinishRePositionFinished?.Invoke(this, EventArgs.Empty);
    }

    public void LevelStarted()
    {
        OnLevelStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StartCombatEvent()
    {
        OnCombatStarted?.Invoke(this, EventArgs.Empty);
    }

    public void FinishCombat()
    {
        OnCombatFinish?.Invoke(this, EventArgs.Empty);
    }

    public void BossFightFinish()
    {
        OnBossFightFinished?.Invoke(this, EventArgs.Empty);
    }

    public void LevelFailed()
    {
        LevelEnd();
        Push(ManagerEvents.FinishLevel, false);
    }

    public void LevelComplited()
    {
        LevelEnd();
        Push(ManagerEvents.FinishLevel, true);
    }

    public void StartFinishlevel()
    {
        CrewManager.gameFinished = true;
        OnFinishLevelStarted?.Invoke(this, EventArgs.Empty);
    }

    private void LevelEnd()
    {
        OnLevelEnd?.Invoke(this, EventArgs.Empty);
    }

    public void RePositionFinish()
    {
        OnRePositionFinished?.Invoke(this, EventArgs.Empty);
    }

    public void StickmanRePositionDone(Vector3 position,GameObject stickman)
    {
        OnStickmanRePositionDone?.Invoke(this, new OnStickmanRePositionDoneEventArgs { position = position , stickman = stickman});
    }

    public void StickmanReachedBlock(Vector3 position)
    {
        OnStickmanReachedBlock?.Invoke(this, new OnStickmanReachedBlockEventArgs { position = position });
    }

    public void StartBossFight()
    {
        OnBossFightStarted?.Invoke(this, EventArgs.Empty);
    }

    public void TriggerEnemyWinEvent()
    {
        OnEnemyWin?.Invoke(this, EventArgs.Empty);
    }

    public void TriggerPlayerWinEvent()
    {
        OnPlayerWin?.Invoke(this, EventArgs.Empty);
        LevelComplited();
    }
}