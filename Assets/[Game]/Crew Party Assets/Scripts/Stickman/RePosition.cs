using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePosition : MonoBehaviour
{
    [SerializeField] float speed;

    bool rePositioned = false;
    bool rePositioning = false;

    Vector3 targetLocalPosition;
    Vector3 targetPosition;
    bool lastOne = false;
    bool lastLevel = false;
    float originalSpeed;
    Transform parent;

    private void Start()
    {
        LevelControl.instance.OnFinishLevelStarted += _OnFinishLevelStarted;
    }

    private void _OnFinishLevelStarted(object sender, System.EventArgs e)
    {
        //speed = 1f;
    }

    void Update()
    {
        if (rePositioning)
        {
            if (!rePositioned) MoveTargetPosition();
        }
    }

    private void MoveTargetPosition()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.1f)
        {
            rePositioned = true;
            rePositioning = false;
            //transform.parent = parent;
            transform.position = targetPosition;
            transform.localPosition = targetLocalPosition;
            transform.eulerAngles = Vector3.zero;
            

            if (lastOne) LevelControl.instance.RePositionFinish();

            if (lastLevel)
            {
                speed = originalSpeed;
                LevelControl.instance.StickmanRePositionDone(targetPosition,gameObject);
                if (lastOne) LevelControl.instance.FinishRePositionFinished();
            }
        }
    }

    public void Trigger(Vector3 rePositionLocalTarget, Vector3 rePositionTarget, bool last)
    {
        targetLocalPosition = rePositionLocalTarget;
        targetPosition = rePositionTarget;
        transform.LookAt(targetPosition);
        
        rePositioned = false;
        rePositioning = true;
        
        lastLevel = false;
        lastOne = last;

        //transform.position = targetPosition;
        //transform.localPosition = targetLocalPosition;
    }

    public void TriggerFinal(Vector3 rePositionLocalTarget, Vector3 rePositionTarget, bool last, float speed)
    {
        targetLocalPosition = rePositionLocalTarget;
        targetPosition = rePositionTarget;
        transform.LookAt(targetPosition);
        lastOne = last;
        originalSpeed = this.speed;
        this.speed = speed;
        rePositioned = false;
        rePositioning = true;
        lastLevel = true;
    }
}
