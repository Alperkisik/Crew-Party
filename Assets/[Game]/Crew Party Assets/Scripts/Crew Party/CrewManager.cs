using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriflesGames.Managers;
using TriflesGames.ManagerFramework;
using UnityEngine;

public class CrewManager : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] GameObject divided;
    [SerializeField] GameObject merged;
    [Header("Game Setup")]
    [SerializeField] int startingPartySize;
    [SerializeField] float splitMaxMin;
    [SerializeField] float splitValue;

    Transform leftPartyTransform, rightPartyTransform;

    [HideInInspector] public List<GameObject> leftParty;
    [HideInInspector] public List<GameObject> mergedParty;
    [HideInInspector] public List<GameObject> rightParty;

    [HideInInspector] public int totalStickmanCount;

    bool canSplit;
    [HideInInspector] public bool split;
    [HideInInspector] public bool finishLevelStarted = false;
    [HideInInspector] public bool combatStarted = false;
    public bool mergedforBossFight = false;
    public static bool gameFinished = false;

    private float sensivity = 3f;
    Vector2 inputFirstPos;
    Vector2 inputCurrPos;

    private void Awake()
    {
        leftPartyTransform = divided.transform.Find("Left").transform;
        rightPartyTransform = divided.transform.Find("Right").transform;

        leftParty = new List<GameObject>();
        mergedParty = new List<GameObject>();
        rightParty = new List<GameObject>();

        canSplit = true;
        gameFinished = false;
    }

    void Start()
    {
        /*merged.GetComponent<PartyManager>().Growth(startingPartySize);
        mergedParty = merged.GetComponent<PartyManager>().GetPartyList();

        divided.SetActive(false);
        merged.SetActive(true);*/
        //Invoke("DelayedPartySetup", 0.05f);

        PartySetup();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        LevelControl.instance.OnCombatStarted += CrewManager_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += CrewManager_OnCombatFinish;
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
        LevelControl.instance.OnFinishLevelStarted += CrewManager_OnFinishLevelStarted;
        LevelControl.instance.OnBossFightStarted += CrewManager_OnBossFightStarted;
        LevelControl.instance.OnRePositionFinished += _OnRePositionFinished;
        LevelControl.instance.OnLevelEnd += _OnLevelEnd;
    }

    private void _OnRePositionFinished(object sender, EventArgs e)
    {
        if (!gameFinished)
        {
            print("Repositioning Work!");
            canSplit = true;
        }
    }

    private void PartySetup()
    {
        merged.GetComponent<PartyManager>().Growth(startingPartySize);
        mergedParty = merged.GetComponent<PartyManager>().GetPartyList();

        divided.SetActive(false);
        merged.SetActive(true);
    }

    private void _OnLevelEnd(object sender, EventArgs e)
    {
        canSplit = false;
        gameFinished = true;
        if (finishLevelStarted)
        {
            GameObject finish = GameObject.Find("Finish Point");
            transform.position = finish.transform.position;
        }
    }

    private void _OnBossFightFinished(object sender, EventArgs e)
    {
        canSplit = false;
        gameFinished = true;
        print("Boss Fight Finished!");
        if (totalStickmanCount > 0)
        {
            //canSplit = true;
            combatStarted = false;

            if (split)
            {
                foreach (GameObject stickman in leftParty)
                {
                    stickman.transform.SetParent(null);
                }
                foreach (GameObject stickman in rightParty)
                {
                    stickman.transform.SetParent(null);
                }
            }
            else
            {
                foreach (GameObject stickman in mergedParty)
                {
                    stickman.transform.SetParent(null);
                }
            }

            float distance;
            if (totalStickmanCount > 70) distance = 4f;
            else if (totalStickmanCount >= 50 && totalStickmanCount <= 70) distance = 3f;
            else distance = 4f;

            distance = 4f;

            Vector3 currentPos = transform.position;
            currentPos.z += distance;
            transform.position = currentPos;

            //Invoke("ContinueToMove", 1.4f);
        }
        else
        {
            LevelControl.instance.LevelFailed();
        }
    }

    private void CrewManager_OnCombatStarted(object sender, EventArgs e)
    {
        canSplit = false;
        combatStarted = true;
    }

    private void CrewManager_OnBossFightStarted(object sender, EventArgs e)
    {
        canSplit = false;
        combatStarted = true;
        if (!mergedforBossFight)
        {
            TriggerMerge();
            mergedforBossFight = true;
        }
    }

    private void CrewManager_OnFinishLevelStarted(object sender, EventArgs e)
    {
        canSplit = false;
        combatStarted = false;
        finishLevelStarted = true;

        bool isSplit = split;
        if (split) TriggerMerge();

        merged.GetComponent<PartyManager>().TriggerFinalPartyFormation(isSplit);
    }

    private void CrewManager_OnCombatFinish(object sender, EventArgs e)
    {
        if(totalStickmanCount > 0)
        {
            //canSplit = true;
            combatStarted = false;

            if (split)
            {
                foreach (GameObject stickman in leftParty)
                {
                    stickman.transform.SetParent(null);
                }
                foreach (GameObject stickman in rightParty)
                {
                    stickman.transform.SetParent(null);
                }
            }
            else
            {
                foreach (GameObject stickman in mergedParty)
                {
                    stickman.transform.SetParent(null);
                }
            }

            float distance;
            if (totalStickmanCount > 70) distance = 6f;
            else if(totalStickmanCount >= 50 && totalStickmanCount <= 70) distance = 5f;
            else distance = 4f;

            Vector3 currentPos = transform.position;
            currentPos.z += distance;
            transform.position = currentPos;

            Invoke("ContinueToMove", 0.6f);
        }
        else
        {
            LevelControl.instance.LevelFailed();
        }
    }

    private void ContinueToMove()
    {
        LevelControl.instance.RePositionFinish();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted) return;

        print("CAN SPLIT OUTSIDE !!!" + " " + canSplit);
        if (canSplit) 
        {

            print("CAN SPLIT INSIDE !!!" + " " + canSplit);
#if UNITY_IPHONE
            SplitInputs();
#endif

#if UNITY_EDITOR
            SplitForEditor();
            SplitInputs();
#endif
        } 

        if (combatStarted) CheckStickmanCount();
    }

    private void CheckStickmanCount()
    {
        /*if (totalStickmanCount <= 0 && finishLevelStarted)
        {
            LevelControl.instance.LevelComplited();
        }*/
        if (totalStickmanCount <= 0) 
        {
            LevelControl.instance.TriggerEnemyWinEvent();
            LevelControl.instance.LevelFailed();
        } 
    }
    private void SplitForEditor()
    {
        if (Input.GetKey(KeyCode.D))
        {
            if (totalStickmanCount <= 1) return;

            if (!split)
            {
                Divide();
                split = true;
            }

            Vector3 currentLeftPos = Vector3.zero;
            Vector3 currentRightPos = Vector3.zero;
            Vector3 newLeftDirection = Vector3.zero;
            Vector3 newRightDirection = Vector3.zero;

            newLeftDirection.x -= splitValue;
            newRightDirection.x += splitValue;

            /*float speed = 3f, acceleration = 10f;
            Vector3 velocityLeft = leftPartyTransform.position;
            velocityLeft.x = Mathf.MoveTowards(velocityLeft.x, -1f * speed, acceleration * Time.deltaTime);
            Vector3 velocityRight = rightPartyTransform.position;
            velocityRight.x = Mathf.MoveTowards(velocityRight.x, 1f * speed, acceleration * Time.deltaTime);
            leftPartyTransform.transform.Translate(velocityLeft * Time.deltaTime);
            rightPartyTransform.transform.Translate(velocityRight * Time.deltaTime);*/

            leftPartyTransform.position += newLeftDirection;
            rightPartyTransform.position += newRightDirection;

            currentLeftPos = leftPartyTransform.position;
            currentLeftPos.x = Mathf.Clamp(currentLeftPos.x, -splitMaxMin, 0f);

            currentRightPos = rightPartyTransform.position;
            currentRightPos.x = Mathf.Clamp(currentRightPos.x, 0f, splitMaxMin);

            leftPartyTransform.position = currentLeftPos;
            rightPartyTransform.position = currentRightPos;
        }

        else if (Input.GetKey(KeyCode.A))
        {
            if (Vector3.Distance(leftPartyTransform.position, rightPartyTransform.position) > 0.1f)
            {
                Vector3 currentLeftPos = Vector3.zero;
                Vector3 currentRightPos = Vector3.zero;
                Vector3 newLeftDirection = Vector3.zero;
                Vector3 newRightDirection = Vector3.zero;

                newLeftDirection.x += splitValue;
                newRightDirection.x -= splitValue;

                leftPartyTransform.position += newLeftDirection;
                rightPartyTransform.position += newRightDirection;

                currentLeftPos = leftPartyTransform.position;
                currentLeftPos.x = Mathf.Clamp(currentLeftPos.x, -splitMaxMin, 0f);
                currentRightPos = rightPartyTransform.position;
                currentRightPos.x = Mathf.Clamp(currentRightPos.x, 0f, splitMaxMin);

                leftPartyTransform.position = currentLeftPos;
                rightPartyTransform.position = currentRightPos;
            }
            else
            {
                if (split)
                {
                    split = false;
                    Merge();
                }
            }
        }
    }
    private void SplitInputs()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.x > 0)
                {
                    if (totalStickmanCount <= 1) return;

                    if (!split)
                    {
                        Divide();
                        split = true;
                    }

                    Vector3 currentLeftPos = Vector3.zero;
                    Vector3 currentRightPos = Vector3.zero;
                    Vector3 newLeftDirection = Vector3.zero;
                    Vector3 newRightDirection = Vector3.zero;

                    newLeftDirection.x -= (splitValue * Time.deltaTime * sensivity * touch.deltaPosition.x);
                    newRightDirection.x += (splitValue * Time.deltaTime * sensivity * touch.deltaPosition.x);

                    /*float speed = 3f, acceleration = 10f;
                    Vector3 velocityLeft = leftPartyTransform.position;
                    velocityLeft.x = Mathf.MoveTowards(velocityLeft.x, -1f * speed, acceleration * Time.deltaTime);
                    Vector3 velocityRight = rightPartyTransform.position;
                    velocityRight.x = Mathf.MoveTowards(velocityRight.x, 1f * speed, acceleration * Time.deltaTime);
                    leftPartyTransform.transform.Translate(velocityLeft * Time.deltaTime);
                    rightPartyTransform.transform.Translate(velocityRight * Time.deltaTime);*/

                    leftPartyTransform.position += newLeftDirection;
                    rightPartyTransform.position += newRightDirection;

                    currentLeftPos = leftPartyTransform.position;
                    currentLeftPos.x = Mathf.Clamp(currentLeftPos.x, -splitMaxMin, 0f);

                    currentRightPos = rightPartyTransform.position;
                    currentRightPos.x = Mathf.Clamp(currentRightPos.x, 0f, splitMaxMin);

                    leftPartyTransform.position = currentLeftPos;
                    rightPartyTransform.position = currentRightPos;
                }
                else
                {
                    if (Vector3.Distance(leftPartyTransform.position, rightPartyTransform.position) > 0.1f)
                    {
                        Vector3 currentLeftPos = Vector3.zero;
                        Vector3 currentRightPos = Vector3.zero;
                        Vector3 newLeftDirection = Vector3.zero;
                        Vector3 newRightDirection = Vector3.zero;

                        newLeftDirection.x += (splitValue * Time.deltaTime * sensivity * -touch.deltaPosition.x);
                        newRightDirection.x -= (splitValue * Time.deltaTime * sensivity * -touch.deltaPosition.x);

                        leftPartyTransform.position += newLeftDirection;
                        rightPartyTransform.position += newRightDirection;

                        currentLeftPos = leftPartyTransform.position;
                        currentLeftPos.x = Mathf.Clamp(currentLeftPos.x, -splitMaxMin, 0f);
                        currentRightPos = rightPartyTransform.position;
                        currentRightPos.x = Mathf.Clamp(currentRightPos.x, 0f, splitMaxMin);

                        leftPartyTransform.position = currentLeftPos;
                        rightPartyTransform.position = currentRightPos;
                    }
                    else
                    {
                        if (split)
                        {
                            split = false;
                            Merge();
                        }
                    }
                }
            }
        }
    }

    private void SetParents(bool mergedValue, bool dividedValue)
    {
        if (mergedValue)
        {
            merged.GetComponent<PartyManager>().ClearParty();
            merged.SetActive(mergedValue);
        }

        if (dividedValue)
        {
            divided.transform.Find("Left").GetComponent<PartyManager>().ClearParty();
            divided.transform.Find("Right").GetComponent<PartyManager>().ClearParty();
            divided.SetActive(dividedValue);
        }
    }

    public void SetLeftParty(List<GameObject> party)
    {
        leftParty = party;
        totalStickmanCount = leftParty.Count + rightParty.Count;
        //Text_TMP_partySize.text = totalStickmanCount.ToString();

        CheckCrewSize();
    }

    public void SetRightParty(List<GameObject> party)
    {
        rightParty = party;
        totalStickmanCount = leftParty.Count + rightParty.Count;
        //Text_TMP_partySize.text = totalStickmanCount.ToString();

        CheckCrewSize();
    }

    public void SetMiddleParty(List<GameObject> party)
    {
        mergedParty = party;
        totalStickmanCount = mergedParty.Count;
        //Text_TMP_partySize.text = totalStickmanCount.ToString();

        CheckCrewSize();
    }

    private void CheckCrewSize()
    {
        if (finishLevelStarted) return;

        if (totalStickmanCount <= 0 && !combatStarted) LevelControl.instance.LevelFailed();
    }

    private void Divide()
    {
        mergedforBossFight = false;
        divided.SetActive(true);

        leftParty.Clear();
        rightParty.Clear();

        leftPartyTransform.GetComponent<PartyManager>().ClearParty();
        rightPartyTransform.GetComponent<PartyManager>().ClearParty();

        int mergedPartyCount = mergedParty.Count;
        for (int i = 0; i < mergedPartyCount; i++)
        {
            //GameObject stickman = merged.transform.GetChild(i).gameObject;
            GameObject stickman = mergedParty[i].gameObject;
            //mergedParty.Remove(stickman);

            if (stickman.transform.localPosition.x < 0f) leftParty.Add(stickman);
            else rightParty.Add(stickman);
        }
        mergedParty.Clear();

        int leftCount = leftParty.Count, rightCount = rightParty.Count;
        int difference = Mathf.Abs(rightCount - leftCount);

        if (difference > 1)
        {
            int switchCount;
            if (difference % 2 == 0) switchCount = difference / 2;
            else switchCount = (difference - 1) / 2;

            if (leftCount > rightCount)
            {
                List<GameObject> sortedList = SortStickmanListByDistance(leftParty);

                for (int i = 0; i < switchCount; i++)
                {
                    rightParty.Add(sortedList[i]);
                    leftParty.Remove(sortedList[i]);
                }
            }
            else if (rightCount > leftCount)
            {
                List<GameObject> sortedList = SortStickmanListByDistance(rightParty);

                for (int i = 0; i < switchCount; i++)
                {
                    leftParty.Add(sortedList[i]);
                    rightParty.Remove(sortedList[i]);
                }
            }
        }

        leftPartyTransform.GetComponent<PartyManager>().SetParty(leftParty);
        rightPartyTransform.GetComponent<PartyManager>().SetParty(rightParty);

        mergedParty.Clear();
        merged.GetComponent<PartyManager>().ClearParty();
        merged.SetActive(false);

        totalStickmanCount = leftParty.Count + rightParty.Count;
        //Text_TMP_partySize.text = totalStickmanCount.ToString();
    }

    private List<GameObject> SortStickmanListByDistance(List<GameObject> partyList)
    {
        List<GameObject> sortedList = partyList;

        GameObject stickman;

        for (int i = 0; i < sortedList.Count - 1; i++)
        {
            for (int j = i; j < sortedList.Count; j++)
            {
                // >(büyük) iþareti <(küçük ) olarak deðiþtirilirse büyükten küçüðe sýralanýr
                if (Mathf.Abs(sortedList[i].transform.position.x) > Mathf.Abs(sortedList[j].transform.position.x))
                {
                    stickman = sortedList[j];
                    sortedList[j] = sortedList[i];
                    sortedList[i] = stickman;
                }
            }
        }
        return sortedList;
    }

    private void Merge()
    {
        mergedforBossFight = true;
        merged.SetActive(true);

        mergedParty.Clear();

        //leftPartyTransform = divided.transform.Find("Left").transform;
        //rightPartyTransform = divided.transform.Find("Right").transform;

        int partyCount = leftParty.Count;
        for (int i = 0; i < partyCount; i++)
        {
            GameObject stickman = leftParty[i];
            mergedParty.Add(stickman);
            //stickman.transform.SetParent(merged.transform);
            //leftPartyTransform.GetChild(0).SetParent(merged.transform);
        }
        leftParty.Clear();

        partyCount = rightParty.Count;
        for (int i = 0; i < partyCount; i++)
        {
            GameObject stickman = rightParty[i];
            mergedParty.Add(stickman);
            //stickman.transform.SetParent(merged.transform);
            //rightPartyTransform.GetChild(0).SetParent(merged.transform);
        }
        rightParty.Clear();

        merged.GetComponent<PartyManager>().SetParty(mergedParty);

        divided.SetActive(false);

        totalStickmanCount = mergedParty.Count;
        //Text_TMP_partySize.text = totalStickmanCount.ToString();
    }
    public void TriggerMerge()
    {
        split = false;
        Merge();
    }

    public int GetStickmanCount()
    {
        return totalStickmanCount;
    }
}
