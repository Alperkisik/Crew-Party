using System.Collections;
using System.Collections.Generic;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using TMPro;

public class DivideStackManGroup : Actor<GameManager>
{
    [SerializeField] private GameObject stickmanPrefab;
    [Header("Text UI")]
    public GameObject leftStickManText;
    public GameObject middleStickManText;
    public GameObject rightStickManText;
    [Header("Party UI")]
    [SerializeField] private TextMeshProUGUI Text_TMP_partySize;
    [Header("Parent")]
    public GameObject left;
    public GameObject mid;
    public GameObject right;
    [Header("Stickman  Parent")]
    public GameObject leftStickManParent;
    public GameObject middleStickManParent;
    public GameObject rightStickManParent;

    public static bool isMiddle = true;
    public static bool isScrool = false;

    // Min left-right

    Vector3 leftParentRight;
    Vector3 rightParentRight;

    //Floor
    float floorSize;
    public BoxCollider collider;

    // Max left-right
    float leftMaxParent;
    float rightMaxParent;
    bool click = false;


    public List<GameObject> party;
    public int startingPartySize;
    public int partySize = 0;

    private float half_radius;
    private float angle;
    private float angleDivider;
    private float angleIncreaser;

    private void Awake()
    {
        party = new List<GameObject>();
        ResetGrowthValues();
        Growth(startingPartySize);
    }

    private void Start()
    {
        floorSize = collider.bounds.size.x;

        transform.root.GetComponent<LevelControl>().OnCombatStarted += PartyManager_OnCombatStarted;
        transform.root.GetComponent<LevelControl>().OnCombatFinish += PartyManager_OnCombatFinish;
    }

    private void PartyManager_OnCombatFinish(object sender, System.EventArgs e)
    {
        FindObjectOfType<PartyMiniGroup>().StopCombatAI();
    }

    private void PartyManager_OnCombatStarted(object sender, System.EventArgs e)
    {
        FindObjectOfType<PartyMiniGroup>().ActivateCombatAI();
    }

    void FixedUpdate()
    {
        MinMaxValue();
        StickManScroll();
    }

    private void ResetGrowthValues()
    {
        partySize = 0;
        angle = 0f;
        angleDivider = 4f;
        angleIncreaser = 360f / angleDivider;
        half_radius = 0.2f;
    }

    public void Growth(int value)
    {
        for (int i = 0; i < value; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = transform.position + spawnDirection;

            GameObject stickmanClone = Instantiate(stickmanPrefab, spawnPosition, Quaternion.identity, transform);
            stickmanClone.transform.SetParent(middleStickManParent.transform);
            middleStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Add(stickmanClone);
            middleStickManParent.GetComponent<PartyMiniGroup>().partySize++;
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
        }
        Text_TMP_partySize.text = partySize.ToString();
    }

    public void Merge(GameObject leftParent, GameObject rightParent)
    {
        isMiddle = true;
        party.Clear();
        SetUI(false, true, false);
        var leftCount = leftParent.transform.childCount;

        for (int i = 0; i < leftCount; i++)
        {
            party.Add(leftParent.transform.GetChild(0).gameObject);
            leftParent.transform.GetChild(0).SetParent(middleStickManParent.transform);
        }

        var rightCount = rightParent.transform.childCount;

        for (int i = 0; i < rightCount; i++)
        {
            party.Add(rightParent.transform.GetChild(0).gameObject);
            rightParent.transform.GetChild(0).SetParent(middleStickManParent.transform);

        }
        partySize = party.Count;
        Text_TMP_partySize.text = partySize.ToString();
    }

    public void DivideStackMan()
    {
        isMiddle = false;
        var middleParentTotalStickMan = new List<GameObject>();

        for (int i = 0; i < middleStickManParent.transform.childCount; i++)
        {
            middleParentTotalStickMan.Add(middleStickManParent.transform.GetChild(i).gameObject);
        }

        var leftList = new List<GameObject>();
        var rightList = new List<GameObject>();
        var forCount = middleParentTotalStickMan.Count;

        for (int i = 0; i < forCount; i++)
        {
            if (middleStickManParent.transform.GetChild(i).transform.position.x < .1f)
            {
                leftList.Add(middleStickManParent.transform.GetChild(i).gameObject);
                middleParentTotalStickMan.Remove(middleStickManParent.transform.GetChild(i).gameObject);
            }
            else
            {
                rightList.Add(middleStickManParent.transform.GetChild(i).gameObject);
                middleParentTotalStickMan.Remove(middleStickManParent.transform.GetChild(i).gameObject);
            }
        }

        leftStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Clear();
        rightStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Clear();

        leftList.ForEach(s => leftStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Add(s));
        rightList.ForEach(s => rightStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Add(s));

        leftList.ForEach(s => s.transform.SetParent(leftStickManParent.transform));
        rightList.ForEach(s => s.transform.SetParent(rightStickManParent.transform));

        leftStickManParent.GetComponent<PartyMiniGroup>().partySize = leftStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Count;
        rightStickManParent.GetComponent<PartyMiniGroup>().partySize = rightStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Count;

        /*if (FinishManager.CanMove)
        {
            SetUI(false, false, false);
        }
        else
        {
            SetUI(true, false, true);
        }*/
    }

    private void StickManScroll()
    {
        if (Input.touchCount > 0 && GameManager.Instance.IsGameStarted)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.x > 0)
                {
                    if (leftMaxParent > -(floorSize / 2f) && rightMaxParent < (floorSize / 2f) - .2f)
                    {
                        Vector3 currentRightPos = Vector3.zero;
                        Vector3 currentLeftPos = Vector3.zero;
                        Vector3 newLeftDirection = Vector3.zero;
                        Vector3 newRightDirection = Vector3.zero;
                        
                        newLeftDirection.x -= 0.1f;
                        newRightDirection.x += 0.1f;

                        left.transform.position += newLeftDirection;
                        right.transform.position += newRightDirection;

                        float minMax = (floorSize / 2f) - 1.6f;

                        currentLeftPos = left.transform.position;
                        currentLeftPos.x = Mathf.Clamp(currentLeftPos.x, -minMax, 0f);
                        
                        currentRightPos = right.transform.position;
                        currentRightPos.x = Mathf.Clamp(currentRightPos.x, 0f, minMax);

                        left.transform.position = currentLeftPos;
                        right.transform.position = currentRightPos;

                        //right.transform.position = new Vector3(right.transform.position.x + .2f, right.transform.position.y, right.transform.position.z);
                        //left.transform.position = new Vector3(left.transform.position.x - .2f, left.transform.position.y, left.transform.position.z);
                        if (!click) DivideStackMan();

                        SetCollider(true, false, true);
                        click = true;
                    }
                }
                else
                {
                    if (Vector3.Distance(rightParentRight, leftParentRight) > 0.5f)
                    {
                        right.transform.position = new Vector3(right.transform.position.x - .2f, right.transform.position.y, right.transform.position.z);
                        left.transform.position = new Vector3(left.transform.position.x + .2f, left.transform.position.y, left.transform.position.z);
                    }
                    else
                    {
                        if (click)
                        {
                            SetUI(false, true, false);
                            Merge(leftStickManParent, rightStickManParent);
                            click = false;
                            SetCollider(false, true, false);
                        }
                    }
                }
            }
        }
    }

    private void MinMaxValue()
    {
        if (leftStickManParent.transform.childCount > 0 && rightStickManParent.transform.childCount > 0)
        {
            leftParentRight = setMin(leftStickManParent.GetComponentsInChildren<Transform>(), true); //left
            rightParentRight = setMin(rightStickManParent.GetComponentsInChildren<Transform>(), false); //Right

            leftMaxParent = setMax(leftStickManParent.GetComponentsInChildren<Transform>(), true); // Left
            rightMaxParent = setMax(rightStickManParent.GetComponentsInChildren<Transform>(), false); // Right
        }
    }

    private Vector3 setMin(Transform[] x, bool left)
    {
        var variable = x[1].transform.position.x;      // .1f 
        for (int i = 1; i < x.Length; i++)
        {
            if (left)
            {
                if (x[i].transform.position.x < variable)
                {
                    variable = x[i].transform.position.x;
                }
            }
            else
            {
                if (x[i].transform.position.x < variable)
                {
                    variable = x[i].transform.position.x;
                }
            }

        }
        return new Vector3(variable, 0, 0);
    }

    private float setMax(Transform[] x, bool left)
    {
        var variable = x[1].transform.position.x;      // .1f 
        for (int i = 1; i < x.Length; i++)
        {
            if (left)
            {
                if (x[i].transform.position.x < variable)
                {
                    variable = x[i].transform.position.x;
                }
            }
            else
            {
                if (x[i].transform.position.x > variable)
                {
                    variable = x[i].transform.position.x;
                }
            }

        }
        return variable;
    }

    public void SetCollider(bool leftCol, bool MidCol, bool RightCol)
    {
        leftStickManParent.GetComponent<BoxCollider>().enabled = leftCol;
        middleStickManParent.GetComponent<BoxCollider>().enabled = MidCol;
        rightStickManParent.GetComponent<BoxCollider>().enabled = RightCol;
    }

    public void SetUI(bool leftUI, bool MidUI, bool RightUI)
    {
        leftStickManText.SetActive(leftUI);
        middleStickManText.SetActive(MidUI);
        rightStickManText.SetActive(RightUI);

        leftStickManText.transform.GetChild(1).GetComponent<TMP_Text>().text = leftStickManParent.GetComponent<PartyMiniGroup>().partySize.ToString();
        if (middleStickManText.activeSelf) middleStickManText.transform.GetChild(1).GetComponent<TMP_Text>().text = middleStickManParent.GetComponent<PartyMiniGroup>().partySize.ToString();
        rightStickManText.transform.GetChild(1).GetComponent<TMP_Text>().text = rightStickManParent.GetComponent<PartyMiniGroup>().partySize.ToString();

    }

    public void ReCalculateAll()
    {
        var leftCount = leftStickManParent.transform.childCount;
        party.Clear();
        for (int i = 0; i < leftCount; i++)
        {
            party.Add(leftStickManParent.transform.GetChild(0).gameObject);
            leftStickManParent.transform.GetChild(0).SetParent(middleStickManParent.transform);
        }

        var rightCount = rightStickManParent.transform.childCount;

        for (int i = 0; i < rightCount; i++)
        {
            party.Add(rightStickManParent.transform.GetChild(0).gameObject);
            rightStickManParent.transform.GetChild(0).SetParent(middleStickManParent.transform);

        }
        var middleCount = middleStickManParent.transform.childCount;

        for (int i = 0; i < middleCount; i++)
        {
            party.Add(middleStickManParent.transform.GetChild(0).gameObject);
            middleStickManParent.transform.GetChild(0).SetParent(middleStickManParent.transform);

        }
        middleStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Clear();

        for (int i = 0; i < party.Count; i++)
        {
            middleStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Add(party[i]);


        }
        middleStickManParent.GetComponent<PartyMiniGroup>().partySize = middleStickManParent.GetComponent<PartyMiniGroup>().party_Variable.Count;


        ReCalculateGrowth();
    }

    public void ReCalculateGrowth()
    {

        ResetGrowthValues();
        for (int i = 0; i < party.Count; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * half_radius;
            float z = Mathf.Sin(radian) * half_radius;

            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = transform.position + spawnDirection;

            party[i].transform.position = spawnPosition;



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

    public int GetPartySize()
    {


        int leftParty = leftStickManParent.GetComponent<PartyMiniGroup>().partySize;
        int middleParty = middleStickManParent.GetComponent<PartyMiniGroup>().partySize;
        int rightParty = rightStickManParent.GetComponent<PartyMiniGroup>().partySize;

        return (leftParty + middleParty + rightParty);
    }

    public void StopCombatAI()
    {
        middleStickManParent.GetComponent<PartyMiniGroup>().StopCombatAI();

    }

    public void CombatWon()
    {
        middleStickManParent.GetComponent<PartyMiniGroup>().CombatWon();
    }

    public void MovementChange(bool p)
    {
        GetComponent<MovementController>().enabled = p;
    }
}
