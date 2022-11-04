using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_TMP_partySize;
    List<GameObject> party;
    public bool isBossFighting = false;

    bool hasSetup;

    
    void Start()
    {
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
        LevelControl.instance.OnCombatFinish += _OnCombatFinish;
    }

    private void _OnCombatFinish(object sender, System.EventArgs e)
    {
        LevelControl.instance.OnBossFightFinished -= _OnBossFightFinished;
        LevelControl.instance.OnCombatFinish -= _OnCombatFinish;
        print("Combat Finish Working");
        Destroy(gameObject);
    }

    private void _OnBossFightFinished(object sender, System.EventArgs e)
    {
        LevelControl.instance.OnBossFightFinished -= _OnBossFightFinished;
        LevelControl.instance.OnCombatFinish -= _OnCombatFinish;
        hasSetup = false;
        transform.parent = GameObject.Find("Garbage").transform;
        this.gameObject.SetActive(false);
        if(!BossManager.isFighting)
        {
            Destroy(gameObject);
        }
        
    }

    private void _OnStickmanDied(object sender, PartyManager.OnStickmanDiedEventArgs e)
    {
        if(e.party.Count > 0)
        {
            this.party = e.party;
            Text_TMP_partySize.text = party.Count.ToString();
        }
        else
        {
            Text_TMP_partySize.text = "0";
            LevelControl.instance.OnBossFightFinished -= _OnBossFightFinished;
            LevelControl.instance.OnCombatFinish -= _OnCombatFinish;

            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
            transform.parent = GameObject.Find("Garbage").transform;
            hasSetup = false;
            if (!isBossFighting)
            {
                Destroy(gameObject);
            }
        }
        
        //CalculateMidPoint();
    }

    void Update()
    {
        if(hasSetup) CalculateMidPoint();
    }

    public void Setup(GameObject parentParty,List<GameObject> party)
    {
        //CalculateMidPoint();
        this.party = party;

        if (this.party.Count == 0)
        {
            this.gameObject.SetActive(false);
            transform.parent = GameObject.Find("Garbage").transform;
        }
        else
        {
            hasSetup = true;
            parentParty.GetComponent<PartyManager>().OnStickmanDied += _OnStickmanDied;
            Text_TMP_partySize.text = this.party.Count.ToString();
        }
    }

    private void CalculateMidPoint()
    {
        Vector3 point = Vector3.zero;
        foreach (GameObject stickman in party)
        {
            point += stickman.transform.position;
        }

        if (party.Count == 0)
        {
            print("Working hereee!!");
            hasSetup = false;
            if (!BossManager.isFighting)
            {
                Destroy(gameObject);
            }

        }

        transform.position = point / party.Count;
        
    }
}
