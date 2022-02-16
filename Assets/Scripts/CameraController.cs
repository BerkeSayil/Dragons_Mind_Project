using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

     [SerializeField] float ScrollSpeed = 15f;
    
    void FixedUpdate()
    {

        if (isInsideWorld())
        {
            if (Input.mousePosition.y >= Screen.height * 0.9)  // upper (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.up * Time.deltaTime * ScrollSpeed, Space.World);
            }
            if (Input.mousePosition.y <= Screen.height * 0.1)  // down (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.down * Time.deltaTime * ScrollSpeed, Space.World);
            }
            if (Input.mousePosition.x >= Screen.width * 0.9)  // right (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            if (Input.mousePosition.x <= Screen.width * 0.1)  // left (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.left * Time.deltaTime * ScrollSpeed, Space.World);
            }
        }


    }

    private bool isInsideWorld()
    {
        return true;

    }
}

