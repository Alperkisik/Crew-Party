using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody physics;
    [SerializeField] private float movementSpeed;
    [HideInInspector] public bool canMove;
    Vector3 direction = Vector3.forward;

    private void Start()
    {
        canMove = true;
        LevelControl.instance.OnCombatStarted += Movement_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += Movement_OnCombatFinish;
        LevelControl.instance.OnFinishLevelStarted += Movement_OnFinishLevelStarted;
        LevelControl.instance.OnLevelEnd += Movement_OnLevelEnd;
        LevelControl.instance.OnRePositionFinished += Movement_OnRePositionFinished;
        LevelControl.instance.OnBossFightStarted += Movement_OnBossFightStarted;
    }

    private void Movement_OnBossFightStarted(object sender, System.EventArgs e)
    {
        canMove = false;
        physics.velocity = Vector3.zero;
    }

    private void Movement_OnRePositionFinished(object sender, System.EventArgs e)
    {
        canMove = true;
    }

    private void Movement_OnLevelEnd(object sender, System.EventArgs e)
    {
        canMove = false;
        physics.velocity = Vector3.zero;
    }

    private void Movement_OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        LevelControl.instance.OnRePositionFinished -= Movement_OnRePositionFinished;
        canMove = false;
        physics.velocity = Vector3.zero;
        currentPosition = transform.position;
        //direction = Vector3.up;
        //Invoke("ContinueToMove", 2f);
    }

    private void Movement_OnCombatFinish(object sender, System.EventArgs e)
    {
        //canMove = true;
    }

    private void Movement_OnCombatStarted(object sender, System.EventArgs e)
    {
        canMove = false;
        physics.velocity = Vector3.zero;
    }

    private void ContinueToMove()
    {
        GameObject levelFinisher = GameObject.Find("Level Finisher");
        direction = (levelFinisher.transform.position - transform.position).normalized;
        physics.velocity = Vector3.zero;
        transform.position = currentPosition;
        canMove = true;
    }

    Vector3 currentPosition;
    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted) return;

        if (canMove) physics.velocity = direction * movementSpeed;
        else physics.velocity = Vector3.zero;

        //if (finishMovement) physics.velocity = direction * movementSpeed;
    }
}
