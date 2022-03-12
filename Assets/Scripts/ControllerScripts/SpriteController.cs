using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] Sprite floorSprite;
    [SerializeField] Sprite defaultEmptySprite;

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

   World world {
        get { return WorldController.Instance.world; }
    }
    void Start() {

        LoadSprites();

        

        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
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


        world.RegisterFurnitureCreated(OnFurnitureCreated);
        world.RegisterTileChanged(OnTileChanged);
    }

    private void LoadSprites() {
        // Getting installed objects from the resources folder

        furnitureSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            furnitureSprites[s.name] = s;
        }
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
            tileGO.GetComponent<SpriteRenderer>().sprite = null;
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
    void OnFurnitureChanged(Furniture furn) {
        // Make sure furniture sprites are correct;
        if(furnitureGameObjectMap.ContainsKey(furn) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGO = furnitureGameObjectMap[furn];
        furnGO.GetComponent<SpriteRenderer>().sprite =
            GetSpriteForInstalledObject(furn);

    }

    public void OnFurnitureCreated(Furniture furn) {
        //FIXME: DOESNOT consider multi tiles objects

        // Create a visual gameobject 
        GameObject furnGO = new GameObject();

        furnitureGameObjectMap.Add(furn, furnGO);


        furnGO.name = furn.objectType + "_" + furn.tile.x + "_" + furn.tile.y;
        furnGO.transform.position = new Vector2(furn.tile.x, furn.tile.y);
        furnGO.transform.SetParent(this.transform, true);

        furnGO.AddComponent<SpriteRenderer>().sprite = 
            GetSpriteForInstalledObject(furn);

        // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
        furnGO.GetComponent<SpriteRenderer>().sortingOrder = 2;


        // Whenever objects anything changes (door animatons n stuff.)
        furn.RegisterOnChangedCallback(OnFurnitureChanged);
    }

    Sprite GetSpriteForInstalledObject(Furniture furn) {
        if(furn.linksToNeighboor == false) {
            return furnitureSprites[furn.objectType];
        }
        else {
            // it changes with neighboors so check that.
            string objectNameConvention = furn.objectType + "_";

            // Check N S E W neighboors
            Tile t;
            int x = furn.tile.x;
            int y = furn.tile.y;

            t = world.GetTileAt(x, y + 1);
            if(t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                objectNameConvention += "N";
            }

            t = world.GetTileAt(x, y - 1);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                objectNameConvention += "S";
            }

            t = world.GetTileAt(x + 1, y); 
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                objectNameConvention += "E";
            }

            t = world.GetTileAt(x - 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                objectNameConvention += "W";
            }

            // if it has all neighboors result script would look like Walls_NSEW
            if(furnitureSprites.ContainsKey(objectNameConvention) == false) {
                Debug.LogError("No sprite with name " + objectNameConvention);
                return null;
            }
            return furnitureSprites[objectNameConvention];
        }

    }

    

}
