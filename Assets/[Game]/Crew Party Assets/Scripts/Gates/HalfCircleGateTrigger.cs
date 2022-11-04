using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class HalfCircleGateTrigger : MonoBehaviour
{
    GateType gateType;
    WeaponType gateWeapon;
    int gateValue;

    int triggerCount = 0;
    public void Setup(GateType type,WeaponType weaponType, int value)
    {
        gateType = type;
        gateWeapon = weaponType;
        gateValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player-Party")
        {
            GameObject player = other.gameObject;
            //VibrationManager.Instance.TriggerHeavyImpact();

            if (gateType == GateType.Numerical)
            {
                if (gateValue > 0) player.GetComponent<PartyManager>().Growth(gateValue);
                else player.GetComponent<PartyManager>().DeGrowth(gateValue);
            }
            else
            {
                player.GetComponent<PartyManager>().ArmParty(gateWeapon);
            }
        }
        if (other.gameObject.tag == "Stickman-Ally")
        {
            triggerCount++;
            if (triggerCount % 2 == 0)
            {
                VibrationManager.Instance.TriggerLightImpact();
            } 
        }
    }
}
