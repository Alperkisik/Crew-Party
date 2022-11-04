using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    [SerializeField] private float rollingSpeed;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * rollingSpeed * Time.deltaTime);
    }
}
