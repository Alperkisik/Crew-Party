using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCapsule : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //11 road layer
        if(collision.gameObject.layer == 11)
        {
            Destroy(GetComponent<Rigidbody>());
        }
    }
}
