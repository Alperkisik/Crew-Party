using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stickman-Ally")
        {
            GetComponent<BoxCollider>().enabled = false;
            LevelControl.instance.LevelComplited();
        }
    }
}
