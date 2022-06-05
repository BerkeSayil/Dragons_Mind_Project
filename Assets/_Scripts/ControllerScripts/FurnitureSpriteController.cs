
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        Sprite[] sprites = Resources.LoadAll<Sprite>("SpriteFinal");
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
        
        if(furnGo != null && furnGo.GetComponent<SpriteRenderer>() != null && furnGo.GetComponent<SpriteRenderer>().sprite != null) {
            MakeWallResponsive(furnGo, furn);
        }
        
        
    }

    private void MakeWallResponsive(GameObject rootGo, Furniture rootFurn)
    {
        Tile southRootTile = World.GetTileAt(rootFurn.Tile.x, rootFurn.Tile.y).South();

        if (southRootTile == null || southRootTile.Furniture == null ||
            southRootTile.Furniture.ObjectType != rootFurn.ObjectType)
        {
            return;
        }
        
        


        SpriteRenderer rootRenderer = rootGo.GetComponent<SpriteRenderer>();
        if (rootRenderer.sprite.name == "Wall_E_S" )
        {
           
            GameObject furnGoChild1 = new GameObject();
            GameObject furnGoChild2 = new GameObject();
            furnGoChild1.transform.position = new Vector2(rootFurn.Tile.x, rootFurn.Tile.y + 1);
            furnGoChild1.transform.SetParent(rootGo.transform, true);

            furnGoChild2.transform.position = new Vector2(rootFurn.Tile.x, rootFurn.Tile.y);
            furnGoChild2.transform.SetParent(rootGo.transform, true);
            
            Sprite cornerSprite = furnitureSprites["Wall_S_E"];

            furnGoChild1.AddComponent<SpriteRenderer>().sprite = cornerSprite;
            furnGoChild1.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";
            furnGoChild1.GetComponent<SpriteRenderer>().sortingOrder = 10;
            
            Sprite connectionSprite = furnitureSprites["Wall_NS_E"];

            furnGoChild2.AddComponent<SpriteRenderer>().sprite = connectionSprite;
            furnGoChild2.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";
            furnGoChild2.GetComponent<SpriteRenderer>().sortingOrder = 10;
            
            
        }else if (rootRenderer.sprite.name == "Wall_W_S")
        {
           
            GameObject furnGoChild1 = new GameObject();
            GameObject furnGoChild2 = new GameObject();
            furnGoChild1.transform.position = new Vector2(rootFurn.Tile.x, rootFurn.Tile.y + 1);
            furnGoChild1.transform.SetParent(rootGo.transform, true);

            furnGoChild2.transform.position = new Vector2(rootFurn.Tile.x, rootFurn.Tile.y);
            furnGoChild2.transform.SetParent(rootGo.transform, true);
            
            Sprite cornerSprite = furnitureSprites["Wall_S_W"];

            furnGoChild1.AddComponent<SpriteRenderer>().sprite = cornerSprite;
            furnGoChild1.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";
            furnGoChild1.GetComponent<SpriteRenderer>().sortingOrder = 10;
            
            Sprite connectionSprite = furnitureSprites["Wall_NS_W"];

            furnGoChild2.AddComponent<SpriteRenderer>().sprite = connectionSprite;
            furnGoChild2.GetComponent<SpriteRenderer>().sortingLayerName = "Furniture";
            furnGoChild2.GetComponent<SpriteRenderer>().sortingOrder = 10;


        }else if (rootRenderer.sprite.name == "Wall_EW_S")
        {
            
            // destroy child gameobjects if they exist
            if (rootGo.transform.childCount > 0)
            {
                Destroy(rootGo.transform.GetChild(0).gameObject);
                Destroy(rootGo.transform.GetChild(1).gameObject);
            }
        }
        
        
    }
    
    private void OnFurnitureRemoved(Furniture furn) {
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
        
        // Create a visual gameobject 
        GameObject furnGo = new GameObject();

        furnitureGameObjectMap.Add(furn, furnGo);


        furnGo.name = furn.ObjectType + "_" + furn.Tile.x + "_" + furn.Tile.y;
        furnGo.transform.position = new Vector2(furn.Tile.x, furn.Tile.y);
        furnGo.transform.SetParent(this.transform, true);

        BoxCollider2D furnGoCollider = furnGo.AddComponent<BoxCollider2D>();

        furnGoCollider.size = new Vector2 (furn.Width, furn.Height);

        furnGoCollider.offset = new Vector2(0.5f, 0.5f);
        
        Sprite sprite = GetSpriteForFurniture(furn);
        
        SpriteRenderer furnSprite = furnGo.AddComponent<SpriteRenderer>();
        
        furnSprite.sprite = sprite;

        if(furnGo != null && furnGo.GetComponent<SpriteRenderer>() != null && furnGo.GetComponent<SpriteRenderer>().sprite != null) {
            MakeWallResponsive(furnGo, furn);
        }

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

    public Sprite GetSpriteForFurniture(Furniture furn)
    {
        if(furn.LinksToNeighboor == false) {
            return furnitureSprites[furn.ObjectType];
        }
        else
        {
            // it changes with neighboors so check that.
            string objectNameConvention = furn.ObjectType + "_";

            // Check N S E W neighboors
            int x = furn.Tile.x;
            int y = furn.Tile.y;

            //TODO: When tech is added, this will need to be changed to check for tech.
            if (furn.ObjectType == "Wall")
            {
                Tile north = null;
                Tile south = null;
                Tile east = null;
                Tile west = null;

                if (World.GetTileAt(x + 1, y) != null)
                {
                    east = World.GetTileAt(x + 1, y);
                }

                if (World.GetTileAt(x - 1, y) != null)
                {
                    west = World.GetTileAt(x - 1, y);
                }

                if (World.GetTileAt(x, y + 1) != null)
                {
                    north = World.GetTileAt(x, y + 1);
                }

                if (World.GetTileAt(x, y - 1) != null)
                {
                    south = World.GetTileAt(x, y - 1);
                }

                
                // Horizontal Walls
                if (east != null && east.Furniture != null && east.Furniture.ObjectType == furn.ObjectType) {
                    if (west != null && west.Furniture != null && west.Furniture.ObjectType == furn.ObjectType) {
                        
                        objectNameConvention += "EW";
                        
                        if (south != null && south.Type == Tile.TileType.Floor)
                        {
                            objectNameConvention += "_S";
                            //Wall_EW_S
                        }
                        else
                        {
                            objectNameConvention += "_N";
                            //Wall_EW_N
                        }
                    
                    }
                    else
                    {
                        objectNameConvention += "E";

                        if (south != null && south.Type == Tile.TileType.Floor)
                        {
                            objectNameConvention += "_S";
                            
                            //Wall_E_S
                        }
                        else
                        {
                            objectNameConvention += "_E";
                            //Wall_E_E
                            
                            
                        }
                    }
                    
                    if (furnitureSprites.ContainsKey(objectNameConvention) == false)
                    {
                        Debug.Log("No sprite with name " + objectNameConvention);
                        return null;
                    }
        
                    return furnitureSprites[objectNameConvention];
                }  else if (west != null && west.Furniture != null && west.Furniture.ObjectType == furn.ObjectType)
                {

                    objectNameConvention += "W";
                    //Wall_W

                    if (south != null && south.Type == Tile.TileType.Floor)
                    {
                        objectNameConvention += "_S";
                        //Wall_W_S
                    }else
                    {
                        objectNameConvention += "_W";
                        //Wall_W_W
                    }
                    if (furnitureSprites.ContainsKey(objectNameConvention) == false)
                    {
                        Debug.Log("No sprite with name " + objectNameConvention);
                        return null;
                    }
        
                    return furnitureSprites[objectNameConvention];
                }               
                
                else //Vertical Walls 
                if(north != null && north.Furniture != null && north.Furniture.ObjectType == furn.ObjectType) {
                    
                    objectNameConvention += "N";
                    
                    //Wall_N
                    if (south != null && south.Furniture != null && south.Furniture.ObjectType == furn.ObjectType)
                    {
                        objectNameConvention += "S";
                        //Wall_NS
                        
                    }
                    
                    else if (east != null && east.Furniture != null && east.Furniture.ObjectType == furn.ObjectType)
                    {
                        objectNameConvention += "E";
                        //Wall_NE
                    }
                    else if (west != null && west.Furniture != null && west.Furniture.ObjectType == furn.ObjectType)
                    {
                        objectNameConvention += "W";
                        //Wall_NW
                    }
                    
                    if (east != null && east.Type == Tile.TileType.Floor)
                    {
                        objectNameConvention += "_E";
                        //Wall_N_E
                        //Wall_NS_E , Wall_NE_E 
                        //Wall_NW_E
                    }
                    else
                    {
                        objectNameConvention += "_W";
                        //Wall_N_W
                        //Wall_NS_W , Wall_NE_W 
                        //Wall_NW_W
                    }
                    
                    
                    if (furnitureSprites.ContainsKey(objectNameConvention) == false)
                    {
                        Debug.Log("No sprite with name " + objectNameConvention);
                        return null;
                    }
        
                    return furnitureSprites[objectNameConvention];
                }
                
            }

            if (furnitureSprites.ContainsKey(objectNameConvention) == false)
            {
                Debug.Log("No sprite with name " + objectNameConvention);
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

    
