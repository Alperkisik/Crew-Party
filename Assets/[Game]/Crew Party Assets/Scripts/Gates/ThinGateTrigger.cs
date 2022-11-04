using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class ThinGateTrigger : MonoBehaviour
{
    GateType gateType;
    WeaponType gateWeapon;
    int gateValue;

    int triggerCount = 0;
    List<GameObject> parties;
    bool triggered = false;

    private void Start()
    {
        parties = new List<GameObject>();
    }

    public void Setup(GateType type, WeaponType weaponType, int value)
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
            parties.Add(other.gameObject);
            //VibrationManager.Instance.TriggerHeavyImpact();
            if (triggered == false)
            {
                triggered = true;
                Invoke("TriggerGate", 0.1f);
            }

            /*if (gateType == GateType.Numerical)
            {
                if (gateValue > 0) player.GetComponent<PartyManager>().Growth(gateValue);
                else player.GetComponent<PartyManager>().DeGrowth(gateValue);
            }
            else
            {
                player.GetComponent<PartyManager>().ArmParty(gateWeapon);
            }*/
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

    private void TriggerGate()
    {
        if (parties.Count == 1)
        {
            if (gateType == GateType.Numerical)
            {
                if (gateValue > 0) parties[0].GetComponent<PartyManager>().Growth(gateValue);
                else parties[0].GetComponent<PartyManager>().DeGrowth(gateValue);
            }
            else
            {
                parties[0].GetComponent<PartyManager>().ArmParty(gateWeapon);
            }
        }
        else
        {
            int addValue = gateValue / 2;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    if (gateValue % 2 != 0 && (gateValue != 2 || gateValue != -2))
                    {
                        if (gateValue > 0) addValue++;
                        else addValue--;
                    }
                }

                if (gateType == GateType.Numerical)
                {
                    if (gateValue > 0) parties[i].GetComponent<PartyManager>().Growth(addValue);
                    else parties[i].GetComponent<PartyManager>().DeGrowth(addValue);
                }
                else
                {
                    parties[i].GetComponent<PartyManager>().ArmParty(gateWeapon);
                }
            }

            parties.Clear();
        }
    }
}
