using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;

public class PartyMiniGroup : Actor<LevelManager>
{
    [SerializeField] private GameObject stickmanPrefab;
    [Header("Party UI")]
    [SerializeField] private TextMeshProUGUI Text_TMP_partySize;

    public List<GameObject> party_Variable = new List<GameObject>();
    public int startingPartySize;
    public int partySize = 0;

    private float half_radius;
    private float angle;
    private float angleDivider;
    private float angleIncreaser;

    private void Start()
    {
        transform.root.GetComponent<LevelControl>().OnCombatStarted += PartyManager_OnCombatStarted;
        transform.root.GetComponent<LevelControl>().OnCombatFinish += PartyManager_OnCombatFinish;
    }

    private void PartyManager_OnCombatFinish(object sender, System.EventArgs e)
    {
        StopCombatAI();
    }

    private void PartyManager_OnCombatStarted(object sender, System.EventArgs e)
    {
        ActivateCombatAI();
    }

    private void Awake()
    {
        ResetGrowthValues();
    }

    private void ResetGrowthValues()
    {
        angle = 0f;
        angleDivider = 4f;
        angleIncreaser = 360f / angleDivider;
        half_radius = 0.2f;
    }

    public void Growth(int value)
    {
        for (int i = 0; i < value; i++)
        {
            GameObject stickmanClone = Instantiate(stickmanPrefab, Vector3.zero, Quaternion.identity, transform);
            this.party_Variable.Add(stickmanClone);
            partySize++;

            angle += angleIncreaser;

            if (angle >= 360f)
            {
                angle = 0f;
                angleDivider *= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius += 0.3f;
            }
        }

        Text_TMP_partySize.text = party_Variable.Count.ToString();
        LocationChange(party_Variable);
    }

    public void LocationChange(List<GameObject> p)
    {
        ResetGrowthValues();
        for (int i = 0; i < p.Count; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = transform.position + spawnDirection;

            party_Variable[i].transform.position = spawnPosition;



            angle += angleIncreaser;

            if (angle >= 360f)
            {
                angle = 0f;
                angleDivider *= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius += 0.3f;
            }
        }
    }

    public void DeGrowth(int value)
    {
        Debug.Log("party size : " + party_Variable.Count + ", value : " + value);
        if (party_Variable.Count - value <= 0)
        {
            value = party_Variable.Count;
            LevelControl.instance.LevelFailed();
            DivideStackManGroup.isScrool = false;
        }

        value = Mathf.Abs(value);

        int length = party_Variable.Count - value - 1;
        for (int i = (party_Variable.Count - 1); i > length; i--)
        {
            Destroy(party_Variable[i].gameObject);
            party_Variable.RemoveAt(i);
            partySize--;
        }

        Text_TMP_partySize.text = party_Variable.Count.ToString();
    }

    public void ArmParty(WeaponType weaponType)
    {
        if (party_Variable.Count <= 0) return;

        foreach (GameObject stickman in party_Variable)
        {
            stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(weaponType);
        }
    }

    public void ActivateCombatAI()
    {
        //FindObjectOfType<DivideStackManGroup>().MovementChange(false);
        //FindObjectOfType<DivideStackManGroup>().SetUI(false,false,false);

        var count = transform.childCount;
        var child = new List<GameObject>();
        party_Variable.Clear();

        for (int i = 0; i < count; i++)
        {
            child.Add(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < child.Count; i++)
        {
            child[i].GetComponent<CombatAI>().ActivateCombatAI();
        }
      
       
    }

    public void StopCombatAI()
    {
        var count = transform.childCount;
        var child = new List<GameObject>();
        party_Variable.Clear();

        for (int i = 0; i < count; i++)
        {
            child.Add(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < child.Count; i++)
        {
            child[i].GetComponent<CombatAI>().StopCombatAI();
        }
        
    }

    private void GetPartyInLine()
    {
        ResetGrowthValues();

        for (int i = 0; i < party_Variable.Count; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            Vector3 lineDirection = new Vector3(x, 0f, z);
            Vector3 linePosition = transform.position + lineDirection;

            party_Variable[i].transform.position = linePosition;
            //GameObject stickmanClone = Instantiate(stickmanPrefab, spawnPosition, Quaternion.identity, transform);
            //party.Add(stickmanClone);
            partySize++;

            angle += angleIncreaser;

            if (angle >= 360f)
            {
                angle = 0f;
                angleDivider *= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius += 0.25f;
            }
        }

        Text_TMP_partySize.text = partySize.ToString();
    }

    public void CombatWon()
    {
        //hizaya sok
        GetPartyInLine();
        //movement'ý devam ettir
        FindObjectOfType<DivideStackManGroup>().MovementChange(true);

        for (int i = 0; i < party_Variable.Count; i++)
        {
            party_Variable[i].GetComponent<StickmanAnimationManager>().AnimateRun(true);
        }
      
    }

    public void DecreasePartySize(GameObject member)
    {
        party_Variable.Remove(member);
        partySize--;
        Text_TMP_partySize.text = partySize.ToString();
    }

    public int GetPartySize()
    {
        return party_Variable.Count;
    }
}
