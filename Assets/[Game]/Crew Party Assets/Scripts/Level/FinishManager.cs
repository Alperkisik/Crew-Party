using System;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class FinishManager : MonoBehaviour
{
    [SerializeField] GameObject platformGenerator;

    void Start()
    {
        LevelControl.instance.OnFinishLevelStarted += FinishManager_OnFinishLevelStarted;
    }

    private void FinishManager_OnFinishLevelStarted(object sender, EventArgs e)
    {
        int count = GameObject.Find("Crew-Party").GetComponent<CrewManager>().totalStickmanCount;
        int generateAmount = (count / 3);
        if (count % 3 == 0) generateAmount--;

        platformGenerator.GetComponent<PlatformGeneratorTest>().Generate(generateAmount);
    }
}