using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.tag == "Player") 
        {
            Debug.Log("trigger");
            GameObject player = other.gameObject;
            Gate gateManager = GetComponent<Gate>();
            if (gateManager.type.Equals(GateType.Numerical))
            {
                int value = gateManager.value;

                if (value > 0)
                {
                    player.GetComponent<PartyManager>().Growth(value);
                }
                else
                {
                    player.GetComponent<PartyManager>().DeGrowth(value);
                }
            }
            else
            {
                player.GetComponent<PartyManager>().ArmParty(gateManager.weapon);
            }
        } */
    }
}
