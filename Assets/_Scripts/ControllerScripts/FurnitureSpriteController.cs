
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour
{
 
    private Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    private Dictionary<string, Sprite> furnitureSprites;

    private const int FurnitureLayer = 8;
    private const int ImpassibleLayer = 9;

    private static World World => WorldController.Instance.World;

    private void Start() {

        LoadSprites();

        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        World.RegisterFurnitureCreated(OnFurnitureCreated);
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

        return World.GetTileAt(x, y);

    }
    private void OnFurnitureChanged(Furniture furn) {
        // Make sure furniture sprites are correct;
        
        if(furnitureGameObjectMap.ContainsKey(furn) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGo = furnitureGameObjectMap[furn];

        SpriteRenderer furnSprite = furnGo.GetComponent<SpriteRenderer>();
        
        

        furnSprite.sprite = GetSpriteForFurniture(furn);
        furnSprite.sortingLayerName = "Furniture";
    }

    private void OnFurnitureRemoved(Furniture furn) {
        //TODO: DOESN'T CONSIDER MULTI TILE FURNITURES
        // Make sure furniture sprites are correct;

        if (furnitureGameObjectMap.ContainsKey(furn) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGo = furnitureGameObjectMap[furn];


        // gets rid of the furniture while at it and for it to work we get tile before getting rid of it
        Tile tileFurnGotRemoved = furn.Tile;
        furn.Tile.PlaceInstalledObject(null);

        // updates 4 direction furnitures to update
        if (tileFurnGotRemoved.North().Furniture != null) OnFurnitureChanged(tileFurnGotRemoved.North().Furniture);
        if (tileFurnGotRemoved.South().Furniture != null) OnFurnitureChanged(tileFurnGotRemoved.South().Furniture);
        if (tileFurnGotRemoved.East().Furniture != null) OnFurnitureChanged(tileFurnGotRemoved.East().Furniture);
        if (tileFurnGotRemoved.West().Furniture != null) OnFurnitureChanged(tileFurnGotRemoved.West().Furniture);

        furnGo.SetActive(false);
    }
    private void OnFurnitureCreated(Furniture furn) {
        //FIXME: DOES NOT consider multi tiles objects

        // Create a visual gameobject 
        GameObject furnGo = new GameObject();

        furnitureGameObjectMap.Add(furn, furnGo);


        furnGo.name = furn.ObjectType + "_" + furn.Tile.x + "_" + furn.Tile.y;
        furnGo.transform.position = new Vector2(furn.Tile.x, furn.Tile.y);
        furnGo.transform.SetParent(this.transform, true);

        furnGo.AddComponent<SpriteRenderer>().sprite = 
            GetSpriteForFurniture(furn);

        BoxCollider2D furnGoCollider = furnGo.AddComponent<BoxCollider2D>();

        furnGoCollider.size = new Vector2 (1f,1f);

        // TODO: Implement better layering system for movement cost consideration maybe ?
         // This layer is designed to be furniture layer and A* sees this as blockadge
        if(furn.MovementCost == 0) {
            furnGo.layer = ImpassibleLayer;
        }
        else {
            furnGo.layer = FurnitureLayer;
        }

        // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
        furnGo.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";


        // Whenever objects anything changes (door animatons n stuff.)
        furn.RegisterOnChangedCallback(OnFurnitureChanged);

        // Whenever object get removed
        furn.RegisterOnRemovedCallback(OnFurnitureRemoved);
    }

    public Sprite GetSpriteForFurniture(Furniture furn) {
        if(furn.LinksToNeighboor == false) {
            return furnitureSprites[furn.ObjectType];
        }
        else {
            // it changes with neighboors so check that.
            string objectNameConvention = furn.ObjectType + "_";

            // Check N S E W neighboors
            Tile t;
            int x = furn.Tile.x;
            int y = furn.Tile.y;

            t = World.GetTileAt(x, y + 1);
            if(t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
                objectNameConvention += "N";
            }

            t = World.GetTileAt(x, y - 1);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
                objectNameConvention += "S";
            }

            t = World.GetTileAt(x + 1, y); 
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
                objectNameConvention += "E";
            }

            t = World.GetTileAt(x - 1, y);
            if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
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

    
