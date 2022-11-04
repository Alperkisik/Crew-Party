using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using TriflesGames.Managers;
using System.Collections.Generic;

public class Gate : MonoBehaviour
{
    [TitleGroup("Gate Type")]
    [EnumToggleButtons] [SerializeField] private GateType type;

    [HorizontalGroup("Gate Type/Split")]
    [TabGroup("Gate Type/Split/Parameters", "Numerical Value")]
    [SerializeField] private int value;

    [TabGroup("Gate Type/Split/Parameters", "Weapon")]
    [SerializeField] private WeaponType weapon;

    [Header("Background Colors")]
    [SerializeField] private Material positiveValueColor;
    [SerializeField] private Material negativeValueColor;
    [SerializeField] private Material weaponColor;

    int triggerCount = 0;

    List<GameObject> parties;
    bool triggered = false;
    private void Start()
    {
        parties = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player-Party")
        {
            //GameObject player = other.gameObject;
            parties.Add(other.gameObject);
            //VibrationManager.Instance.TriggerHeavyImpact();
            if(triggered == false)
            {
                triggered = true;
                Invoke("TriggerGate", 0.1f);
            }
            /*if (type == GateType.Numerical)
            {
                if (value > 0) player.GetComponent<PartyManager>().Growth(value);
                else player.GetComponent<PartyManager>().DeGrowth(value);
            }
            else
            {
                player.GetComponent<PartyManager>().ArmParty(weapon);
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
        if(parties.Count == 1)
        {
            if (type == GateType.Numerical)
            {
                if (value > 0) parties[0].GetComponent<PartyManager>().Growth(value);
                else parties[0].GetComponent<PartyManager>().DeGrowth(value);
            }
            else
            {
                parties[0].GetComponent<PartyManager>().ArmParty(weapon);
            }
        }
        else
        {
            int addValue = value / 2;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    if (value % 2 != 0 && (value != 2 || value != -2))
                    {
                        if (value > 0) addValue++;
                        else addValue--;
                    }
                }

                if (type == GateType.Numerical)
                {
                    if (value > 0) parties[i].GetComponent<PartyManager>().Growth(addValue);
                    else parties[i].GetComponent<PartyManager>().DeGrowth(addValue);
                }
                else
                {
                    parties[i].GetComponent<PartyManager>().ArmParty(weapon);
                }
            }

            parties.Clear();
        }
        
    }

    [Button]
    private void GenerateGate()
    {
        Transform ui = transform.Find("UI");
        Transform weaponsUI = ui.Find("Weapons");
        MeshRenderer meshRenderer = transform.Find("Transparent").GetComponent<MeshRenderer>();
        TextMeshProUGUI gateText = ui.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();

        switch (type)
        {
            case GateType.Numerical:

                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                gateText.gameObject.SetActive(true);
                if (value >= 0)
                {
                    gateText.text = "+" + value.ToString();
                    meshRenderer.material = positiveValueColor;
                }
                else
                {
                    gateText.text = value.ToString();
                    meshRenderer.material = negativeValueColor;
                }

                break;

            case GateType.Weapon:

                gateText.gameObject.SetActive(false);
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                switch (weapon)
                {
                    case WeaponType.Pillow:
                        weaponsUI.Find("Pillow").gameObject.SetActive(true);
                        break;
                    case WeaponType.Chair:
                        weaponsUI.Find("Chair").gameObject.SetActive(true);
                        break;
                    case WeaponType.Bat:
                        weaponsUI.Find("Bat").gameObject.SetActive(true);
                        break;
                    case WeaponType.Rifle:
                        weaponsUI.Find("Rifle").gameObject.SetActive(true);
                        break;
                    case WeaponType.Shotgun:
                        weaponsUI.Find("Shotgun").gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }

                meshRenderer.material = weaponColor;

                break;

            default:
                break;
        }
    }
}