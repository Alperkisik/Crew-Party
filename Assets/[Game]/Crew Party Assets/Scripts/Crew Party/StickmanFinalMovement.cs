using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class StickmanFinalMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] Rigidbody physics;
    public bool canMove;
    public bool IsInFinalLevel = false;

    private void Start()
    {
        //LevelControl.instance.OnFinishRePositionFinished += _OnFinishRePositionFinished;
        LevelControl.instance.OnFinishCameraRePositionFinished += _OnFinishCameraRePositionFinished;
    }

    private void _OnFinishCameraRePositionFinished(object sender, System.EventArgs e)
    {
        if (this == null) return;

        if (GetComponent<StickmanManager>().IsStickmanAlive()) StartMove();
    }

    private void _OnFinishRePositionFinished(object sender, System.EventArgs e)
    {
        if (this == null) return;

        if (GetComponent<StickmanManager>().IsStickmanAlive()) StartMove();
    }

    private void StartMove()
    {
        physics.isKinematic = false;
        physics.useGravity = false;
        transform.SetParent(transform.root);
        canMove = true;
        IsInFinalLevel = true;
    }

    private void FixedUpdate()
    {

        if (IsInFinalLevel)
        {
            //if (canMove) physics.velocity = Vector3.forward * movementSpeed;

            if (canMove)
            {
                Vector3 targetPosition = transform.position + Vector3.forward;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 16)
        {
            canMove = false;
            physics.velocity = Vector3.zero;
            VibrationManager.Instance.TriggerLightImpact();
            GetComponent<StickmanAnimationManager>().AnimateIdle();
            LevelControl.instance.StickmanReachedBlock(transform.position);
        }
    }
}
