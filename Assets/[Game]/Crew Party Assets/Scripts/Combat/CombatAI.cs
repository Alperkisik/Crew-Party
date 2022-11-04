using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAI : MonoBehaviour
{
    [SerializeField] private StickmanAnimationManager animationManager;
    [SerializeField] new CapsuleCollider collider;
    [SerializeField] Rigidbody physics;

    public event EventHandler OnFire;

    public enum State
    {
        Attacking, Idling, Moving, Not_Active
    }

    State state;

    StickmanManager stickmanManager;
    WeaponManager weaponManager;
    GameObject equippedWeapon;
    GameObject targetEnemy;
    bool isCombatActive = false;
    bool hasTarget = false;
    bool canAttack = false;
    bool moving = false;
    bool bossFight;
    bool firing = false;

    private void Start()
    {
        LevelControl.instance.OnEnemyWin += _OnEnemyWin;
        LevelControl.instance.OnBossFightStarted += CombatAI_OnBossFightStarted;
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;

        LevelControl.instance.OnCombatStarted += CombatAI_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += _OnCombatFinish;
        state = State.Not_Active;
    }

    bool enemyWin = false;
    private void _OnEnemyWin(object sender, EventArgs e)
    {
        StopAllCoroutines();
        physics.velocity = Vector3.zero;
        physics.isKinematic = true;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        bossFight = false;
        isCombatActive = false;
        canAttack = false;
        state = State.Not_Active;
        moving = false;
        targetEnemy = null;
        hasTarget = false;
        enemyWin = true;
        Invoke("DelayedRunEnemyVictoryAnimation", 0.3f);
    }

    private void DelayedRunEnemyVictoryAnimation()
    {
        animationManager.DefaultAnimationSpeed();
        animationManager.AnimateVictory();
    }

    private void _OnCombatFinish(object sender, EventArgs e)
    {
        physics.isKinematic = true;
        StopAllCoroutines();
        physics.velocity = Vector3.zero;
        animationManager.DefaultAnimationSpeed();
        animationManager.AnimateIdle();
        //animationManager.AnimateRun(true);
        bossFight = false;
        isCombatActive = false;
        canAttack = false;
        state = State.Not_Active;
        moving = false;
        targetEnemy = null;
        hasTarget = false;
    }

    private void _OnBossFightFinished(object sender, EventArgs e)
    {
        physics.isKinematic = true;
        StopAllCoroutines();
        physics.velocity = Vector3.zero;
        animationManager.DefaultAnimationSpeed();
        animationManager.AnimateIdle();
        //animationManager.AnimateRun(true);
        //animationManager.AnimateRunning();
        bossFight = false;
        isCombatActive = false;
        canAttack = false;
        state = State.Not_Active;
        moving = false;
        targetEnemy = null;
        hasTarget = false;
    }

    private void CombatAI_OnCombatStarted(object sender, EventArgs e)
    {
        bossFight = false;
    }

    private void CombatAI_OnBossFightStarted(object sender, EventArgs e)
    {
        bossFight = true;
    }

    void FixedUpdate()
    {
        if (isCombatActive)
        {
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            if (hasTarget)
            {
                if (IsTargetEnemyAlive())
                {
                    transform.LookAt(targetEnemy.transform);

                    if (IsTargetInWeaponRange()) Attack();
                    else MoveToTarget();
                }
                else HasNoTarget();
            }
            else FindEnemy();
        }
    }

    private void StopMovement()
    {
        physics.velocity = Vector3.zero;
        physics.AddForce(Vector3.zero);
    }

    private void Attack()
    {
        state = State.Attacking;
        physics.velocity = Vector3.zero;
        //physics.isKinematic = true;

        StopMovement();

        if (moving)
        {
            moving = false;
            animationManager.DecreaseAnimationSpeed(1f);
            //animationManager.AnimateRun(false);
        }

        if (canAttack)
        {
            canAttack = false;
            switch (weaponManager.GetAttackType())
            {
                case AttackType.Melee:
                    StartCoroutine(MeleeAttack());
                    break;
                case AttackType.Range:
                    StartCoroutine(RangeAttack());
                    break;
                default:
                    break;
            }
        }
    }

    private bool IsTargetEnemyAlive()
    {
        if (bossFight)
        {
            BossManager targetManager = targetEnemy.GetComponent<BossManager>();
            return targetManager.IsBossAlive();
        }
        else
        {
            StickmanManager targetManager = targetEnemy.GetComponent<StickmanManager>();
            return targetManager.IsStickmanAlive();
        }
    }

    IEnumerator MeleeAttack()
    {
        animationManager.DefaultAnimationSpeed();
        animationManager.AnimateMelee();

        yield return new WaitForSeconds(weaponManager.GetAttackRate());

        if (targetEnemy == null) HasNoTarget();
        else
        {
            if (IsTargetEnemyAlive()) DealMeleeDamageToTarget();
            else HasNoTarget();
        }

        canAttack = true;
        state = State.Idling;
        //physics.isKinematic = false;
    }

    IEnumerator RangeAttack()
    {
        physics.isKinematic = true;
        /*if (firing == false)
        {
            firing = true;
            animationManager.Firing(true, weaponManager.GetAttackRate());
        }
        else
        {
            animationManager.AnimateFire(weaponManager.GetAttackRate());
        }*/

        animationManager.AnimateFire(weaponManager.GetAttackRate());
        equippedWeapon.GetComponent<WeaponManager>().Fire();
        //OnFire?.Invoke(this, new OnFireEventArgs { weapon = equippedWeapon });
        yield return new WaitForSeconds(weaponManager.GetAttackRate());
        canAttack = true;
        state = State.Idling;
        physics.isKinematic = false;
    }

    private void DealMeleeDamageToTarget()
    {
        int damage = weaponManager.GetDamage();
        if (bossFight)
        {
            BossManager targetManager = targetEnemy.GetComponent<BossManager>();
            targetManager.DecreaseHitPoints(damage);
        }
        else
        {
            StickmanManager targetManager = targetEnemy.GetComponent<StickmanManager>();
            targetManager.DecreaseHitPoints(damage);
        }
    }

    private void MoveToTarget()
    {
        if (state == State.Attacking) return;

        if (IsTargetEnemyAlive())
        {
            state = State.Moving;

            /*if (firing)
            {
                firing = false;
                animationManager.Firing(false,1f);
            }*/

            if (moving == false)
            {
                animationManager.AnimateIdle();
                animationManager.IncreaseAnimationSpeed(1f);
                //animationManager.AnimateRun(true);
                animationManager.AnimateRunning();
                moving = true;
            }

            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            physics.velocity = direction * stickmanManager.movementSpeed;
        }
        else
        {
            if (moving == true)
            {
                moving = false;
                animationManager.AnimateRun(false);
            }

            physics.velocity = Vector3.zero;
            HasNoTarget();
        }
    }

    private bool IsTargetInWeaponRange()
    {
        float targetDistance = Vector3.Distance(transform.position, targetEnemy.transform.position);
        float weaponRange = weaponManager.GetRange();

        if (targetDistance <= weaponRange) return true;
        else return false;
    }

    public void ActivateCombatAI()
    {
        stickmanManager = GetComponent<StickmanManager>();
        equippedWeapon = GetComponent<StickmanWeaponManager>().equippedWeapon;
        weaponManager = equippedWeapon.GetComponent<WeaponManager>();
        animationManager.AnimateRun(false);
        state = State.Idling;
        moving = false;
        collider.enabled = true;
        isCombatActive = true;
        canAttack = true;
    }

    public void StopCombatAI()
    {
        StopAllCoroutines();
        isCombatActive = false;
        canAttack = false;
        state = State.Not_Active;
        HasNoTarget();
    }

    private void FindEnemy()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, stickmanManager.enemyScanRange, stickmanManager.enemyLayer);

        if (enemyColliders.Length > 0)
        {
            List<GameObject> enemies = new List<GameObject>();
            foreach (Collider collider in enemyColliders)
            {
                enemies.Add(collider.gameObject);
            }

            AttackType attackType = weaponManager.GetAttackType();
            WeaponType weaponType = weaponManager.GetWeaponType();
            if (attackType == AttackType.Melee || weaponType == WeaponType.Shotgun) targetEnemy = FindNearestEnemy(enemies);
            else targetEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];

            transform.LookAt(targetEnemy.transform);
            hasTarget = true;
        }
        else
        {
            HasNoTarget();
        }
    }

    private void HasNoTarget()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        physics.velocity = Vector3.zero;
        physics.isKinematic = false;
        moving = false;
        targetEnemy = null;
        hasTarget = false;
        state = State.Idling;
        // animationManager.AnimateIdle();
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
                if (Mathf.Abs(sortedList[i].transform.position.z) > Mathf.Abs(sortedList[j].transform.position.z))
                {
                    stickman = sortedList[j];
                    sortedList[j] = sortedList[i];
                    sortedList[i] = stickman;
                }
            }
        }
        return sortedList;
    }

    private GameObject FindNearestEnemy(List<GameObject> enemies)
    {
        GameObject targetEnemy;

        if (enemies.Count == 1)
        {
            targetEnemy = enemies[0];
            return targetEnemy;
        }
        else
        {
            float distance;
            float tempDistance;

            distance = Vector3.Distance(transform.position, enemies[0].transform.position);
            targetEnemy = enemies[0];

            foreach (GameObject enemy in enemies)
            {
                tempDistance = Vector3.Distance(transform.position, enemy.transform.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    targetEnemy = enemy;
                }
            }

            return targetEnemy;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCombatActive)
        {
            if (collision.gameObject.layer != 11)
            {
                physics.AddForce(Vector3.zero);
                physics.velocity = Vector3.zero;
            }

            if (collision.gameObject.tag == "Stickman-Enemy" || collision.gameObject.tag == "Stickman-Ally")
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.zero);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        /*if (collision.transform.tag == transform.tag || collision.transform.tag == "Stickman-Enemy")
        {
            physics.AddForce(Vector3.zero);
            physics.velocity = Vector3.zero;
        }*/
        if (isCombatActive)
        {
            if (collision.gameObject.layer != 11)
            {
                physics.AddForce(Vector3.zero);
                physics.velocity = Vector3.zero;
            }

            if (collision.gameObject.tag == "Stickman-Enemy" || collision.gameObject.tag == "Stickman-Ally")
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.zero);
                }
            }
        }

    }
}
