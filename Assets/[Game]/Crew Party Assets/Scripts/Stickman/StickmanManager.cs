using System;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class StickmanManager : MonoBehaviour
{
    public enum Status
    {
        Alive, Dead
    }

    public event EventHandler OnStickmanTossed;

    [SerializeField] GameObject guyRig;
    [SerializeField] GameObject splashPrefab;
    [SerializeField] GameObject splash;
    [SerializeField] new CapsuleCollider collider;
    [SerializeField] private int hitPoints;

    Collider[] ragdollColliders;
    public float movementSpeed;
    public float enemyScanRange;
    public LayerMask enemyLayer;
    public Status status;
    bool finishLevel = false;
    bool tossed = false;
    bool IsFalledToTheGround = false;

    private void Awake()
    {
        status = Status.Alive;

        if (transform.tag == "Stickman-Ally")
        {
            ragdollColliders = guyRig.GetComponentsInChildren<Collider>(true);
            DoRagdoll(false);
        }
    }

    private void DoRagdoll(bool isRagDoll)
    {
        GetComponent<Animator>().enabled = !isRagDoll;
        foreach (var col in ragdollColliders)
        {
            col.enabled = isRagDoll;
            col.gameObject.GetComponent<Rigidbody>().isKinematic = !isRagDoll;
            col.gameObject.GetComponent<Rigidbody>().useGravity = isRagDoll;
        }
    }

    private void Start()
    {
        LevelControl.instance.OnFinishLevelStarted += StickmanManager_OnFinishLevelStarted;
    }

    private void StickmanManager_OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        finishLevel = true;
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }

    public void DecreaseHitPoints(int value)
    {
        if (IsStickmanAlive() == false) return;

        VibrationManager.Instance.TriggerLightImpact();
        hitPoints -= value;

        if (hitPoints <= 0) KilledInAction();
    }

    public bool IsStickmanAlive()
    {
        if (status == Status.Alive) return true;
        else return false;
    }

    public void KilledByObstacle()
    {
        VibrationManager.Instance.TriggerLightImpact();
        Dead();
    }

    public void Falled()
    {
        if (status == Status.Dead) return;

        hitPoints = 0;
        status = Status.Dead;
        gameObject.layer = 12;

        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Rigidbody>().useGravity = true;

        collider.enabled = false;

        transform.parent.GetComponent<PartyManager>().DecreasePartySize(this.gameObject);

        transform.parent = GameObject.Find("Graveyard").transform;

        DoRagdoll(true);
        Invoke("StopFall", 1.5f);
    }

    public void Deleted()
    {
        hitPoints = 0;

        status = Status.Dead;
        collider.enabled = false;
        GetComponent<GroundCheck>().StopGroundCheck();
        GetComponent<CombatAI>().StopCombatAI();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        transform.SetParent(GameObject.Find("Graveyard").transform);
    }

    public void Tossed(Vector3 force)
    {
        hitPoints = 0;
        
        status = Status.Dead;
        gameObject.layer = 12;
        tossed = true;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<GroundCheck>().StopGroundCheck();
        GetComponent<CombatAI>().StopCombatAI();

        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Rigidbody>().useGravity = true;

        collider.enabled = false;
        
        transform.parent.GetComponent<PartyManager>().DecreasePartySize(this.gameObject);
        transform.SetParent(GameObject.Find("Graveyard").transform);

        //tossedForce = force;
        //GetComponent<Rigidbody>().AddForce(force);

        OnStickmanTossed?.Invoke(this, EventArgs.Empty);
        //collider.enabled = true;
        GetComponent<Animator>().enabled = false;

        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            col.gameObject.GetComponent<Rigidbody>().useGravity = true;
            //col.gameObject.GetComponent<Rigidbody>().mass = 0.5f;
            col.gameObject.GetComponent<Rigidbody>().AddForce(force);
        }

        //Invoke("RagdollFall", 0.3f);
        //GetComponent<Rigidbody>().useGravity = true;
        //DoRagdoll(true);
        //Falled();
    }

    /*Vector3 tossedForce;
    private void RagdollFall()
    {
        OnStickmanTossed?.Invoke(this, EventArgs.Empty);
        //collider.enabled = true;
        GetComponent<Animator>().enabled = false;

        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            col.gameObject.GetComponent<Rigidbody>().useGravity = true;
            col.gameObject.GetComponent<Rigidbody>().mass = 0.5f;
            col.gameObject.GetComponent<Rigidbody>().AddForce(tossedForce);
        }
    }*/

    private void StopFall()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

        DoRagdoll(false);
    }

    private void Dead()
    {
        hitPoints = 0;
        status = Status.Dead;

        if (transform.tag == "Stickman-Ally")
        {
            transform.GetComponent<Rigidbody>().useGravity = false;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        collider.enabled = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        splash.SetActive(true);

        if (transform.tag == "Stickman-Enemy") transform.parent.parent.GetComponent<EnemyPartyManager>().DecreasePartySize(this.gameObject);
        else transform.parent.GetComponent<PartyManager>().DecreasePartySize(this.gameObject);

        GameObject splashEffect = Instantiate(splashPrefab, splash.transform.position, splash.transform.rotation,transform.root);
        Vector3 splashScale = splashEffect.transform.localScale;
        splashScale = splashScale * 0.14f;
        splashEffect.transform.localScale = splashScale;

        if (finishLevel) splashEffect.transform.SetParent(transform.root.Find("Finish").transform);
        else splashEffect.transform.SetParent(transform.root.Find("Graveyard").transform);

        transform.SetParent(GameObject.Find("Graveyard").transform);
    }

    private void KilledInAction()
    {
        GetComponent<CombatAI>().StopCombatAI();
        if (transform.tag == "Stickman-Ally") GetComponent<GroundCheck>().StopGroundCheck();

        Dead();
    }

    public void StickmanFalledToTheGround(Vector3 position)
    {
        if(IsFalledToTheGround == false)
        {
            IsFalledToTheGround = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            splash.SetActive(true);

            position.y = 0f;
            GameObject splashEffect = Instantiate(splashPrefab, position, splash.transform.rotation, GameObject.Find("Garbage").transform);
            
            Vector3 splashScale = splashEffect.transform.localScale;
            splashScale = splashScale * 0.14f;
            splashEffect.transform.localScale = splashScale;
        }
    }
}
