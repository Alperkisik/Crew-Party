using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationTest : MonoBehaviour
{
    [SerializeField] float half_radius = 0.28f; // a
    [SerializeField] float a = 0.28f;
    [SerializeField] float angleStart = 0f;
    [SerializeField] float angle = 0f; //0
    [SerializeField] float angleDivider = 6;
    [SerializeField] float angleDividerIncreaser;
    [SerializeField] float angleIncreaser = 60f; //60
    [SerializeField] int value;
    [SerializeField] GameObject stickmanPrefab;

    public bool autoEditorUpdate;
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }



    public void Generate()
    {
        Vector3 origin = transform.position;
        a = half_radius;
        angle = angleStart;
        angleDividerIncreaser = 6f;
        angleDivider = 6f;
        angleIncreaser = 360f / angleDivider;

        GameObject stickmanClone = Instantiate(stickmanPrefab, origin, Quaternion.identity, transform);
        stickmanClone.transform.localPosition = origin;
        stickmanClone.name = "1. Stickman angle : " + angle;

        for (int i = 1; i < value; i++)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * a;
            float z = Mathf.Sin(radian) * a;

            if ((angle - angleStart) % 60f != 0)
            {
                float h = (a * Mathf.Sqrt(3)) / 2f;
                x = Mathf.Cos(radian) * h;
                z = Mathf.Sin(radian) * h;
            }

            Vector3 spawnDirection = new Vector3(x, 0f, z);
            Vector3 spawnPosition = origin + spawnDirection;

            stickmanClone = Instantiate(stickmanPrefab, spawnPosition, Quaternion.identity, transform);
            stickmanClone.transform.localPosition = spawnDirection;
            stickmanClone.name = (i + 1) + ". Stickman angle : " + angle;
            angle += angleIncreaser;

            if (angle >= 360f + angleStart)
            {
                angle = angleStart;
                angleDivider += angleDividerIncreaser;
                angleIncreaser = 360f / angleDivider;
                a += half_radius;
            }
        }
    }
}

/*[CustomEditor(typeof(FormationTest))]
public class FormationTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FormationTest test = (FormationTest)target;

        if (DrawDefaultInspector())
        {
            if (test.autoEditorUpdate)
            {
                test.Generate();
            }
        }

        if (GUILayout.Button("Update"))
        {
            test.Generate();
        }
    }
}*/
