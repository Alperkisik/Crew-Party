using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator.speed = 1.25f;

        LevelControl.instance.OnEnemyWin += Stickman_OnEnemyWin;

        LevelControl.instance.OnBossFightStarted += _OnBossFightStarted;
        gameObject.GetComponent<BossManager>().OnAttack += _OnAttack;
    }

    private void _OnAttack(object sender, System.EventArgs e)
    {
        AnimateAttack();
    }

    private void _OnBossFightStarted(object sender, System.EventArgs e)
    {
        AnimateRunning();
        //AnimateRun(true);
    }

    private void Stickman_OnEnemyWin(object sender, System.EventArgs e)
    {
        Debug.Log("Animate Victory");
        AnimateVictory();
    }

    public void AnimateAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void AnimateRunning()
    {
        animator.SetTrigger("Running");
    }

    public void AnimateRun(bool value)
    {
        animator.SetBool("Run", value);
    }

    public void AnimateIdle()
    {
        AnimateRun(false);
        animator.SetTrigger("Idle");
    }

    public void AnimateVictory()
    {
        AnimateRun(false);
        animator.SetTrigger("Win");
    }
}
