using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour
{
 
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;

    const int FURNITURE_LAYER = 8;
    const int IMPASSIBE_LAYER = 9;

    World world {
        get { return WorldController.Instance.world; }
    }
    void Start() {

        LoadSprites();

        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        world.RegisterFurnitureCreated(OnFurnitureCreated);
    }

    private void LoadSprites() {
        // Getting installed objects from the resources folder

        furnitureSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            furnitureSprites[s.name] = s;
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

        SpriteRenderer furnSprite = furnGO.GetComponent<SpriteRenderer>();
        
        

        furnSprite.sprite = GetSpriteForFurniture(furn);
        furnSprite.sortingLayerName = "Furniture";
    }

    void OnFurnitureRemoved(Furniture furn) {
        // Make sure furniture sprites are correct;

        if (furnitureGameObjectMap.ContainsKey(furn) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGO = furnitureGameObjectMap[furn];


        // gets rid of the furniture while at it and for it to work we get tile before getting rid of it
        Tile tileFurnGotRemoved = furn.tile;
        furn.tile.PlaceInstalledObject(null);

        // updates 4 direction furnitures to update
        if (tileFurnGotRemoved.North().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.North().furniture);
        if (tileFurnGotRemoved.South().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.South().furniture);
        if (tileFurnGotRemoved.East().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.East().furniture);
        if (tileFurnGotRemoved.West().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.West().furniture);

        furnGO.SetActive(false);
    }
    public void OnFurnitureCreated(Furniture furn) {
        //FIXME: DOES NOT consider multi tiles objects

        // Create a visual gameobject 
        GameObject furnGO = new GameObject();

        furnitureGameObjectMap.Add(furn, furnGO);


        furnGO.name = furn.objectType + "_" + furn.tile.x + "_" + furn.tile.y;
        furnGO.transform.position = new Vector2(furn.tile.x, furn.tile.y);
        furnGO.transform.SetParent(this.transform, true);

        furnGO.AddComponent<SpriteRenderer>().sprite = 
            GetSpriteForFurniture(furn);

        BoxCollider2D furnGOCollider = furnGO.AddComponent<BoxCollider2D>();

        // TODO: Make this understand how big a furniture is and apply with that in mind.
        furnGOCollider.size = new Vector2 (1f,1f);

        // TODO: Implement better layering system for movement cost consideration maybe ?
         // This layer is designed to be furniture layer and A* sees this as blockadge
        if(furn.movementCost == 0) {
            furnGO.layer = IMPASSIBE_LAYER;
        }
        else {
            furnGO.layer = FURNITURE_LAYER;
        }

        // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
        furnGO.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";


        // Whenever objects anything changes (door animatons n stuff.)
        furn.RegisterOnChangedCallback(OnFurnitureChanged);

        // Whenever object get removed
        furn.RegisterOnRemovedCallback(OnFurnitureRemoved);
    }

    public Sprite GetSpriteForFurniture(Furniture furn) {
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

    public Sprite GetSpriteForFurniture(string objectType) {

        if (furnitureSprites.ContainsKey(objectType)) {
            return furnitureSprites[objectType];
        }
        if (furnitureSprites.ContainsKey(objectType + "_")) {
            return furnitureSprites[objectType + "_"];
        }

        Debug.LogError("No sprite with name " + objectType);

        return null;

    }


}

    
