using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Necessary")]
    [SerializeField] WeaponData weaponData;
    [Header("Optional - If this weapon is a Firearm, Setup These")]
    [SerializeField] GameObject muzzle;
    [SerializeField] Transform bulletEjector;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject bulletCapsulePrefab;

    //int damage;
    //float range;
    //float attackRate;
    //WeaponType weaponType;
    //AttackType attackType;
    float defaultRange;

    private void Start()
    {
        //weaponType = weaponData.weapon;
        //damage = weaponData.damage;
        //range = weaponData.range;
        //attackRate = weaponData.attackRate;
        //attackType = weaponData.attackType;

        if (weaponData.weapon == WeaponType.Rifle)
        {
            defaultRange = weaponData.range;
            LevelControl.instance.OnBossFightStarted += _OnBossFightStarted;
            LevelControl.instance.OnBossFightFinished += _OnBossFightFinished;
        }
        
        if (weaponData.weapon == WeaponType.Rifle || weaponData.weapon == WeaponType.Shotgun) transform.parent.GetComponent<CombatAI>().OnFire += WeaponManager_OnFire;
    }

    private void _OnBossFightFinished(object sender, System.EventArgs e)
    {
        weaponData.range = defaultRange;
    }

    private void _OnBossFightStarted(object sender, System.EventArgs e)
    {
        weaponData.range += 2f;
    }

    private void WeaponManager_OnFire(object sender, System.EventArgs e)
    {
        /*muzzle.transform.Find("MuzzleFlash").gameObject.SetActive(true);
        muzzle.transform.Find("MuzzleFlash").GetComponent<ParticleSystem>().Play();
        
        EjectCapsule();

        GameObject garbage = GameObject.Find("Garbage");
        if (weaponData.weapon == WeaponType.Rifle)
        {
            GameObject rifleBullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation, garbage.transform);
            rifleBullet.GetComponent<Bullet>().BulletSetup(muzzle.transform.up, transform.parent.tag, weaponData.damage, weaponData.weapon);
        }
        else if(weaponData.weapon == WeaponType.Shotgun)
        {
            float bulletCount = 3;
            float minMaxY = 0.2f;
            float minMaxX = 0.2f;

            for (int i = 0; i < bulletCount; i++)
            {
                float x = Random.Range(-minMaxX, minMaxX); //yukarý asagý
                float y = Random.Range(-minMaxY, minMaxY); //sag sol

                GameObject shotgunBullet = Instantiate(bulletPrefab, muzzle.transform.position, Quaternion.identity, garbage.transform);

                Vector3 bulletDirection = muzzle.transform.up + new Vector3(x, y, 0f);
                shotgunBullet.GetComponent<Bullet>().BulletSetup(bulletDirection, transform.parent.tag, weaponData.damage, weaponData.weapon);
            }
        }*/
    }

    public void Fire()
    {
        muzzle.transform.Find("MuzzleFlash").gameObject.SetActive(true);
        muzzle.transform.Find("MuzzleFlash").GetComponent<ParticleSystem>().Play();

        EjectCapsule();

        GameObject garbage = GameObject.Find("Garbage");
        if (weaponData.weapon == WeaponType.Rifle)
        {
            GameObject rifleBullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation, garbage.transform);
            rifleBullet.GetComponent<Bullet>().BulletSetup(muzzle.transform.up, transform.parent.tag, weaponData.damage, weaponData.weapon);
        }
        else if (weaponData.weapon == WeaponType.Shotgun)
        {
            float bulletCount = 3;
            float minMaxY = 0.2f;
            float minMaxX = 0.2f;

            for (int i = 0; i < bulletCount; i++)
            {
                float x = Random.Range(-minMaxX, minMaxX); //yukarý asagý
                float y = Random.Range(-minMaxY, minMaxY); //sag sol

                GameObject shotgunBullet = Instantiate(bulletPrefab, muzzle.transform.position, Quaternion.identity, garbage.transform);

                Vector3 bulletDirection = muzzle.transform.up + new Vector3(x, y, 0f);
                shotgunBullet.GetComponent<Bullet>().BulletSetup(bulletDirection, transform.parent.tag, weaponData.damage, weaponData.weapon);
            }
        }
    }

    private void EjectCapsule()
    {
        Vector3 ejectForce;
        float forceMultiplier;

        forceMultiplier = Random.Range(8f, 12f);

        ejectForce.x = Random.Range(2f, 5f);
        ejectForce.y = Random.Range(8f, 12f);
        ejectForce.z = Random.Range(-5f, 5f);

        if (transform.parent.tag == "Stickman-Enemy") ejectForce.x *= -1f;

        Quaternion rotation = new Quaternion(90f, 0f, 0f, 0f);
        rotation.eulerAngles = new Vector3(90f, 0f, 0f);

        GameObject garbage = GameObject.Find("Garbage");
        GameObject capsule = Instantiate(bulletCapsulePrefab, bulletEjector.position, rotation, garbage.transform);
        capsule.GetComponent<Rigidbody>().AddForce(ejectForce * forceMultiplier);
    }

    public int GetDamage()
    {
        return weaponData.damage;
    }

    public float GetRange()
    {
        return weaponData.range;
    }

    public float GetAttackRate()
    {
        return weaponData.attackRate;
    }

    public WeaponType GetWeaponType()
    {
        return weaponData.weapon;
    }

    public AttackType GetAttackType()
    {
        return weaponData.attackType;
    }

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }
}
