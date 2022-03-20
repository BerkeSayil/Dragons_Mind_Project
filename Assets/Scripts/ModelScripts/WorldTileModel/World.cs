using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    List<Character> characters;
    public List<Room> rooms;

    Dictionary<string, Furniture> furniturePrototypes;

    public int width { get; }
    public int height { get; }

    public GameObject characterPrefab { get; set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;
    Action<GameObject> cbCharacterCreated;

    // TODO: This should be replaced with a dedicated
    // class for managing job queues (plural!!!)
    // might look into making it semi-static or self initializing...
    public JobQueue jobQueue;


    public World(int width = 128, int height = 128)
    {
        this.width = width;
        this.height = height;

        // room system list and first room as outside
        rooms = new List<Room>();
        rooms.Add(new Room()); // Creates the outside?

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                tiles[x, y].room = GetOutsideRoom(); // they all belong to outside at start
            }
        }

        CreateFurniturePrototypes();

        jobQueue = new JobQueue();

        // Creates characters for us. 
        characters = new List<Character>();

        
    }
    public void AddRoom(Room r) {
        rooms.Add(r);
    }

    public Room GetOutsideRoom() {

        return rooms[0];
    }
    public void DeleteRooms(Room r) {
        if(r == GetOutsideRoom()) {
            return;
        }

        rooms.Remove(r);

        r.UnassignAllTiles();
        

    }
    //TODO: FIX
    public Character CreateCharacter(Tile t) {
        GameObject c = GameObject.Instantiate(characterPrefab);
        Character cScript =c.GetComponent<Character>();
        characters.Add(cScript);
        // because we registered this cb as charactercreated this goes and call that
        // with the given variable.
        if(cbCharacterCreated != null) {
            cbCharacterCreated(c);
        }
        return cScript;
    }

    protected void CreateFurniturePrototypes()
    {
        furniturePrototypes = new Dictionary<string, Furniture>();

        // TODO: Make this get read from a data, XML, json or some other file
        furniturePrototypes.Add("Wall", 
            Furniture.CreatePrototype( 
            "Wall", 
            0, // impassible
            1, //width
            1, //height
            true, // links to neighboors to look like linked.
            true // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("Door",
            Furniture.CreatePrototype(
            "Door",
            1, // impassible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            true //TODO: it actually need to check if closed ?)
            ));

    }

    public void SetUpExampleStation() {

        Debug.Log("Example Station");

        int l = width / 2 - 5;
        int b = height / 2 - 5;

        for (int x = l - 10 ; x < l + 15; x++) {
            for (int y = b - 10; y < b + 15 ; y++) {
                tiles[x, y].Type = Tile.TileType.Floor;

                if((x == l + 3 ) || (x == (l+4)) || (y == b - 3) || (y == (b + 4))) {
                    if(x != (l + 3) && y != (b + 9)) {
                        PlaceFurnitureAt("Wall", tiles[x,y]);
                    }  
                }
                if ((x == l - 10) || (x == (l + 14)) || (y == b - 10) || (y == (b + 14))) {
                    PlaceFurnitureAt("Wall", tiles[x, y]);
                    
                }
            }
        }
        //TODO: Get rid of this while you get rid of the example station.
        GameObject.Find("A*").GetComponent<AstarPath>().Scan();


    }

    public Tile GetTileAt(int x, int y)
    {
        if(x < 0 || x > width || y < 0 || y > height)
        {
            // clean this later
            Debug.LogError("Tile (" + x + " , " + y + ") is out of range for this world.");
            return null;
        }

        return tiles[x, y];
    }

    public void PlaceFurnitureAt(string objectType, Tile t)
    {
        //TODO: We only assume 1x1 tiles change later...

        if(furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }

        Furniture furniture = Furniture.PlaceInstance(furniturePrototypes[objectType], t);
        if(furniture == null) {
            //There was something already there probly so no cally cally the callback
            return;
        }

        // Should we recalculate our rooms ?
        if (furniture.stationExterior) {
            Room.DoFloodFillRoom(furniture);
        }

        if(cbFurnitureCreated != null)
        {
            cbFurnitureCreated(furniture);
        }

    }
    public bool IsFurniturePlacementValid(string furnitureType, Tile tile) {
        return furniturePrototypes[furnitureType].ValidatePositionOfFurniture(tile);


    }
    public void RegisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged += callbackFunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged -= callbackFunc;
    }

    public void RegisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        cbFurnitureCreated += callbackFunc;
    }
    public void UnregisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        cbFurnitureCreated -= callbackFunc;
    }
    public void RegisterCharacterCreated(Action<GameObject> callbackFunc) {
        cbCharacterCreated += callbackFunc;
    }
    public void UnregisterCharacterCreated(Action<GameObject> callbackFunc) {
        cbCharacterCreated -= callbackFunc;
    }

    void OnTileChanged(Tile t) {
        if(cbTileChanged == null) {
            return;
        }

        cbTileChanged(t);
    }

    public Furniture GetFurniturePrototype(string objectType) {

        if(furniturePrototypes.ContainsKey(objectType) == false) {

            Debug.Log("furniturePrototypes doesn't contain the key");
            return null;
        }
        return furniturePrototypes[objectType];
    }


}
