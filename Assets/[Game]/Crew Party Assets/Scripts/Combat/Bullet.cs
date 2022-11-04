using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bloodImpact;

    Vector3 bulletDirection;
    string ownerTag;
    bool bulletSet = false;
    int damage;

    private void FixedUpdate()
    {
        if(bulletSet) GetComponent<Rigidbody>().velocity = bulletDirection * bulletSpeed;
    }

    public void BulletSetup(Vector3 direction,string ownerTag,int damage,WeaponType weaponType)
    {
        bulletDirection = direction;
        this.ownerTag = ownerTag;
        this.damage = damage;

        if(weaponType == WeaponType.Rifle) Destroy(gameObject, 0.8f);
        else if(weaponType == WeaponType.Shotgun) Destroy(gameObject, 0.5f);

        bulletSet = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ownerTag == "Stickman-Enemy" && other.gameObject.tag == "Stickman-Ally") DealDamage(other.gameObject);
        else if (ownerTag == "Stickman-Ally" && other.gameObject.tag == "Stickman-Enemy") DealDamage(other.gameObject);
        else if (other.gameObject.tag == "Boss") DealDamageToBoss(other.gameObject);
    }

    private void DealDamage(GameObject stickman)
    {
        GameObject blood = Instantiate(bloodImpact, transform.position, Quaternion.identity);
        Destroy(blood, 1.5f);

        stickman.GetComponent<Rigidbody>().AddForce(Vector3.zero);
        stickman.GetComponent<Rigidbody>().velocity = Vector3.zero;
        stickman.GetComponent<StickmanManager>().DecreaseHitPoints(damage);
        Destroy(this.gameObject);
    }

    private void DealDamageToBoss(GameObject boss)
    {
        GameObject garbage = GameObject.Find("Garbage");
        GameObject blood = Instantiate(bloodImpact, transform.position, Quaternion.identity, garbage.transform);
        Destroy(blood, 1.5f);

        boss.GetComponent<Rigidbody>().AddForce(Vector3.zero);
        boss.GetComponent<Rigidbody>().velocity = Vector3.zero;
        boss.GetComponent<BossManager>().DecreaseHitPoints(damage);
        Destroy(this.gameObject);
    }
}
