using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevelStarter : MonoBehaviour
{
    [SerializeField] GameObject cameraFollowTarget;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Player-Party")
        {
            GameObject cam = Instantiate(cameraFollowTarget, transform.position, Quaternion.identity, transform.root);
            cam.GetComponent<FinishCameraMovement>().Setup();
            GetComponent<BoxCollider>().enabled = false;
            
            LevelControl.instance.StartFinishlevel();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stickman-Ally"))
        {
            GameObject cam = Instantiate(cameraFollowTarget, transform.position, Quaternion.identity, transform.root);
            cam.GetComponent<FinishCameraMovement>().Setup();
            GetComponent<BoxCollider>().enabled = false;
            CrewManager.gameFinished = true;
            LevelControl.instance.StartFinishlevel();
        }
    }
}
