using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Mouse Movement https://pastebin.com/f7ThUg04 

[RequireComponent(typeof(Camera))]
public class MouseMovement : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 500f;
    [SerializeField]
    private float xMinClamp = -90f, xMaxClamp = 90f, yMinClamp = -60f, yMaxClamp = 60f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //control rotation around x axis (Look up and down)
        xRotation -= mouseY;

        //we clamp the rotation so we cant Over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, xMinClamp, xMaxClamp);

        //control rotation around y axis (Look up and down)
        YRotation += mouseX;

        YRotation = Mathf.Clamp(YRotation, yMinClamp, yMaxClamp);

        //applying both rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

    }
    
}
