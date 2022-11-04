using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlight : MonoBehaviour
{
    [SerializeField] Material highlightMaterial;

    Material originalMaterial;
    bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stickman-Ally" && triggered == false)
        {
            //meshRenderer.material = greenMaterial;
            triggered = true;
            originalMaterial = GetComponent<MeshRenderer>().material;
            GetComponent<MeshRenderer>().material = highlightMaterial;
            Invoke("DefaultBlockMaterial", 0.2f);
        }
    }

    private void DefaultBlockMaterial()
    {
        GetComponent<MeshRenderer>().material = originalMaterial;
    }
}
