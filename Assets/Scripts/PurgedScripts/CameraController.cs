using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

     [SerializeField] float ScrollSpeed = 15f;

    const int SECTORWIDHTHEIGHT = 64;
    const int CAMERABOUNDSX = 16;
    const int CAMERABOUNDSY = 9;


    void FixedUpdate()
    {

        if (CanMoveLeft())
        {
            if (Input.mousePosition.x <= Screen.width * 0.1)  // left (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.left * Time.deltaTime * ScrollSpeed, Space.World);
            }
        }
        if (CanMoveRight())
        {
            if (Input.mousePosition.x >= Screen.width * 0.9)  // right (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
        }
        if (CanMoveUp())
        {
            if (Input.mousePosition.y >= Screen.height * 0.9)  // upper (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.up * Time.deltaTime * ScrollSpeed, Space.World);
            }
        }
        if (CanMoveDown())
        {
            if (Input.mousePosition.y <= Screen.height * 0.1)  // down (1 - percent of padding) is the buffer zone to move
            {
                transform.Translate(Vector3.down * Time.deltaTime * ScrollSpeed, Space.World);
            }
        }
        
        
        
        


    }

    private bool CanMoveLeft()
    {
        if (gameObject.transform.position.x - CAMERABOUNDSX < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool CanMoveRight()
    {
        if (gameObject.transform.position.x + CAMERABOUNDSX > SECTORWIDHTHEIGHT)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool CanMoveUp()
    {
        if (gameObject.transform.position.y + CAMERABOUNDSY > SECTORWIDHTHEIGHT)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool CanMoveDown()
    {
        if (gameObject.transform.position.y - CAMERABOUNDSY < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

