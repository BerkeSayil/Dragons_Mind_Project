using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{
    [SerializeField] Sprite floorSprite;
    [SerializeField] Sprite defaultEmptySprite;

    Dictionary<Tile, GameObject> tileGameObjectMap;

   World world {
        get { return WorldController.Instance.world; }
    }
    void Start() {

        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a gameobject for every tile so we have visual representation.
        for (int x = 0; x < world.width; x++) {
            for (int y = 0; y < world.height; y++) {
                Tile tileData = world.GetTileAt(x, y);

                GameObject tileGO = new GameObject();
                tileGO.name = "Tile_" + x + "_" + y;
                tileGO.transform.position = new Vector2(tileData.x, tileData.y);
                tileGO.transform.SetParent(this.transform, true);

                // adding a sprite renderer and giving it the defaultyEmpty sprite.
                tileGO.AddComponent<SpriteRenderer>();
                tileGO.GetComponent<SpriteRenderer>().sprite = defaultEmptySprite;

                tileGameObjectMap.Add(tileData, tileGO);

            }
        }

        world.RegisterTileChanged(OnTileChanged);
    }

    

    void OnTileChanged(Tile tileData)
    {
        if(tileGameObjectMap.ContainsKey(tileData) == false) {
            Debug.Log("tileGameObjectMap doesn't contain key");
            return;
        }

        GameObject tileGO = tileGameObjectMap[tileData];

        if(tileData.Type == Tile.TileType.Floor)
        {
            // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
            tileGO.GetComponent<SpriteRenderer>().sprite = floorSprite;
            tileGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else if(tileData.Type == Tile.TileType.Empty)
        {
            tileGO.GetComponent<SpriteRenderer>().sprite = defaultEmptySprite;
            tileGO.GetComponent<SpriteRenderer>().sortingOrder = 0;

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

        return world.GetTileAt(x, y);

    }
    

}
