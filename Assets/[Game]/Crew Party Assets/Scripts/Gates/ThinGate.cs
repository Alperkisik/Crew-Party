using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ThinGate : MonoBehaviour
{
    [TitleGroup("Left Gate")]
    [EnumToggleButtons] [SerializeField] private GateType middleGateType;

    [HorizontalGroup("Left Gate/Split")]
    [TabGroup("Left Gate/Split/Parameters", "Numerical Value")]
    [SerializeField] private int middleGateValue;

    [TabGroup("Left Gate/Split/Parameters", "Weapon")]
    [SerializeField] private WeaponType middleGateWeapon;

    [TitleGroup("Right Gate")]
    [EnumToggleButtons] [SerializeField] private GateType leftRightGateType;

    [HorizontalGroup("Right Gate/Split")]
    [TabGroup("Right Gate/Split/Parameters", "Numerical Value")]
    [SerializeField] private int leftRightValue;

    [TabGroup("Right Gate/Split/Parameters", "Weapon")]
    [SerializeField] private WeaponType leftRightWeapon;

    [Header("Background Colors")]
    [SerializeField] private Material positiveValueColor;
    [SerializeField] private Material negativeValueColor;
    [SerializeField] private Material weaponColor;
    [SerializeField] private Material ironColor;

    [SerializeField] private Color redColor;
    [SerializeField] private Color greenColor;



    [Button]
    private void GenerateGates()
    {
        GenerateMiddleGate();

        //GenerateLeftRightGate();
    }

    private void Start()
    {
        Transform middleGate = transform.Find("Middle Gate");
        middleGate.GetComponent<ThinGateTrigger>().Setup(middleGateType, middleGateWeapon, middleGateValue);

        Transform leftRightGate = transform.Find("Left Right Gate");
        int leftValue,rightValue;
        leftValue = leftRightValue / 2;
        rightValue = leftRightValue - leftValue;

        leftRightGate.Find("Gate Left").GetComponent<ThinGateTrigger>().Setup(leftRightGateType, leftRightWeapon, leftValue);
        leftRightGate.Find("Gate Right").GetComponent<ThinGateTrigger>().Setup(leftRightGateType, leftRightWeapon, rightValue);

        GenerateLeftRightGate();
    }

    private void GenerateMiddleGate()
    {
        Transform middleGate = transform.Find("Middle Gate");
        Transform ui = middleGate.transform.Find("UI");
        Transform weaponsUI = ui.Find("Weapons");
        MeshRenderer meshRenderer = middleGate.GetComponent<MeshRenderer>();
        
        TextMeshProUGUI gateText = ui.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
        //meshRenderer.sharedMaterials[1] = ironColor;
        //meshRenderer.material = ironColor;
        switch (middleGateType)
        {
            case GateType.Numerical:

                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                gateText.gameObject.SetActive(true);

                if (middleGateValue >= 0)
                {
                    gateText.text = "+" + middleGateValue.ToString();
                    //meshRenderer.sharedMaterials[0] = positiveValueColor;
                    middleGate.GetComponent<Renderer>().materials[0].color = greenColor;
                }
                else
                {
                    gateText.text = middleGateValue.ToString();
                    middleGate.GetComponent<Renderer>().materials[0].color = redColor;
                    //meshRenderer.sharedMaterials[0] = negativeValueColor;
                }
                break;

            case GateType.Weapon:

                gateText.gameObject.SetActive(false);
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                switch (middleGateWeapon)
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
                //meshRenderer.sharedMaterials[0] = weaponColor;
                break;

            default:
                break;
        }
    }
    
    private void GenerateLeftRightGate()
    {
        Transform leftRightGate = transform.Find("Left Right Gate");
        Transform ui = leftRightGate.Find("UI");
        Transform weaponsUI = ui.Find("Weapons");
        MeshRenderer meshRenderer = leftRightGate.GetComponent<MeshRenderer>();
        TextMeshProUGUI gateText = ui.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
        //meshRenderer.materials[0] = ironColor;
        //meshRenderer.sharedMaterials[0] = ironColor;

        meshRenderer.materials[1] = weaponColor;
        
        switch (leftRightGateType)
        {
            case GateType.Numerical:
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                gateText.gameObject.SetActive(true);
                if (leftRightValue >= 0)
                {
                    gateText.text = "+" + leftRightValue.ToString();
                    //meshRenderer.material = positiveValueColor;
                    //meshRenderer.material = positiveValueColor;
                    //meshRenderer.material = positiveValueColor;
                    meshRenderer.materials[1] = positiveValueColor;

                }
                else
                {
                    gateText.text = leftRightValue.ToString();
                    //meshRenderer.material = negativeValueColor;
                    //meshRenderer.material = negativeValueColor;
                    leftRightGate.GetComponent<Renderer>().materials[1].color = redColor;
                    //meshRenderer.material = negativeValueColor;
                    print(meshRenderer.materials[1].name);
                }
                //meshRenderer.material = ironColor;
                break;

            case GateType.Weapon:

                gateText.gameObject.SetActive(false);
                for (int i = 0; i < weaponsUI.childCount; i++)
                {
                    weaponsUI.GetChild(i).gameObject.SetActive(false);
                }

                switch (leftRightWeapon)
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
                meshRenderer.material = weaponColor;
                //meshRenderer.material = ironColor;
                //meshRenderer.sharedMaterials[1] = weaponColor;
                break;

            default:
                break;
        }
    }
}
