using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEjector : MonoBehaviour
{
    [SerializeField] GameObject bulletCapsulePrefab;
    [SerializeField] Vector3 ejectForce;
    [SerializeField] float forceMultiplier;

    private void Start()
    {
        transform.parent.GetComponent<CombatAI>().OnFire += BulletEjector_OnFire;
    }

    private void BulletEjector_OnFire(object sender, System.EventArgs e)
    {
        //EjectCapsule();
    }

    private void EjectCapsule()
    {
        forceMultiplier = Random.Range(8f, 12f);
        
        ejectForce.x = Random.Range(2f, 5f);
        ejectForce.y = Random.Range(8f, 12f);
        ejectForce.z = Random.Range(-5f, 5f);

        if (transform.parent.tag == "Stickman-Enemy") ejectForce.x *= -1f;

        Quaternion rotation = new Quaternion(90f, 0f, 0f, 0f);
        rotation.eulerAngles = new Vector3(90f, 0f, 0f);

        GameObject capsule = Instantiate(bulletCapsulePrefab, transform.position, rotation, transform.root);
        capsule.GetComponent<Rigidbody>().AddForce(ejectForce * forceMultiplier);
    }
}
