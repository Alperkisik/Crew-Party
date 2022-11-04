using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanAI : MonoBehaviour
{
    public enum Status
    {
        Alive, Dead
    }

    [SerializeField] private int hitPoints;
    [SerializeField] private GameObject splash;
    [SerializeField] private GameObject characterModel;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Rigidbody physics;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float enemyScanRange;
    [SerializeField] private StickmanAnimationManager animationManager;

    public bool isCombatActivated = false;
    public Status status = Status.Alive;
    private bool targetFound = false;
    private GameObject targetEnemy;
    private bool IsWeaponValuesSet = false;
    private GameObject equippedWeapon;
    private float weaponRange;
    private int weaponDamage;
    private float attackRate;
    private bool canAttack = true;

    private void FixedUpdate()
    {
        if (isCombatActivated)
        {
            if (IsWeaponValuesSet == false) SetWeaponValues();

            if (targetFound == false)
            {
                FindEnemy();
            }
            else
            {
                if (IsTargetEnemyAlive())
                {
                    if (Vector3.Distance(transform.position, targetEnemy.transform.position) <= weaponRange)
                    {
                        if (canAttack)
                        {
                            AttackType attackType = equippedWeapon.GetComponent<WeaponManager>().GetAttackType();
                            transform.LookAt(targetEnemy.transform);
                            switch (attackType)
                            {
                                case AttackType.Melee:
                                    animationManager.AnimateMelee();
                                    break;
                                case AttackType.Range:
                                    animationManager.AnimateFire(1f);
                                    break;
                                default:
                                    break;
                            }
                            StartCoroutine(DealDamageAfterAnimation(targetEnemy, weaponDamage));
                            //DealDamage(targetEnemy, weaponDamage);
                        }
                    }
                    else
                    {
                        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetEnemy.transform.position, movementSpeed * Time.deltaTime);
                        //physics.velocity = targetEnemy.transform.position.normalized * movementSpeed;
                    }
                }
                else
                {
                    targetFound = false;
                }
            }
        }
    }

    public void ActivateCombatAI()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        isCombatActivated = true;
    }

    private void SetWeaponValues()
    {
        equippedWeapon = GetComponent<StickmanWeaponManager>().equippedWeapon;
        weaponRange = equippedWeapon.GetComponent<WeaponManager>().GetRange();
        weaponDamage = equippedWeapon.GetComponent<WeaponManager>().GetDamage();
        attackRate = equippedWeapon.GetComponent<WeaponManager>().GetAttackRate();

        IsWeaponValuesSet = true;
    }

    private bool IsTargetEnemyAlive()
    {
        if (targetEnemy.GetComponent<StickmanAI>().status == Status.Dead) return false;
        else return true;
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

            targetEnemy = FindNearestEnemy(enemies);
            targetFound = true;
        }
        else
        {
            ForwardMovement();
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

    private void ForwardMovement()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.forward + transform.localPosition, movementSpeed * Time.deltaTime);
        //physics.velocity = Vector3.forward * movementSpeed;
    }

    public int GetHP()
    {
        return hitPoints;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        StopAllCoroutines();
        status = Status.Dead;
        splash.SetActive(true);
        isCombatActivated = false;
        GetComponent<CapsuleCollider>().enabled = false;
        characterModel.SetActive(false);
        equippedWeapon.SetActive(false);
        if (transform.tag == "Stickman-Enemy") transform.parent.parent.GetComponent<EnemyPartyManager>().DecreasePartySize(this.gameObject);
        else transform.parent.GetComponent<PartyManager>().DecreasePartySize(this.gameObject);
        transform.parent = GameObject.Find("Graveyard").transform;
        //this.enabled = false;
    }

    public void DeActivateCombatAI()
    {
        isCombatActivated = false;
        //this.enabled = false;
    }

    IEnumerator DealDamageAfterAnimation(GameObject target, int damage)
    {
        yield return new WaitForSeconds(0.5f);

        target.GetComponent<StickmanAI>().TakeDamage(damage);
        canAttack = false;

        yield return new WaitForSeconds(attackRate);

        canAttack = true;
    }

    private void DealDamage(GameObject stickman, int damage)
    {
        stickman.GetComponent<StickmanAI>().TakeDamage(damage);
        canAttack = false;
        Invoke("AttackCooldown", attackRate);
    }

    private void AttackCooldown()
    {
        canAttack = true;
    }
}
