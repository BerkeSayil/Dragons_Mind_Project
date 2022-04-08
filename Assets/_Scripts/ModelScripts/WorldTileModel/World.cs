using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    List<Character> characters;
    public List<Room> rooms;
    public List<Designation> designations;

    Dictionary<string, Furniture> furniturePrototypes;
    Dictionary<string, Inventory> inventoryPrototypes;

    public int width { get; }
    public int height { get; }

    public GameObject characterPrefab { get; set; }

    Action<Furniture> cbFurnitureCreated;
    Action<Furniture> cbFurnitureChanged;

    Action<Inventory> cbInventoryCreated;

    Action<Tile> cbTileChanged;
    Action<GameObject> cbCharacterCreated;
    Action<Designation> cbDesigChanged;
    //TODO: 1000 tane callback var aq arasira sil bunlari

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

        designations = new List<Designation>();

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                tiles[x, y].room = GetOutsideRoom(); // they all belong to outside at start
                tiles[x, y].designationType = GetDefaultDesignation(); // all tiles are designated as empty at first
            }
        }

        CreateFurniturePrototypes();
        CreateInventoryPrototypes();

        jobQueue = new JobQueue();

        // Creates characters for us. 
        characters = new List<Character>();

        
    }
    public void AddRoom(Room r) {
        rooms.Add(r);
    }
    public void AddDesignation(Designation d) {
        designations.Add(d);
        d.RegisterDesignationTypeChangedCallback(OnDesignationChanged);
    }

    public void RemoveDesignation(Designation d) { //TODO: Designation removing should exist in a capacity
        designations.Remove(d);
    }
    public Designation.DesignationType GetDefaultDesignation() {

        return Designation.DesignationType.None;
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
    //TODO: This is not how we want to create characters
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
            1, // passible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            true //TODO: it actually need to check if closed ?)
            ));
        furniturePrototypes.Add("SleepingPod",
            Furniture.CreatePrototype(
            "SleepingPod",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            false // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("FoodProcesser",
            Furniture.CreatePrototype(
            "FoodProcesser",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            false // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("Desk",
            Furniture.CreatePrototype(
            "Desk",
            0, // impassible
            2, //width
            1, //height
            false, //TODO: This could look like linked depending on the sprites we use
            false // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("Engine",
            Furniture.CreatePrototype(
            "Engine",
            0, // impassible
            2, //width
            2, //height
            false, // links to neighboors to look like linked.
            false // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("LifeSupportMaintainer",
            Furniture.CreatePrototype(
            "LifeSupportMaintainer",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            false // can this be used as exterior blockadge ?
            ));
        furniturePrototypes.Add("AtmosProvider",
            Furniture.CreatePrototype(
            "AtmosProvider",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighboors to look like linked.
            false // can this be used as exterior blockadge ?
            ));


    }

    protected void CreateInventoryPrototypes() {
        inventoryPrototypes = new Dictionary<string, Inventory>();

        // TODO: Make this get read from a data, XML, json or some other file
        inventoryPrototypes.Add("Wall_Scrap",
            Inventory.CreateInventoryProto(
            "Wall_Scrap", // what the object is
            12, // how much are there in this stack
            12 // how much can there be for this tile
            ));
       

    }

    public void SetUpExampleStation() {

        //TODO: Debug 

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

    public void PlaceTileAt(Tile.TileType tileType, Tile t) {
        
        if(t.Type == Tile.TileType.Floor) {
            if(tileType == Tile.TileType.Empty) {

                PlaceInventory(tileType.ToString(), t);
            }
        }


        t.Type = tileType;
        
        

    }

    public void RemoveFurnitureAt(string objectType, Tile t) {
        //TODO: Remove furniture create inventory type object in its place

        if (furniturePrototypes.ContainsKey(objectType) == false) {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }

        // bu tiledaki bu obje furnitureini kaldir.
        // tile in 4 yanini kontrol et + bu 4 tile icin furniturechange callback at komsularinin gitmesine gore duzelsinler
        // 4 yanda room u null olanlari discard et
        // kalan odalardan index numarasi kucuk olani bul 
        // index numarasi buyuk olan odalardaki butun tile lari da kucuk olanin tilelari arasina kat birlesmis olsunlar
        // buyuk indexli artik ici bos odalari dunya listesinden sil

        Furniture whatFurnitureWas = furniturePrototypes[objectType];
        Furniture.DismantleFurniture(furniturePrototypes[objectType], t);

        //TODO: We don't care about it being multiple tile when generating deconstructed inventory also don't think we should tbh ?


        PlaceInventory(objectType, t);


        List<Room> neighboorRooms = new List<Room>();

        int northIndex = int.MaxValue;
        int southIndex = int.MaxValue;
        int eastIndex = int.MaxValue;
        int westIndex = int.MaxValue;

        // if this is an exterior tile we need to recalculate rooms
        if (whatFurnitureWas.stationExterior) {

            if (t.North().room != null) {
                northIndex = rooms.IndexOf(t.North().room);
                neighboorRooms.Add(rooms[northIndex]);
            }
            if (t.South().room != null) {
                southIndex = rooms.IndexOf(t.South().room);
                neighboorRooms.Add(rooms[southIndex]);
            }
            if (t.East().room != null) {
                eastIndex = rooms.IndexOf(t.East().room);
                neighboorRooms.Add(rooms[eastIndex]);
            }
            if (t.West().room != null) {
                westIndex = rooms.IndexOf(t.West().room);
                neighboorRooms.Add(rooms[westIndex]);
            }

        }

        List<int> minIndexList = new List<int>();
        minIndexList.Add(northIndex);
        minIndexList.Add(southIndex);
        minIndexList.Add(eastIndex);
        minIndexList.Add(westIndex);

        //TODO: what should we do if we remove something and a neighbooring furniture is present ?
        minIndexList.RemoveAll((int i) => i == -1); // This removes nearby furnitures so they don't conflict

        int minIndex = minIndexList.Any() ? minIndexList.Min() : -1; // if there is any ? for true get min else(:) get -1

        Room minIndexedRoom = rooms[minIndex];

        if(neighboorRooms != null) {
            
            Room.DoReverseFloodFillRoom(neighboorRooms, minIndexedRoom);
        }

        minIndexedRoom.AssignTile(t);


    }

    private void PlaceInventory(string objectType, Tile t) {

        string s = "Wall_Scrap";

        Inventory inv = Inventory.PlaceInstance(inventoryPrototypes[s], t);

        if (cbInventoryCreated != null) {
            cbInventoryCreated(inv);
        }

        if (t.looseObject != null) {
            // inventory exists.

            
                Job j = new Job(t, s, (theJob) => {
                    WorldController.Instance.world.
                    HaulInventory(s, theJob.tile);

                    t.pendingHaulJob = null;
                },
                Job.JobType.ConstructionSecond
                );


                // TODO: This being this way very easy to clear or forget make it automated in
                // some other way possible
                t.pendingHaulJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingHaulJob = null; });

                WorldController.Instance.world.jobQueue.Enqueue(j);



            }                 

        }

    private void HaulInventory(string objectType, Tile tile) {

        if (inventoryPrototypes.ContainsKey(objectType) == false) {
            Debug.LogError("inventoryPrototypes doesn't contain key: " + objectType);
            return;
        }


        Inventory whatInventoryWas = inventoryPrototypes[objectType];
        Inventory.DismantleFurniture(inventoryPrototypes[objectType], tile);


    }

    public void PlaceFurnitureAt(string objectType, Tile t)
    {
        // if width or height bigger than 1 on world call PlaceFurnitureAt for those other tiles too

        if (furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }
        Furniture furniture = Furniture.PlaceInstance(furniturePrototypes[objectType], t);

        // should this type of furniture create furniture instances on multiple tiles ?
        List<Furniture> furnitures = new List<Furniture> {
            furniture
        };

        // not my produest hard code but it works ?
        // fuck yeah it works :D

        switch (furniturePrototypes[objectType].width) {
            case 1:
                switch (furniturePrototypes[objectType].height) {
                    case 1:
                        // already handling as default
                        break;
                    case 2:
                        Furniture f2x1c1c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        furnitures.Add(f2x1c1c2);
                        break;
                    case 3:
                        Furniture f2x1c1c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        Furniture f3x1c1c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North());
                        furnitures.Add(f2x1c1c3);
                        furnitures.Add(f3x1c1c3);
                        break;
                }
                break;
            case 2:
                switch (furniturePrototypes[objectType].height) {
                    case 1:
                        Furniture f1x2c2c1 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        furnitures.Add(f1x2c2c1);
                        break;
                    case 2:
                        Furniture f1x2c2c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        Furniture f2x1c2c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        Furniture f2x2c2c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().East());

                        furnitures.Add(f1x2c2c2);
                        furnitures.Add(f2x1c2c2);
                        furnitures.Add(f2x2c2c2);
                        break;
                    case 3:
                        Furniture f1x2c2c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        Furniture f2x1c2c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        Furniture f2x2c2c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().East());
                        Furniture f3x1c2c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North());
                        Furniture f3x2c2c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North().East());

                        furnitures.Add(f1x2c2c3);
                        furnitures.Add(f2x1c2c3);
                        furnitures.Add(f2x2c2c3);
                        furnitures.Add(f3x1c2c3);
                        furnitures.Add(f3x2c2c3);

                        break;
                }
                break;
            case 3:
                switch (furniturePrototypes[objectType].height) {
                    case 1:
                        Furniture f1x2c3c1 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        Furniture f1x3c3c1 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East().East());
                        furnitures.Add(f1x2c3c1);
                        furnitures.Add(f1x3c3c1);

                        break;
                    case 2:
                        Furniture f1x2c3c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        Furniture f2x1c3c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        Furniture f2x2c3c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().East());
                        Furniture f1x3c3c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East().East());
                        Furniture f2x3c3c2 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East().East().North());

                        furnitures.Add(f1x2c3c2);
                        furnitures.Add(f2x1c3c2);
                        furnitures.Add(f2x2c3c2);
                        furnitures.Add(f1x3c3c2);
                        furnitures.Add(f2x3c3c2);
                        break;
                    case 3:
                        Furniture f1x2c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East());
                        Furniture f2x1c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North());
                        Furniture f2x2c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().East());
                        Furniture f1x3c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East().East());
                        Furniture f2x3c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.East().East().North());
                        Furniture f3x1c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North());
                        Furniture f3x2c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North().East());
                        Furniture f3x3c3c3 = Furniture.PlaceInstance(furniturePrototypes[objectType], t.North().North().East().East());

                        furnitures.Add(f1x2c3c3);
                        furnitures.Add(f2x1c3c3);
                        furnitures.Add(f2x2c3c3);
                        furnitures.Add(f1x3c3c3);
                        furnitures.Add(f2x3c3c3);
                        furnitures.Add(f3x1c3c3);
                        furnitures.Add(f3x2c3c3);
                        furnitures.Add(f3x3c3c3);
                        break;
                }
                break;
            
        }

        foreach (Furniture furn in furnitures) {

            if (furn == null) {
                //There was something already there probly so no cally cally the callback
                return;
            }

            // Should we recalculate our rooms ?
            if (furn.stationExterior) {
                Room.DoFloodFillRoom(furn);
            }

            if (cbFurnitureCreated != null) {
                cbFurnitureCreated(furn);
            }

        }

        

    }
    public bool IsFurniturePlacementValid(string furnitureType, Tile tile) {
        
        return furniturePrototypes[furnitureType].ValidatePositionOfFurniture(tile);

    }

    public bool IsTilePlacementValid(Tile.TileType tileType, Tile tile) {
        return tile.ValidateTileChange(tileType);

    }

    public void RegisterDesignationChanged(Action<Designation> callbackFunc) {
        cbDesigChanged += callbackFunc;
    }
    public void UnregisterDesignationChanged(Action<Designation> callbackFunc) {
        cbDesigChanged -= callbackFunc;
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
    public void RegisterInventoryCreated(Action<Inventory> callbackFunc) {
        cbInventoryCreated += callbackFunc;
    }
    public void UnregisterInventoryCreated(Action<Inventory> callbackFunc) {
        cbInventoryCreated -= callbackFunc;
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

    private void OnDesignationChanged(Designation d) {
        
        if(cbDesigChanged == null) {
            return;
        }

        cbDesigChanged(d);
    }

    public Furniture GetFurniturePrototype(string objectType) {

        if(furniturePrototypes.ContainsKey(objectType) == false) {

            Debug.Log("furniturePrototypes doesn't contain the key");
            return null;
        }
        return furniturePrototypes[objectType];
    }


}
