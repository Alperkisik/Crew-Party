using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Player-Party")
        {
            collider.enabled = false;
            LevelControl.instance.StartBossFight();
        }
    }
}
