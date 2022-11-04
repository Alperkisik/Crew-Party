using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyPartyManager : MonoBehaviour
{
    public enum FormationType
    {
        Hexagon, Circle
    }
    [Header("Party Generation Setup")]
    [SerializeField] private GameObject enemyStickmanPrefab;
    [SerializeField] private Transform _parent;
    [Header("Group Weapon Options")]
    [InfoBox("This option arms group randomly. If you dont check this options, group will be armed according to chosen weapon", InfoMessageType.None)]
    [SerializeField] private bool equipRandomWeapons;
    [InfoBox("This option arms group with choosen weapon type", InfoMessageType.None)]
    [SerializeField] private WeaponType weaponType;
    [Header("Generate Options")]
    [SerializeField] FormationType formationType;
    [SerializeField] private int partySize;
    [InfoBox("This option presents space between stickmans", InfoMessageType.None)]
    [SerializeField] [Range(0.28f, 0.28f * 3f)] private float radius = 0.28f;
    [Header("Party UI Setup")]
    [SerializeField] private TextMeshProUGUI Text_TMP_partySize;

    private List<GameObject> party;
    bool isInCombat;
    private float half_radius;
    private float angle;
    private float angleDivider;
    private float angleIncreaser;
    float a;
    float angleStart = 20f;
    float angleDividerIncreaser;
    bool generated = false;

    private void Awake()
    {
        LevelControl.instance.OnCombatStarted += EnemyPartyManager_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += EnemyPartyManager_OnCombatFinish;
        LevelControl.instance.OnEnemyWin += EnemyPartyManager_OnEnemyWin;
        isInCombat = false;
        Generate();
        Text_TMP_partySize.text = partySize.ToString();
        StickmenCollisionSetting(true);
    }

    private void EnemyPartyManager_OnEnemyWin(object sender, System.EventArgs e)
    {
        StopCombatAI();
        CombatWon();
    }

    private void StickmenCollisionSetting(bool value)
    {
        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<Rigidbody>().isKinematic = value;
        }
    }

    private void EnemyPartyManager_OnCombatFinish(object sender, System.EventArgs e)
    {
        if (isInCombat) isInCombat = false;
        StopCombatAI();
    }

    private void EnemyPartyManager_OnCombatStarted(object sender, System.EventArgs e)
    {
        //StickmenCollisionSetting(false);
        //if(isInCombat) ActivateCombatAI();
    }

    private void ResetGrowthValues()
    {
        half_radius = radius;
        a = half_radius;
        angle = angleStart;
        angleDividerIncreaser = 6f;
        angleDivider = 6f;
        angleIncreaser = 360f / angleDivider;
    }

    public void StartCombat()
    {
        isInCombat = true;
        StickmenCollisionSetting(false);
        if (isInCombat) ActivateCombatAI();
    }

    private void Generate()
    {
        ResetGrowthValues();

        if(formationType == FormationType.Hexagon)
        {
            HexagonFormation();
        }
        else
        {
            CircleFormation();
        }

        ReErrangeParty();
    }

    private void GenerateGroup()
    {
        generated = false;

        for (int i = 0; i < _parent.childCount; i++)
        {
            Destroy(_parent.GetChild(i));
        }

        Generate();

        generated = true;
    }

    private void HexagonFormation()
    {
        party = new List<GameObject>();

        Vector3 origin = _parent.position;
        GameObject stickmanClone;
        int index = 0;
        if (party.Count == 0)
        {
            stickmanClone = Instantiate(enemyStickmanPrefab, origin, _parent.rotation, _parent);
            stickmanClone.transform.localPosition = Vector3.zero;
            stickmanClone.name = "Enemy Stickman : " + 1;
            party.Add(stickmanClone);
            index = 1;
        }

        for (int i = index; i < partySize; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * a;
            float z = Mathf.Sin(radian) * a;

            if ((angle - angleStart) % 60f != 0)
            {
                float h = (a * Mathf.Sqrt(3)) / 2f;
                x = Mathf.Cos(radian) * h;
                z = Mathf.Sin(radian) * h;
            }

            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = origin + spawnDirection;

            GameObject enemyStickmanClone = Instantiate(enemyStickmanPrefab, spawnPosition, _parent.rotation, _parent);
            enemyStickmanClone.transform.localPosition = spawnDirection;
            enemyStickmanClone.name = "Enemy Stickman : " + (i + 1);
            enemyStickmanClone.GetComponent<StickmanAnimationManager>().AnimateIdle();

            ArmStickman(enemyStickmanClone);

            party.Add(enemyStickmanClone);

            angle += angleIncreaser;

            if (angle >= 360f + angleStart)
            {
                angle = angleStart;
                angleDivider += angleDividerIncreaser;
                angleIncreaser = 360f / angleDivider;
                a += half_radius;
            }
        }
    }

    private void CircleFormation()
    {
        party = new List<GameObject>();

        for (int i = 0; i < partySize; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            Vector3 randomVector = new Vector3(Random.Range(-0.08f, 0.08f), 0f, Random.Range(-0.08f, 0.08f));
            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = transform.position + spawnDirection;

            GameObject enemyStickmanClone = Instantiate(enemyStickmanPrefab, spawnPosition, _parent.rotation, _parent);
            enemyStickmanClone.GetComponent<StickmanAnimationManager>().AnimateIdle();
            enemyStickmanClone.name = "Enemy " + (i + 1).ToString();
            enemyStickmanClone.transform.localPosition = spawnDirection;

            if (equipRandomWeapons == true)
            {
                int randomValue = Random.Range(0, 6);

                if (randomValue == 0) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Pillow);
                else if (randomValue == 1) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Chair);
                else if (randomValue == 2) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Bat);
                else if (randomValue == 3) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Rifle);
                else if (randomValue == 4) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Shotgun);
                else if (randomValue == 5) enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Unarmed);
            }
            else
            {
                enemyStickmanClone.GetComponent<StickmanWeaponManager>().EquipWeapon(weaponType);
            }

            party.Add(enemyStickmanClone);

            angle += angleIncreaser;

            if (angle >= 360f)
            {
                angle = 0f;
                angleDivider *= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius += 0.6f;
            }
        }
    }

    private void ArmStickman(GameObject stickman)
    {
        if (equipRandomWeapons == true)
        {
            int randomValue = Random.Range(0, 6);

            if (randomValue == 0) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Pillow);
            else if (randomValue == 1) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Chair);
            else if (randomValue == 2) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Bat);
            else if (randomValue == 3) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Rifle);
            else if (randomValue == 4) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Shotgun);
            else if (randomValue == 5) stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(WeaponType.Unarmed);
        }
        else
        {
            stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(weaponType);
        }
    }

    public void DecreasePartySize(GameObject member)
    {
        party.Remove(member);
        partySize--;
        Text_TMP_partySize.text = partySize.ToString();
    }

    public void ActivateCombatAI()
    {
        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<CombatAI>().ActivateCombatAI();
        }
    }

    public void StopCombatAI()
    {
        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<CombatAI>().StopCombatAI();
        }
    }

    public int GetPartySize()
    {
        return party.Count;
    }

    public void CombatWon()
    {
        Debug.Log("trigger CombatWon");
        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<StickmanAnimationManager>().AnimateVictory();
        }
    }

    private void ReErrangeParty()
    {
        List<GameObject> sortedList = party;

        for (int i = 0; i < sortedList.Count - 1; i++)
        {
            for (int j = i; j < sortedList.Count; j++)
            {
                // >(büyük) iþareti <(küçük ) olarak deðiþtirilirse büyükten küçüðe sýralanýr
                if (sortedList[i].transform.localPosition.z < sortedList[j].transform.localPosition.z)
                {
                    GameObject stickman = sortedList[j];
                    sortedList[j] = sortedList[i];
                    sortedList[i] = stickman;
                }
            }
        }

        List<GameObject> weaponSortedList = sortedList;
        for (int i = 0; i < weaponSortedList.Count; i++)
        {
            GameObject rangeStickman = weaponSortedList[i];
            if (rangeStickman.GetComponent<StickmanWeaponManager>().equippedWeapon.GetComponent<WeaponManager>().GetAttackType() == AttackType.Range)
            {
                if (i + 1 == weaponSortedList.Count) break;

                for (int j = i + 1; j < weaponSortedList.Count; j++)
                {
                    GameObject meleeStickman = weaponSortedList[j];
                    if (meleeStickman.GetComponent<StickmanWeaponManager>().equippedWeapon.GetComponent<WeaponManager>().GetAttackType() == AttackType.Melee)
                    {
                        GameObject temp = weaponSortedList[i];
                        Vector3 tempPos = weaponSortedList[i].transform.localPosition;

                        weaponSortedList[i].transform.localPosition = weaponSortedList[j].transform.localPosition;
                        weaponSortedList[i] = weaponSortedList[j];

                        weaponSortedList[j].transform.localPosition = tempPos;
                        weaponSortedList[j] = temp;

                        break;
                    }
                }
            }
        }
    }
}
