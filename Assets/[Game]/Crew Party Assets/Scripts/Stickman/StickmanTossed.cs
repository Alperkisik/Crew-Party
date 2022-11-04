using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanTossed : MonoBehaviour
{
    [SerializeField] StickmanManager stickmanManager;
    bool tossed = false;

    void Start()
    {
        stickmanManager.OnStickmanTossed += _OnStickmanTossed;
    }

    private void _OnStickmanTossed(object sender, System.EventArgs e)
    {
        tossed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (tossed && collision.gameObject.layer == 11)
        {
            stickmanManager.StickmanFalledToTheGround(transform.position);
        }
    }
}
