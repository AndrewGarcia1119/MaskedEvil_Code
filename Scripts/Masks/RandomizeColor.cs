using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    [SerializeField]
    Material[] colors = null;

    private void Awake()
    {
        if (colors == null) return;
        int randomColor = Random.Range(0, colors.Length);
        GetComponent<MeshRenderer>().material = colors[randomColor];
    }
}
