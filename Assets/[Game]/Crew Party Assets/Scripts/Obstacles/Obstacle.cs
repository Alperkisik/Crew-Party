using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Stickman-Ally")
        {
            GameObject stickman = other.gameObject;
            stickman.GetComponent<StickmanManager>().KilledByObstacle();
            if (transform.tag == "Glass-Door") gameObject.SetActive(false);
        }
    }
}
