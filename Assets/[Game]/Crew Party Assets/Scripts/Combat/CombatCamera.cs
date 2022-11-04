using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CombatCamera : MonoBehaviour
{
    void Start()
    {
        Invoke("Subscribe", 1f);
    }

    void Subscribe()
    {
        LevelControl.instance.OnCombatStarted += CombatCamera_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += CombatCamera_OnCombatFinish;
        LevelControl.instance.OnLevelEnd += CombatCamera_OnLevelEnd;
        LevelControl.instance.OnFinishLevelStarted += _OnFinishLevelStarted;
        LevelControl.instance.OnLevelStarted += _OnLevelStarted;
        LevelControl.instance.OnBossFightStarted += _OnBossFightStarted;
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
    }

    private void _OnBossFightFinished(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 10;
        else if (transform.tag == "BossFightCamera") cinemachineVirtualCamera.Priority = 1;
    }

    private void _OnBossFightStarted(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 1;
        else if (transform.tag == "BossFightCamera") cinemachineVirtualCamera.Priority = 10;
    }

    private void _OnLevelStarted(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 10;
        else cinemachineVirtualCamera.Priority = 1;

        Invoke("Subscribe", 1f);
    }

    private void _OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 1;
        else if (transform.tag == "LevelFinishCamera") cinemachineVirtualCamera.Priority = 10;
    }

    private void CombatCamera_OnLevelEnd(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 10;
        else cinemachineVirtualCamera.Priority = 1;
    }

    private void CombatCamera_OnCombatFinish(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 10;
        else if (transform.tag == "CombatCamera") cinemachineVirtualCamera.Priority = 1;
    }

    private void CombatCamera_OnCombatStarted(object sender, System.EventArgs e)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (transform.tag == "PlayerCamera") cinemachineVirtualCamera.Priority = 1;
        else if (transform.tag == "CombatCamera") cinemachineVirtualCamera.Priority = 10;
    }
}
