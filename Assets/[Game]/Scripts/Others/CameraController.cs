using Cinemachine;
using Game.GlobalVariables;
using System;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

public class CameraController : Actor<LevelManager>
{
    [Header("General Variables")]
    public bool usingPathFollower = false;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera _CinemachineVirtualCamera;
    Transform playerAimPoint;

    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            LevelManager.Instance.Subscribe(CustomManagerEvents.SendCameraTarget, FetchCameraTarget);
        }
        else
        {
            LevelManager.Instance.Unsubscribe(CustomManagerEvents.SendCameraTarget, FetchCameraTarget);
        }
    }

    private void FetchCameraTarget(object[] args)
    {
        Transform target = (Transform)args[0];
        playerAimPoint = (Transform)args[1];

        _CinemachineVirtualCamera.LookAt = playerAimPoint;
        _CinemachineVirtualCamera.Follow = target;

    } // FetchCameraTarget()

} // class
