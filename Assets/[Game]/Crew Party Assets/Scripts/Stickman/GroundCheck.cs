using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask roadLayer;
    [SerializeField] float distance;
    [SerializeField] GameObject groundCheck;

    public bool isGrounded = true;
    public bool groundChecking = true;

    private void Start()
    {
        groundChecking = true;
        LevelControl.instance.OnCombatStarted += GroundCheck_OnCombatStarted;
        LevelControl.instance.OnCombatFinish += GroundCheck_OnCombatFinish;
        LevelControl.instance.OnFinishLevelStarted += GroundCheck_OnFinishLevelStarted;
    }

    private void GroundCheck_OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        groundChecking = false;
    }

    private void GroundCheck_OnCombatFinish(object sender, System.EventArgs e)
    {
        groundChecking = true;
    }

    private void GroundCheck_OnCombatStarted(object sender, System.EventArgs e)
    {
        groundChecking = false;
    }

    private void FixedUpdate()
    {
        if (groundChecking) Check();
    }

    private void Check()
    {
        Collider[] roads = Physics.OverlapSphere(groundCheck.transform.position, distance, roadLayer);

        if (roads.Length == 0) GetComponent<StickmanManager>().Falled();
    }

    public void StopGroundCheck()
    {
        groundChecking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.transform.position, distance);
    }
}
