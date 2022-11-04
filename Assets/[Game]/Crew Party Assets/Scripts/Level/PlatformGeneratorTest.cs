using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGeneratorTest : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] List<Material> blockColors;
    [SerializeField] float blockStartValue;

    float blockValue;
    public void Generate(int count)
    {
        bool hasFinish;
        float val;
        int colorIndex = 0;
        float zPos = 0f;
        for (int i = 0; i < count; i++)
        {
            val = ((float)i / 10f);
            blockValue = blockStartValue + val;
            Vector3 spawnPosition = new Vector3(0f, 0f, zPos);
            GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity, transform);
            block.name = blockValue.ToString() + ". Block ";
            block.transform.localPosition = spawnPosition;
            block.transform.Find("Block").GetComponent<MeshRenderer>().material = blockColors[colorIndex];

            colorIndex++;
            zPos += 0.9f;
            if (colorIndex == blockColors.Count) colorIndex = 0;

            if (i == (count - 1)) hasFinish = true;
            else hasFinish = false;

            block.GetComponent<FinishBlock>().SetupBlock(blockValue, i, hasFinish);
        }
    }
}