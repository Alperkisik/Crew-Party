using System;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossManager : MonoBehaviour
{
    public enum Status
    {
        Alive, Dead
    }

    public enum CombatStatus
    {
        Idling, Moving, Attacking, No_Enemy, Dead
    }

    public event EventHandler<OnHitPointLostEventArgs> OnHitPointLost;
    public class OnHitPointLostEventArgs : EventArgs
    {
        public int damage;
    }

    public event EventHandler OnAttack;
    [Header("Setup")]
    [SerializeField] GameObject splashPrefab;
    [SerializeField] GameObject splash;
    [SerializeField] Rigidbody physics;
    [SerializeField] Collider collider;
    [SerializeField] BossAnimationManager animationManager;
    [SerializeField] GameObject dynamicBossUI;
    [SerializeField] bool hasDynamicUI;
    [Header("Boss Enemy Scan Setup")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float enemyScanRange;
    [Header("Boss Stats")]
    [SerializeField] float movementSpeed;
    [SerializeField] int hitPoints;
    [SerializeField] float attackRange;
    [SerializeField] float attackRate;
    [Header("Toss Stats")]
    [SerializeField] float tossForceMultiplier = 70f;
    [SerializeField] float tossForceMultiplierRandomChangerValue = 10f;
    [SerializeField] float tossMinY = 1.6f;
    [SerializeField] float tossMaxY = 2.2f;
    Status status;
    CombatStatus combatStatus; //debug variable
    bool isMoving;
    bool isAIStarted;
    GameObject targetEnemy;
    public bool hasTarget;
    public bool canAttack;
    float bossScale;

    public static bool isFighting = false;

    void Start()
    {
        if (hasDynamicUI)
        {
            GameObject ui = transform.Find("UI").gameObject;
            GameObject bossUI = Instantiate(dynamicBossUI, ui.transform.position, Quaternion.identity, transform.root);
            bossUI.GetComponent<DynamicBossUI>().Setup(gameObject, ui);

            ui.SetActive(false);
            /*for (int i = 0; i < ui.transform.childCount; i++)
            {
                ui.transform.GetChild(i).gameObject.SetActive(false);
            } */
        }

        LevelControl.instance.OnBossFightStarted += _OnBossFightStarted;
        LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;

        status = Status.Alive;
        combatStatus = CombatStatus.Idling;
        isMoving = false;
        isAIStarted = false;
        hasTarget = false;
        animationManager.AnimateIdle();
        bossScale = transform.localScale.x;
    }

    private void _OnBossFightFinished(object sender, EventArgs e)
    {
        if (status == Status.Dead)
        {
            LevelControl.instance.OnBossFightStarted -= _OnBossFightStarted;
            LevelControl.instance.OnBossFightFinished -= _OnBossFightFinished;
            Destroy(gameObject);
        }
        else animationManager.AnimateVictory();
    }

    private void _OnBossFightStarted(object sender, EventArgs e)
    {
        isAIStarted = true;
        canAttack = true;
        hasTarget = false;
        isMoving = false;
        //Vector3 scale = transform.localScale;
        //scale.z *= -1f;
        //transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (isAIStarted)
        {
            if (IsBossAlive())
            {
                if (hasTarget)
                {
                    if (IsTargetEnemyAlive())
                    {
                        if (IsTargetInWeaponRange()) Attack();
                        else MoveToTarget();
                    }
                    else HasNoTarget();
                }
                else FindEnemy();
            }
        }
    }

    private bool IsTargetEnemyAlive()
    {
        StickmanManager targetManager = targetEnemy.GetComponent<StickmanManager>();
        return targetManager.IsStickmanAlive();
    }

    private bool IsTargetInWeaponRange()
    {
        float targetDistance = Vector3.Distance(transform.position, targetEnemy.transform.position);

        if (targetDistance <= attackRange) return true;
        else return false;
    }

    private void MoveToTarget()
    {
        isFighting = true;
        //if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted) return;

        if (isMoving == false)
        {
            animationManager.AnimateRunning();
            //animationManager.AnimateRun(true);
            isMoving = true;
        }
        combatStatus = CombatStatus.Moving;
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        physics.velocity = direction * movementSpeed;

        FindEnemy();
    }

    private void Attack()
    {
        physics.velocity = Vector3.zero;

        if (isMoving == true) isMoving = false;

        if (canAttack)
        {
            combatStatus = CombatStatus.Attacking;
            canAttack = false;
            OnAttack?.Invoke(this, EventArgs.Empty);

            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(attackRate);

        if (targetEnemy == null) HasNoTarget();
        else
        {
            transform.LookAt(targetEnemy.transform);

            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

            if (enemyColliders.Length > 0)
            {
                int maxTargetEnemyCount = 3;
                int count = 0;

                /*if (transform.localScale.x >= 0.35f) maxTargetEnemyCount = 3;
                if (transform.localScale.x < 0.35f && transform.localScale.x > 0.2f) maxTargetEnemyCount = 2;
                else maxTargetEnemyCount = 1;*/

                List<GameObject> enemies = new List<GameObject>();
                foreach (Collider collider in enemyColliders)
                {
                    count++;
                    enemies.Add(collider.gameObject);
                    if (count == maxTargetEnemyCount) break;
                }

                foreach (GameObject enemy in enemies)
                {
                    StickmanManager targetManager = enemy.GetComponent<StickmanManager>();
                    Vector3 forceVector = transform.forward;
                    forceVector.y = Random.Range(tossMinY, tossMaxY);

                    float multiplier = Random.Range(tossForceMultiplier - tossForceMultiplierRandomChangerValue, tossForceMultiplier + tossForceMultiplierRandomChangerValue);
                    forceVector *= multiplier;
                    targetManager.Tossed(forceVector);
                }

                HasNoTarget();
            }
            else HasNoTarget();
        }

        canAttack = true;
    }

    public bool IsBossAlive()
    {
        if (status == Status.Alive) return true;
        else return false;
    }

    public void DecreaseHitPoints(int value)
    {
        if (hitPoints - value <= 0) Killed();
        else
        {
            //float scale = scaleDecreaseValueOnHit;
            //decreasedScale -= scaleDecreaseValueOnHit;
            VibrationManager.Instance.TriggerLightImpact();
            hitPoints -= value;

            float scale = (bossScale - 0.16f) / (float)hitPoints;

            Vector3 targetScale = transform.localScale - new Vector3(scale, scale, scale);
            if (targetScale.x <= 0.16) targetScale = new Vector3(0.16f, 0.16f, 0.16f);

            transform.localScale = targetScale;
        }

        OnHitPointLost?.Invoke(this, new OnHitPointLostEventArgs { damage = value });
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }

    private void Killed()
    {
        StopAllCoroutines();
        HasNoTarget();

        hitPoints = 0;
        status = Status.Dead;
        combatStatus = CombatStatus.Dead;
        CrewManager.gameFinished = true;
        GameObject splashEffect = Instantiate(splashPrefab, splash.transform.position, splash.transform.rotation, GameObject.Find("Garbage").transform);
        Vector3 splashScale = splashEffect.transform.localScale;
        splashScale = splashScale * transform.localScale.x;
        splashEffect.transform.localScale = splashScale;
        splashEffect.name = "Boss Splash";
        splashEffect.SetActive(true);
        splashEffect.GetComponent<MeshRenderer>().enabled = true;
        collider.enabled = false;
        transform.SetParent(GameObject.Find("Graveyard").transform);
        gameObject.SetActive(false);
        LevelControl.instance.BossFightFinish();
        //gameObject.SetActive(false);
    }

    private void FindEnemy()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, enemyScanRange, enemyLayer);

        if (enemyColliders.Length > 0)
        {
            List<GameObject> enemies = new List<GameObject>();
            foreach (Collider collider in enemyColliders)
            {
                enemies.Add(collider.gameObject);
            }

            if (enemies.Count == 1) targetEnemy = enemies[0];
            else targetEnemy = FindNearestEnemy(enemies);

            transform.LookAt(targetEnemy.transform);
            hasTarget = true;
        }
        else
        {
            HasNoTarget();
        }
    }

    private GameObject FindNearestEnemy(List<GameObject> enemies)
    {
        GameObject targetEnemy;

        float distance;
        float tempDistance;

        distance = Vector3.Distance(transform.position, enemies[0].gameObject.transform.position);
        targetEnemy = enemies[0].gameObject;

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

    private void HasNoTarget()
    {
        physics.velocity = Vector3.zero;
        isMoving = false;
        combatStatus = CombatStatus.No_Enemy;
        //animationManager.AnimateIdle();
        targetEnemy = null;
        hasTarget = false;
    }
}
