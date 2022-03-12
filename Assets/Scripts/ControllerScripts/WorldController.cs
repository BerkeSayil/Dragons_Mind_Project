using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; } // everyone can ask for it
                                                                   // only worldcontroller set it

    public World world { get; protected set; }

    void OnEnable() {

        if (Instance != null) {
            Debug.LogError("There shouldn't be more than one worldcontroller");
        }
        else Instance = this;

        
        world = new World(); //default empty world

        // Get the camera to middle of the world
        Camera.main.transform.position = new Vector3(world.width / 2,
                world.height / 2, Camera.main.transform.position.z);

    }
    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        int x = Mathf.FloorToInt(coordinates.x);
        int y = Mathf.FloorToInt(coordinates.y);

        return world.GetTileAt(x, y);

    }
}
