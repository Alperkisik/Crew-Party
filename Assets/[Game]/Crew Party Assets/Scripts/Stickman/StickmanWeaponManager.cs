using Sirenix.OdinInspector;
using UnityEngine;

public class StickmanWeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject shotgun;
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject chair;
    [SerializeField] private GameObject bat;
    [SerializeField] private GameObject unarmed;
    public WeaponType equippedWeaponType;
    public GameObject equippedWeapon;

    private void UnEquipWeapons()
    {
        equippedWeaponType = WeaponType.Unarmed;

        rifle.SetActive(false);
        shotgun.SetActive(false);
        pillow.SetActive(false);
        chair.SetActive(false);
        bat.SetActive(false);
        unarmed.SetActive(false);
    }

    public void EquipWeapon(WeaponType weapon)
    {
        UnEquipWeapons();
        equippedWeaponType = weapon;

        switch (weapon)
        {
            case WeaponType.Pillow:
                pillow.SetActive(true);
                equippedWeapon = pillow;
                break;
            case WeaponType.Chair:
                chair.SetActive(true);
                equippedWeapon = chair;
                break;
            case WeaponType.Bat:
                bat.SetActive(true);
                equippedWeapon = bat;
                break;
            case WeaponType.Rifle:
                rifle.SetActive(true);
                equippedWeapon = rifle;
                break;
            case WeaponType.Shotgun:
                shotgun.SetActive(true);
                equippedWeapon = shotgun;
                break;
            case WeaponType.Unarmed:
                unarmed.SetActive(true);
                equippedWeapon = unarmed;
                break;
            default:
                break;
        }
    }

    [Button]
    private void UpdateWeapon()
    {
        EquipWeapon(equippedWeaponType);
    }
}
