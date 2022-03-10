using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] Sprite floorSprite;

    public static WorldController Instance { get; protected set; } // everyone can ask for it
                                                                   // only worldcontroller set it

    public World World { get; protected set; }
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("There shouldn't be more than one worldcontroller");
        }
        else Instance = this;

        World = new World(); //default empty world

        // Create a gameobject for every tile so we have visual representation.
        for (int x = 0; x < World.width; x++)
        {
            for (int y = 0; y < World.height; y++)
            {
                Tile tileData = World.GetTileAt(x, y);

                GameObject tileGO = new GameObject(); 
                tileGO.name = "Tile_" + x + "_" + y;
                tileGO.transform.position = new Vector2(tileData.x, tileData.y);
                tileGO.transform.SetParent(this.transform, true);

                // adding a sprite renderer and leaving it null because space == empty.
                tileGO.AddComponent<SpriteRenderer>();
                

                // Whenever tile changed this function will get called
                // which in turn calls the lambda function inside.
                tileData.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tileGO); } );
            }
        }
    }


    void OnTileTypeChanged(Tile tileData, GameObject tileGO)
    {
        if(tileData.Type == Tile.TileType.Floor)
        {
            tileGO.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if(tileData.Type == Tile.TileType.Empty)
        {
            tileGO.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        int x = Mathf.FloorToInt(coordinates.x);
        int y = Mathf.FloorToInt(coordinates.y);

        return World.GetTileAt(x, y);

    }

}
