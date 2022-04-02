using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TileSpriteController : MonoBehaviour
{
    [SerializeField] Sprite floorSprite;
    [SerializeField] Sprite defaultEmptySprite;

    Dictionary<Tile, GameObject> tileGameObjectMap;


    const int EMPTY_LAYER = 6;
    const int FLOOR_LAYER = 7;

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
                SpriteRenderer renderer = tileGO.GetComponent<SpriteRenderer>();
                renderer.sprite = defaultEmptySprite;
                renderer.sortingLayerName = "Tiles";

                // setting collisionBox layer for pathfinding.
                tileGO.layer = EMPTY_LAYER;
                BoxCollider2D collider =  tileGO.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(1f, 1f);
                

                tileGameObjectMap.Add(tileData, tileGO);

            }
        }

        world.RegisterTileChanged(OnTileChanged);
        GameObject.Find("DesignationController").
            GetComponent<DesignationController>().
            RegisterTileDesignationChangedCallback(OnTileChanged);
    }

    public Sprite GetSpriteForTile(Tile.TileType jobTileType) {
        //TODO: When you fix serialize field sprite things you will need to fix these too!

        if(jobTileType == Tile.TileType.Empty) {
            return defaultEmptySprite;
        }
        
        if(jobTileType == Tile.TileType.Floor) {
            return floorSprite;
        }

        return null; // This shouldn't come up

    }

    void OnTileChanged(Tile tileData)
    {

        if (tileGameObjectMap.ContainsKey(tileData) == false) {
            Debug.Log("tileGameObjectMap doesn't contain key");
            return;
        }

        GameObject tileGO = tileGameObjectMap[tileData];

        if(tileData.Type == Tile.TileType.Floor)
        {
            // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
            tileGO.GetComponent<SpriteRenderer>().sprite = floorSprite;
            tileGO.layer = FLOOR_LAYER;


        }
        else if(tileData.Type == Tile.TileType.Empty)
        {
            tileGO.GetComponent<SpriteRenderer>().sprite = defaultEmptySprite;
            tileGO.layer = EMPTY_LAYER;

        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }

        if(tileData.designationType != Designation.DesignationType.None) {
            if (tileData.designationType == Designation.DesignationType.Cafeteria) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
            if (tileData.designationType == Designation.DesignationType.Engine) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            if (tileData.designationType == Designation.DesignationType.Kitchen) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.magenta;
            }
            if (tileData.designationType == Designation.DesignationType.LifeSupport) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.red;
            }
            if (tileData.designationType == Designation.DesignationType.PersonalCrewRoom) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.green;
            }
            if (tileData.designationType == Designation.DesignationType.TradeGoods) {
                tileGO.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            

        }
    }

    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        int x = Mathf.FloorToInt(coordinates.x);
        int y = Mathf.FloorToInt(coordinates.y);

        return world.GetTileAt(x, y);

    }
}
