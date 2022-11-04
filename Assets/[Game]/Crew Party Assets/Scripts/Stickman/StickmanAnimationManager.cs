using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class StickmanAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    bool isRunStarted = true;
    bool testScene = false;

    private void Start()
    {
        LevelControl.instance.OnPlayerWin += Stickman_OnPlayerWin;
        LevelControl.instance.OnEnemyWin += Stickman_OnEnemyWin;
        LevelControl.instance.OnLevelEnd += StickmanAnimationManager_OnLevelEnd;
    }

    private void Stickman_OnEnemyWin(object sender, System.EventArgs e)
    {
        if (animator != null)
        {
            //AnimateRun(false);
            AnimateVictory();
        }
    }

    private void Stickman_OnPlayerWin(object sender, System.EventArgs e)
    {
        if (animator != null)
        {
            //AnimateRun(false);
            AnimateVictory();
        }
    }

    private void StickmanAnimationManager_OnLevelEnd(object sender, System.EventArgs e)
    {
        if (animator != null)
        {
            //AnimateRun(false);
            AnimateVictory();
        }
    }

    private void Update()
    {
        if (!testScene)
        {
            if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted) return;

            if (isRunStarted && transform.tag == "Stickman-Ally")
            {
                //AnimateRun(true);
                AnimateRunning();
                isRunStarted = false;
            }
        }
    }

    public void AnimateRunning()
    {
        animator.SetTrigger("Running");
    }

    public void AnimateRun(bool value)
    {
        animator.SetBool("Run", value);
    }

    public void AnimateMelee()
    {
        animator.SetTrigger("Melee");
    }

    public void AnimateFire(float firingSpeed)
    {
        animator.speed = 1f + firingSpeed;
        animator.SetTrigger("Fire");
    }

    public void AnimateIdle()
    {
        AnimateRun(false);
        animator.SetTrigger("Idle");
    }

    public void AnimateVictory()
    {
        AnimateRun(false);
        animator.SetTrigger("Victory");
    }

    public void Firing(bool value, float firingSpeed)
    {
        animator.speed = 1f + firingSpeed;
        animator.SetBool("Firing", value);

        if (value == false) DefaultAnimationSpeed();
    }

    public void IncreaseAnimationSpeed(float value)
    {
        animator.speed = 1f + value;
    }

    public void DecreaseAnimationSpeed(float value)
    {
        if (animator.speed == 1f) return;
        animator.speed = 1f- value;
    }

    public void DefaultAnimationSpeed()
    {
        animator.speed = 1f;
    }
}
