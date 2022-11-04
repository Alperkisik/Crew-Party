using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] GameObject enemyParty;
    [SerializeField] GameObject combatCamera;

    bool isCombatActive = false;
    int enemyPartySize;

    private void Start()
    {
        isCombatActive = false;
        LevelControl.instance.OnCombatStarted += CombatManager_OnCombatStarted;
        Invoke("DelayedCamera", 0.1f);
    }

    private void CombatManager_OnCombatStarted(object sender, System.EventArgs e)
    {
        //isCombatActive = true;
    }

    private void DelayedCamera()
    {
        combatCamera.SetActive(false);
    }

    void Update()
    {
        if (isCombatActive) CheckCombatSituation();
    }

    private void CheckCombatSituation()
    {
        enemyPartySize = enemyParty.GetComponent<EnemyPartyManager>().GetPartySize();

        if (enemyPartySize <= 0)
        {
            //Debug.Log("trigger");
            enemyParty.SetActive(false);
            combatCamera.SetActive(false);
            isCombatActive = false;
            LevelControl.instance.FinishCombat();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player-Party" || other.gameObject.tag == "Player")
        {
            isCombatActive = true;
            combatCamera.SetActive(true);

            enemyParty.GetComponent<EnemyPartyManager>().StartCombat();
            enemyPartySize = enemyParty.GetComponent<EnemyPartyManager>().GetPartySize();
            other.GetComponent<PartyManager>().StartCombat();

            LevelControl.instance.StartCombatEvent();
        }
    }
}
