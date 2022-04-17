
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; } // everyone can ask for it
                                                                   // only world controller set it

    public World World { get; private set; }

    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject visitorPrefab;
    [SerializeField] private GameObject currentSpaceship;


    private void OnEnable() {

        if (Instance != null) {
            Debug.LogError("There shouldn't be more than one world controller");
        }
        else Instance = this;

        
        World = new World(); //default empty world

        // TODO: Bad implementation.
        World.WorkerPrefab = workerPrefab;
        World.EngineerPrefab = visitorPrefab;
        World.ShipPrefab = currentSpaceship;


        // Get the camera to middle of the world
        Camera.main.transform.position = new Vector3(World.Width / 2,
                World.Height / 2, Camera.main.transform.position.z);

     
    }
    public void CreateStarterBase() {
        //Instance.world.SetUpExampleStation();
        Instance.World.DeliverShipToWorld();
    }
    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        var x = Mathf.FloorToInt(coordinates.x);
        var y = Mathf.FloorToInt(coordinates.y);

        return World.GetTileAt(x, y);

    }

    
}
