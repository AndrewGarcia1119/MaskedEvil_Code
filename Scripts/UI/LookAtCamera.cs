using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    //references
    CameraSwitch cs;

    void Start()
    {
        cs = FindObjectOfType<CameraSwitch>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!cs) return;
        transform.forward = cs.activePlayerTransform.position - transform.position;
    }
}
