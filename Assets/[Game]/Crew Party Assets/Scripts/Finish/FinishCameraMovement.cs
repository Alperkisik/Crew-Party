using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.Managers;
using UnityEngine;

public class FinishCameraMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] float cameraTopDownTime;

    Vector3 targetPosition;
    Vector3 lastPosition;
    bool isMoving = false;
    bool isResettingPosition = false;
    bool isFollowing = false;
    List<GameObject> stickmanList;
    int stickmanIndex = 0;
    float defaultMovementSpeed;
    float topDownTime = 0f;
    float topDownTimeIncrease = 0f;
    float time = 0.0f;

    private void Start()
    {
        LevelControl.instance.OnFinishRePositionFinished += _OnFinishRePositionFinished;
        LevelControl.instance.OnStickmanRePositionDone += _OnStickmanRePositionDone;
        LevelControl.instance.OnStickmanReachedBlock += _OnStickmanReachedBlock;
        LevelControl.instance.OnFinishCameraRePositionFinished += _OnFinishCameraRePositionFinished;

        stickmanList = new List<GameObject>();
        positionList = new List<int>();
    }

    private void _OnFinishCameraRePositionFinished(object sender, System.EventArgs e)
    {
        //StopAllCoroutines();
    }

    bool IsCoroutinesStopped = false;

    private void _OnStickmanReachedBlock(object sender, LevelControl.OnStickmanReachedBlockEventArgs e)
    {

        if (IsCoroutinesStopped == false)
        {
            IsCoroutinesStopped = true;
            stickmanIndex = -1;
            StopAllCoroutines();
        }
        stickmanIndex++;

        if (stickmanIndex == 0)
        {
            Vector3 position = e.position;
            transform.position = position + new Vector3(0f, 0f, 1f);
            cinemachineVirtualCamera.Follow = transform;
            cinemachineVirtualCamera.LookAt = transform;
            targetPosition = GameObject.Find("Level Finisher").transform.position;

            isMoving = true;
        }

        //targetPosition = e.position;
        /*if (e.position.x == 0)
        {
            cinemachineVirtualCamera.Follow = stickmanList[stickmanIndex].transform;
            cinemachineVirtualCamera.LookAt = stickmanList[stickmanIndex].transform;
        }*/

        //cinemachineVirtualCamera.Follow = stickmanList[stickmanIndex].transform;
        //cinemachineVirtualCamera.LookAt = stickmanList[stickmanIndex].transform;
    }

    int rePositionedStickmanCount = 0;
    int stopIndex = 2;
    List<int> positionList;
    int positionIndex = 1;
    private void _OnStickmanRePositionDone(object sender, LevelControl.OnStickmanRePositionDoneEventArgs e)
    {
        rePositionedStickmanCount++;

        Vector3 position = e.position;
        transform.position = position;

        stickmanList.Add(e.stickman);
        positionList.Add(positionIndex);
        positionIndex++;
        if (positionIndex > 3) positionIndex = 1;

        if (rePositionedStickmanCount == stopIndex)
        {
            stopIndex += 3;
            cinemachineVirtualCamera.Follow = e.stickman.transform;
            cinemachineVirtualCamera.LookAt = e.stickman.transform;
            VibrationManager.Instance.TriggerLightImpact();
        }

        // 10 11 12
        // 7 8 9
        // 4 5 6
        // 1 2 3

        /*if (e.position.x == 0)
        {
            cinemachineVirtualCamera.Follow = e.stickman.transform;
            cinemachineVirtualCamera.LookAt = e.stickman.transform;
            VibrationManager.Instance.TriggerLightImpact();
        }*/
    }

    private void _OnFinishRePositionFinished(object sender, System.EventArgs e)
    {
        //cinemachineVirtualCamera.Follow = transform;
        //cinemachineVirtualCamera.LookAt = transform;

        //topDownTimeIncrease = cameraTopDownTime / stickmanList.Count;
        //period = cameraTopDownTime / stickmanList.Count;
        period = cameraTopDownTime / (stickmanList.Count / 3);
        stickmanIndex = stickmanList.Count - 1;
        StartCoroutine(CameraTopDownCoroutine());
        //nextActionTime = Time.time;

        //defaultMovementSpeed = movementSpeed;
        //ResetPosition();
    }

    float period;

    IEnumerator CameraTopDownCoroutine()
    {
        int eventIndex;
        if (stickmanList.Count >= 20) eventIndex = 11;
        else eventIndex = 5;

        while (true)
        {
            if(positionList[stickmanIndex] == 2)
            {
                cinemachineVirtualCamera.Follow = stickmanList[stickmanIndex].transform;
                cinemachineVirtualCamera.LookAt = stickmanList[stickmanIndex].transform;
            }

            if (stickmanIndex == eventIndex) 
            {
                LevelControl.instance.FinishCameraRePositionFinished();
                break;
            } 
            if (stickmanIndex <= 0) break;

            stickmanIndex--;

            yield return new WaitForSecondsRealtime(period);
        }
        //cinemachineVirtualCamera.Follow = stickmanList[0].transform;
        //cinemachineVirtualCamera.LookAt = stickmanList[0].transform;
        //StopAllCoroutines();
        //LevelControl.instance.FinishCameraRePositionFinished();
    }

    private void Update()
    {
        if (isMoving) Movement();
    }

    private void Movement()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        /*if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }*/

        if (isFollowing)
        {
            //Vector3 stickmanPosition = stickmanList[stickmanIndex].transform.position + new Vector3(0f, 0f, 1f);
            //transform.position = Vector3.MoveTowards(transform.position, stickmanPosition, movementSpeed);
            //cinemachineVirtualCamera.transform.position += new Vector3(1f, 0f, 0f);
            //transform.position = stickmanList[stickmanIndex].transform.position + new Vector3(0f, 0f, 1f);
        }
        else
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }

    public void Setup()
    {
        float h = 0.72f;
        GameObject player = GameObject.Find("Crew-Party");
        int count = player.GetComponent<CrewManager>().totalStickmanCount;

        transform.position = player.transform.position;
        //transform.eulerAngles = new Vector3(0f, -25f, 0f);
        targetPosition = player.transform.position + new Vector3(0f, h * (float)count, 0f);
        lastPosition = transform.position;
        isMoving = false;
    }



    private void ReSettingPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        movementSpeed += 0.5f;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            //transform.position = targetPosition;
            transform.position = stickmanList[0].transform.position;
            //cinemachineVirtualCamera.transform.eulerAngles = new Vector3(0f, -25f, 0f);
            cinemachineVirtualCamera.Follow = stickmanList[0].transform;
            cinemachineVirtualCamera.LookAt = stickmanList[0].transform;
            //cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset += new Vector3(3.264f,0f,0f); 
            //3.264
            movementSpeed = defaultMovementSpeed;
            isResettingPosition = false;
            isFollowing = true;
            //transform.eulerAngles = new Vector3(0f, -45f, 0f);
            LevelControl.instance.FinishCameraRePositionFinished();
        }
    }

    private void ResetPosition()
    {
        isResettingPosition = true;
        //transform.position = lastPosition;
        targetPosition = stickmanList[0].transform.position + new Vector3(0f, 0.42f, 0f);

        //targetPosition = lastPosition + new Vector3(0f,0f,1f);
        isMoving = true;

        //targetPosition = lastPosition + new Vector3(0f, 0f, 20f);
        movementSpeed = 7f;
    }
}
