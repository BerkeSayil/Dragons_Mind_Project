
using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; } // everyone can ask for it
                                                                   // only world controller set it

    public World World { get; private set; }

    [SerializeField] private GameObject workerPrefab;

    private void OnEnable() {

        if (Instance != null) {
            Debug.LogError("There shouldn't be more than one world controller");
        }
        else Instance = this;

        // create world according to the world size
        World = new World((int)GameManager.Instance.PlayableArea.x, (int)GameManager.Instance.PlayableArea.y);

        // TODO: Bad implementation.


        // Get the camera to middle of the world
        Camera.main.transform.position = new Vector3(World.Width / 2,
                World.Height / 2, Camera.main.transform.position.z);

        CreateCharacters(0, new Vector2(World.Width + 10, World.Height / 2));
    }
    
    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        var x = Mathf.FloorToInt(coordinates.x);
        var y = Mathf.FloorToInt(coordinates.y);

        return World.GetTileAt(x, y);

    }
    public void CreateCharacters(int jobType, Vector2 position)
    {

        Vector2 characterSpawnPosition = position;
        
        while (World.Characters.Count < GameManager.Instance.NumOfWorkersConstruction)
        {
            //Instantiate worker at character spawn position
            GameObject worker = Instantiate(workerPrefab, characterSpawnPosition, Quaternion.identity);
            worker.tag = "Worker";
            World.Characters.Add(worker.GetComponent<Character>());
        }
        
        
    }
}
