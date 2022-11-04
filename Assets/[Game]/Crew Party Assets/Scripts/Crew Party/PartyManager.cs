using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriflesGames.Managers;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public enum PartyType
    {
        Left, Right, Middle
    }

    public event EventHandler<OnStickmanDiedEventArgs> OnStickmanDied;
    public class OnStickmanDiedEventArgs : EventArgs
    {
        public List<GameObject> party;
    }

    [SerializeField] GameObject dynamicUI;
    [SerializeField] GameObject stickmanPrefab;
    public GameObject crewParty;
    [SerializeField] PartyType partyType;
    [Header("Party UI")]
    [SerializeField] Canvas ui_canvas;
    [SerializeField] TextMeshProUGUI Text_TMP_partySize;

    [HideInInspector] public List<GameObject> party;

    private float half_radius;
    private float angle;
    private float angleDivider;
    private float angleIncreaser;
    float a;
    float angleStart = 20f;
    float angleDividerIncreaser;

    private void Start()
    {
        LevelControl.instance.OnCombatStarted += PartyManager_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += PartyManager_OnCombatFinish;
        LevelControl.instance.OnBossFightStarted += PartyManager_OnBossFightStarted;
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
        LevelControl.instance.OnFinishLevelStarted += _OnFinishLevelStarted;
    }

    private void _OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        ui_canvas.enabled = false;
    }

    private void _OnBossFightFinished(object sender, System.EventArgs e)
    {
        if (party.Count > 0)
        {
            ui_canvas.enabled = true;
            StopCombatAI();
            StickmenCollisionSetting(true);
            RePositionParty();
            //Invoke("FightFinished", 0.1f);
        }
    }

    private void FightFinished()
    {
        StickmenCollisionSetting(true);
        RePositionParty();
    }

    private void PartyManager_OnBossFightStarted(object sender, System.EventArgs e)
    {
        ui_canvas.enabled = false;
        GameObject dynamicUi = Instantiate(dynamicUI, transform.position, Quaternion.identity, transform.root);
        dynamicUi.GetComponent<DynamicUI>().Setup(gameObject,party);

        StickmenCollisionSetting(false);
        ActivateCombatAI();
    }

    private void PartyManager_OnCombatFinish(object sender, System.EventArgs e)
    {
        if (party.Count > 0)
        {
            ui_canvas.enabled = true;
            StopCombatAI();
            StickmenCollisionSetting(true);
            RePositionParty();
            //Invoke("FightFinished", 0.2f);
        }
    }

    private void PartyManager_OnCombatStarted(object sender, System.EventArgs e)
    {
        /*ui_canvas.enabled = false;
        GameObject dynamicUi = Instantiate(dynamicUI, transform.position, Quaternion.identity, transform.root);
        dynamicUi.GetComponent<DynamicUI>().Setup(gameObject, party);

        StickmenCollisionSetting(false);
        ActivateCombatAI();*/
    }

    private void Awake()
    {
        party = new List<GameObject>();
        ResetGrowthValues();
    }

    private void StickmenCollisionSetting(bool value)
    {
        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<Rigidbody>().isKinematic = value;
        }
    }

    private void RePositionParty()
    {
        ResetGrowthValues();
        Vector3 origin = transform.position;
        bool lastOne = false;

        party[0].transform.SetParent(transform);
        party[0].GetComponent<RePosition>().Trigger(Vector3.zero, origin, lastOne);

        for (int i = 1; i < party.Count; i++)
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

            Vector3 formationDirection = new Vector3(x, 0f, z);
            Vector3 targetPosition = origin + formationDirection;
            Vector3 targetLocalPosition = formationDirection;

            party[i].transform.SetParent(transform);
            if (i + 1 == party.Count) lastOne = true;
            else lastOne = false;

            party[i].GetComponent<RePosition>().Trigger(targetLocalPosition, targetPosition, lastOne);

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

    private void ResetGrowthValues()
    {
        half_radius = 0.28f;
        a = half_radius;
        angle = angleStart;
        angleDividerIncreaser = 6f;
        angleDivider = 6f;
        angleIncreaser = 360f / angleDivider;
    }

    private void SetCrewParty()
    {
        ReErrangeParty();

        switch (partyType)
        {
            case PartyType.Left:
                crewParty.GetComponent<CrewManager>().SetLeftParty(party);
                break;
            case PartyType.Right:
                crewParty.GetComponent<CrewManager>().SetRightParty(party);
                break;
            case PartyType.Middle:
                crewParty.GetComponent<CrewManager>().SetMiddleParty(party);
                break;
            default:
                break;
        }
    }
    bool firstGrotwhHappened = false;
    public void Growth(int value)
    {
        if (party.Count == 0 && firstGrotwhHappened) return;

        if (firstGrotwhHappened == false) firstGrotwhHappened = true;

        Vector3 origin = transform.position;

        #region circle growth
        /*for (int i = 0; i < value; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            //Vector3 randomVector = new Vector3(Random.Range(-0.08f, 0.08f), 0f, Random.Range(-0.08f, 0.08f));
            Vector3 randomVector = Vector3.zero;
            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = origin + spawnDirection;

            GameObject stickmanClone = Instantiate(stickmanPrefab, spawnPosition, Quaternion.identity, transform);
            stickmanClone.transform.localPosition = spawnDirection + randomVector;
            party.Add(stickmanClone);
            partySize++;

            angle += angleIncreaser;

            if (angle >= 360f)
            {
                angle = 0f;
                angleDivider *= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius += 0.3f;
            }
        }*/
        #endregion

        GameObject stickmanClone;
        int index = 0;
        if (party.Count == 0)
        {
            stickmanClone = Instantiate(stickmanPrefab, origin, Quaternion.identity, transform);
            stickmanClone.transform.localPosition = Vector3.zero;
            stickmanClone.name = "Stickman : " + 1;
            party.Add(stickmanClone);
            index = 1;
        }

        for (int i = index; i < value; i++)
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

            stickmanClone = Instantiate(stickmanPrefab, spawnPosition, Quaternion.identity, transform);
            stickmanClone.transform.localPosition = spawnDirection;
            stickmanClone.name = "Stickman : " + (i + 1);

            party.Add(stickmanClone);

            angle += angleIncreaser;

            if (angle >= 360f + angleStart)
            {
                angle = angleStart;
                angleDivider += angleDividerIncreaser;
                angleIncreaser = 360f / angleDivider;
                a += half_radius;
            }
        }

        SetCrewParty();
        SetUI();
    }

    public void DeGrowth(int value)
    {
        if (party.Count == 0) return;

        value = Mathf.Abs(value);
        if (party.Count - value <= 0) value = party.Count;

        int length = party.Count - value - 1;
        for (int i = (party.Count - 1); i > length; i--)
        {
            party[i].GetComponent<StickmanManager>().Deleted();
            //Destroy(party[i].gameObject);
            party.RemoveAt(i);

            /*angle -= angleIncreaser;

            if (angle <= 0f)
            {
                angle = 360f;
                angleDivider /= 2f;
                angleIncreaser = 360f / angleDivider;
                half_radius -= 0.3f;
            }*/
        }

        SetCrewParty();

        if (party.Count == 0) ui_canvas.enabled = false;
        SetUI();
    }

    public void ArmParty(WeaponType weaponType)
    {
        if (party.Count <= 0) return;

        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<StickmanWeaponManager>().EquipWeapon(weaponType);
        }

        ReErrangeParty();
    }

    public void StartCombat()
    {
        ui_canvas.enabled = false;
        GameObject dynamicUi = Instantiate(dynamicUI, transform.position, Quaternion.identity, transform.root);
        dynamicUi.GetComponent<DynamicUI>().Setup(gameObject, party);

        StickmenCollisionSetting(false);
        ActivateCombatAI();
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
            //stickman.GetComponent<CombatAI>().StopCombatAI();
            //stickman.GetComponent<StickmanAnimationManager>().DefaultAnimationSpeed();
            //stickman.GetComponent<StickmanAnimationManager>().AnimateIdle();
            //stickman.GetComponent<StickmanAnimationManager>().AnimateRun(true);
            stickman.GetComponent<StickmanAnimationManager>().AnimateRunning();
        }
    }

    private void GetPartyInFormation()
    {
        ResetGrowthValues();

        Vector3 origin = transform.position;

        party[0].transform.position = origin;
        party[0].transform.localPosition = Vector3.zero;

        for (int i = 1; i < party.Count; i++)
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

            Vector3 formationDirection = new Vector3(x, 0f, z);
            Vector3 formationPosition = origin + formationDirection;

            party[i].transform.position = formationPosition;
            party[i].transform.localPosition = formationDirection;
            party[i].transform.eulerAngles = Vector3.zero;

            angle += angleIncreaser;

            if (angle >= 360f + angleStart)
            {
                angle = angleStart;
                angleDivider += angleDividerIncreaser;
                angleIncreaser = 360f / angleDivider;
                a += half_radius;
            }
        }

        SetUI();
        ReErrangeParty();
    }

    public void ReFormation()
    {
        GetPartyInFormation();
    }

    public void CombatWon()
    {
        GetPartyInFormation();

        foreach (GameObject stickman in party)
        {
            stickman.GetComponent<StickmanAnimationManager>().AnimateRun(true);
        }
    }

    public void DecreasePartySize(GameObject member)
    {
        party.Remove(member);

        SetCrewParty();

        SetUI();
        if (party.Count == 0) ui_canvas.enabled = false;
        if (OnStickmanDied != null)
        {
            OnStickmanDied?.Invoke(this, new OnStickmanDiedEventArgs { party = this.party });
        }
    }

    public int GetPartySize()
    {
        return party.Count;
    }

    public List<GameObject> GetPartyList()
    {
        return party;
    }

    public void SetParty(List<GameObject> party)
    {
        this.party = party;

        foreach (GameObject stickman in party)
        {
            stickman.transform.SetParent(transform);
        }

        GetPartyInFormation();

        if (ui_canvas.enabled == false) ui_canvas.enabled = true;
        SetUI();
    }

    public void ClearParty()
    {
        party.Clear();
    }

    private void SetUI()
    {
        Text_TMP_partySize.text = party.Count.ToString();
    }

    private void ReErrangeParty()
    {
        List<GameObject> sortedList = party;

        for (int i = 0; i < sortedList.Count - 1; i++)
        {
            for (int j = i; j < sortedList.Count; j++)
            {
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

        for (int i = 0; i < weaponSortedList.Count; i++)
        {
            if (weaponSortedList[i].GetComponent<StickmanWeaponManager>().equippedWeapon.GetComponent<WeaponManager>().GetWeaponType() == WeaponType.Rifle)
            {
                if (i + 1 == weaponSortedList.Count) break;

                for (int j = i + 1; j < weaponSortedList.Count; j++)
                {
                    if (weaponSortedList[j].GetComponent<StickmanWeaponManager>().equippedWeapon.GetComponent<WeaponManager>().GetWeaponType() == WeaponType.Shotgun)
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

        party = weaponSortedList;
    }

    public void TriggerFinalPartyFormation(bool IsSplit)
    {
        /*if (IsSplit)
        {
            foreach (GameObject stickman in party)
            {
                stickman.transform.SetParent(null);
            }

            RePositionParty();

            Invoke("StartFinalFormation", 0.5f);
        }
        else
        {
            StartFinalFormation();
        }*/
        StartFinalFormation();
    }

    private void StartFinalFormation()
    {
        StartCoroutine(DelayedSetRePosition());
    }

    IEnumerator DelayedSetRePosition()
    {
        Vector3 targetPosition;
        Vector3 targetLocalPosition;
        Vector3 origin = transform.position;

        float h = 0.72f;
        bool last = false;
        float speed = 6f;

        origin += new Vector3(0f, 0f, 1f);

        float xPos = -0.2f;
        float yPos = 0f;
        int hCount = 1;
        for (int i = 0; i < party.Count; i++)
        {
            //float y = h * (float)i;
            targetLocalPosition = new Vector3(xPos, yPos, 1f);
            targetPosition = origin + targetLocalPosition;
            
            xPos += 0.2f;
            if (xPos >= 0.4) 
            {
                xPos = -0.2f;
                yPos = h * hCount;
                hCount++;
            }
            
            if (i == party.Count - 1) last = true;

            party[i].GetComponent<GroundCheck>().StopGroundCheck();
            speed += yPos;
            party[i].GetComponent<RePosition>().TriggerFinal(targetLocalPosition, targetPosition, last, speed);

            yield return new WaitForSeconds(0.05f);
        }

        StopAllCoroutines();
    }
}
