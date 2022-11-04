using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    [SerializeField] GameObject target;
    public Vector3 rayVec;

    private void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 rayStartPos = transform.position + rayVec;
        Vector3 rayTargetPos = target.transform.position + rayVec;

        Vector3 direction = (rayTargetPos - rayStartPos).normalized;
        if (Physics.Raycast(rayStartPos, direction, out hit, Mathf.Infinity, layer))
        {
            Debug.DrawRay(rayStartPos, direction * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(rayStartPos, direction * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
