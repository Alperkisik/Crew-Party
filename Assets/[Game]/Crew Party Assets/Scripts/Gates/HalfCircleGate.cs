using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HalfCircleGate : MonoBehaviour
{
    [TitleGroup("Left Gate")]
    [EnumToggleButtons] [SerializeField] private GateType leftGateType;

    [HorizontalGroup("Left Gate/Split")]
    [TabGroup("Left Gate/Split/Parameters", "Numerical Value")]
    [SerializeField] private int leftGateValue;

    [TabGroup("Left Gate/Split/Parameters", "Weapon")]
    [SerializeField] private WeaponType leftGateWeapon;

    [TitleGroup("Right Gate")]
    [EnumToggleButtons] [SerializeField] private GateType rightGateType;

    [HorizontalGroup("Right Gate/Split")]
    [TabGroup("Right Gate/Split/Parameters", "Numerical Value")]
    [SerializeField] private int rightGateValue;

    [TabGroup("Right Gate/Split/Parameters", "Weapon")]
    [SerializeField] private WeaponType rightGateWeapon;

    [Header("Background Colors")]
    [SerializeField] private Material positiveValueColor;
    [SerializeField] private Material negativeValueColor;
    [SerializeField] private Material weaponColor;

    [Button]
    private void GenerateGates()
    {
        GenerateLeftGate();

        GenerateRightGate();
    }

    private void Start()
    {
        Transform leftGate = transform.Find("Gate Left");
        leftGate.GetComponent<HalfCircleGateTrigger>().Setup(leftGateType, leftGateWeapon, leftGateValue);

        Transform rightGate = transform.Find("Gate Right");
        rightGate.GetComponent<HalfCircleGateTrigger>().Setup(rightGateType, rightGateWeapon, rightGateValue);
    }

    private void GenerateLeftGate()
    {
        Transform leftGate = transform.Find("Gate Left");
        Transform ui = leftGate.transform.Find("UI");
        Transform weaponsUI = ui.Find("Weapons");
        MeshRenderer meshRenderer = leftGate.GetComponent<MeshRenderer>();
        TextMeshProUGUI gateText = ui.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();

        switch (leftGateType)
        {
            case GateType.Numerical:

                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                gateText.gameObject.SetActive(true);
                if (leftGateValue >= 0)
                {
                    gateText.text = "+" + leftGateValue.ToString();
                    meshRenderer.material = positiveValueColor;
                }
                else
                {
                    gateText.text = leftGateValue.ToString();
                    meshRenderer.material = negativeValueColor;
                }

                break;

            case GateType.Weapon:

                gateText.gameObject.SetActive(false);
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                switch (leftGateWeapon)
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

    private void GenerateRightGate()
    {
        Transform rightGate = transform.Find("Gate Right");
        Transform ui = rightGate.transform.Find("UI");
        Transform weaponsUI = ui.Find("Weapons");
        MeshRenderer meshRenderer = rightGate.GetComponent<MeshRenderer>();
        TextMeshProUGUI gateText = ui.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();

        switch (rightGateType)
        {
            case GateType.Numerical:

                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                gateText.gameObject.SetActive(true);
                if (rightGateValue >= 0)
                {
                    gateText.text = "+" + rightGateValue.ToString();
                    meshRenderer.material = positiveValueColor;
                }
                else
                {
                    gateText.text = rightGateValue.ToString();
                    meshRenderer.material = negativeValueColor;
                }

                break;

            case GateType.Weapon:

                gateText.gameObject.SetActive(false);
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                switch (rightGateWeapon)
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
