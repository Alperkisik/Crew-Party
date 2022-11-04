using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStarter : MonoBehaviour
{
    [SerializeField] CombatManager combatManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player-Party" || other.gameObject.tag == "Player")
        {
            //combatManager.StartCombat(other.gameObject);
        }
    }
}
