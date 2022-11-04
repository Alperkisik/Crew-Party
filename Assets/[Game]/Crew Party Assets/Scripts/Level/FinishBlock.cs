using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI blockText;
    [SerializeField] float rasingSpeed;
    [SerializeField] GameObject finish;
    [SerializeField] GameObject finishPointPrefab;
    
    Vector3 targetPosition;
    const float h = 0.72f;
    float blockValue;
    bool rasing = false;

    private void Update()
    {
        if (rasing) Rasing();
    }

    public void SetupBlock(float value, int line, bool hasFinish)
    {
        blockValue = value;
        targetPosition = transform.position + new Vector3(0f, h * (float)line, 0f);
        blockText.text = "x" + value.ToString("0.0");

        finish.SetActive(hasFinish);
        if (hasFinish == true)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, h * ((float)line - 1f), 0f);
            GameObject finishPoint = Instantiate(finishPointPrefab, spawnPosition, Quaternion.identity, transform.root);
            finishPoint.name = "Finish Point";
        }

        //transform.position = targetPosition;
        Invoke("StartRasing", 0.5f);
    }

    public float GetValue()
    {
        return blockValue;
    }

    private void StartRasing()
    {
        rasing = true;
    }

    private void StopRasing()
    {
        rasing = false;
        transform.position = targetPosition;
    }

    private void Rasing()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rasingSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            StopRasing();
        }
    }
}
