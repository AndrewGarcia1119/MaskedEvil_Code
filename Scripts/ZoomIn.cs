using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    //As of right now, the current implementation is that the camera zooms in using the camera's FOV property.

    [SerializeField]
    float fovDivider = 4f;

    //default camera FOV, usually 60
    private float defaultFOV;
    //reference to camera component
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = defaultFOV / fovDivider;
        }
        else
        {
            cam.fieldOfView = defaultFOV;
        }
    }
}
