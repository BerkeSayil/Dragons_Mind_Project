using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World
{

    Tile[,] Tiles;
    List<Character> Characters;
    public List<WorkerAI> Workers { get; protected set; }
    List<VisitorAI> Engineers;

    public List<Room> Rooms;
    public List<Designation> Designations;

    public Dictionary<string, Furniture> FurniturePrototypes { get; private set; }
    public Dictionary<string, Inventory> InventoryPrototypes { get; private set; }

    public int Width { get; }
    public int Height { get; }

    public GameObject WorkerPrefab { get; set; }
    public GameObject EngineerPrefab { get; set; }
    public GameObject ShipPrefab { get; set; }
    public Vector2 ShipTilePos;

    private Action<Tile> _cbTileChanged;
    private Action<Furniture> _cbFurnitureCreated;
    private Action<Inventory> _cbInventoryCreated;
    private Action<GameObject> _cbCharacterCreated;
    private Action<Designation> _cbDesigChanged;
    //TODO: When and if we should unregister these callbacks ?

    // TODO: This should be replaced with a dedicated class for managing job queues (plural!!!)
    // might look into making it semi-static or self initializing... (don't know what they mean but I saw it online lol :d)
    public JobQueue JobQueue;
    

    public World(int width = 128, int height = 128)
    {
        this.Width = width;
        this.Height = height;

        // room system list and first room as outside
        Rooms = new List<Room>();
        Rooms.Add(new Room()); // Creates the outside?

        Designations = new List<Designation>();

        Tiles = new Tile[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                Tiles[x, y] = new Tile(this, x, y);
                Tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                Tiles[x, y].room = GetOutsideRoom(); // they all belong to outside at start
                Tiles[x, y].designationType = GetDefaultDesignation(); // all tiles are designated as empty at first
            }
        }

        // This loads preset furniture and inventory info into their dictionaries
        CreateFurniturePrototypes();
        CreateInventoryPrototypes();

        // Get the ship tile center position to spawn crewmates
        ShipTilePos = new Vector2(width / 2 + 9.5f, height / 2);

        JobQueue = new JobQueue();

        // Creates characters for us. 
        Characters = new List<Character>();
        Workers = new List<WorkerAI>();
        Engineers = new List<VisitorAI>();

    }

    public void ReturnToSender(Job job) {
        
        //TODO: this deletes given jobs from job queue destroys game-objects and everything about them and later requeue


    }

    public void DeliverShipToWorld() {

        //TODO: Make our ship get cooler as our reputation increases.
        

        GameObject c = GameObject.Instantiate(ShipPrefab, ShipTilePos, Quaternion.identity);
        c.tag = "SpaceShip";

        MotherShip shipScript = c.GetComponent<MotherShip>();

        List<Tile> shipCargoBay = new List<Tile>();

        // 71-58, 71-68, 75-68, 75-58
        for (var x = 71; x <= 75; x++) {

            for (var y = 58; y <= 68; y++) {

                if ((int)ShipTilePos.x  == x && (int)ShipTilePos.y  == y) continue;

                Tile t = GetTileAt(x, y);

                shipCargoBay.Add(t);

                t.isSpaceShip = true;
            }
        }


        Designation cargoBay = new Designation(shipCargoBay, Designation.DesignationType.TradeGoods);

    }

    internal void DeliverInventoryOnTile(Tile.TileType jobTileType, Tile destination) {

        //TODO: Consider objectType for this string
        string s = "Wall_Scrap";

        Inventory inv = Inventory.PlaceInstance(InventoryPrototypes[s], destination);

        if (inv == null) return;

        _cbInventoryCreated?.Invoke(inv);
    }

    internal void DeliverInventoryOnTile(string jobObjectType, Tile destination) {

        //TODO: Consider objectType
        string s = "Wall_Scrap";

        Inventory inv = Inventory.PlaceInstance(InventoryPrototypes[s], destination);

        if (inv == null) return;

        _cbInventoryCreated?.Invoke(inv);
    }

    public void AddRoom(Room r) {
        Rooms.Add(r);
    }
    public void AddDesignation(Designation d) {
        Designations.Add(d);
        d.RegisterDesignationTypeChangedCallback(OnDesignationChanged);
    }

    public void RemoveDesignation(Designation d) { //TODO: Designation removing should exist in a capacity
        Designations.Remove(d);
    }
    private static Designation.DesignationType GetDefaultDesignation() {

        return Designation.DesignationType.None;
    }

    public List<Designation> GetDesignationOfType(Designation.DesignationType typeWanted) {
      
        return Designations.Any() ? Designations.FindAll((type) => type.Type == typeWanted ) : null;

    }
    public Room GetOutsideRoom() {

        return Rooms[0];
    }
    public void DeleteRooms(Room r) {
        if(r == GetOutsideRoom()) {
            return;
        }

        Rooms.Remove(r);

        r.UnassignAllTiles();
        

    }
    private void CreateFurniturePrototypes()
    {
        FurniturePrototypes = new Dictionary<string, Furniture>();

        // TODO: Make this get read from a data, XML, json or some other file
        FurniturePrototypes.Add("Wall",
            Furniture.CreatePrototype( 
            "Wall", 
            0, // impassible
            1, //width
            1, //height
            true, // links to neighbours to look like linked.
            true // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("Door",
            Furniture.CreatePrototype(
            "Door",
            1, // passable
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            true //TODO: it actually need to check if closed ?)
            ));
        FurniturePrototypes.Add("SleepingPod",
            Furniture.CreatePrototype(
            "SleepingPod",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("FoodProcessor",
            Furniture.CreatePrototype(
            "FoodProcessor",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("Desk",
            Furniture.CreatePrototype(
            "Desk",
            0, // impassible
            2, //width
            1, //height
            false, //TODO: This could look like linked depending on the sprites we use
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("Engine",
            Furniture.CreatePrototype(
            "Engine",
            0, // impassible
            2, //width
            2, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("LifeSupportMaintainer",
            Furniture.CreatePrototype(
            "LifeSupportMaintainer",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("AtmosProvider",
            Furniture.CreatePrototype(
            "AtmosProvider",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));


    }

    private void CreateInventoryPrototypes() {
        InventoryPrototypes = new Dictionary<string, Inventory>();

        // TODO: Make this get read from a data, XML, json or some other file
        InventoryPrototypes.Add("Wall_Scrap",
            Inventory.CreateInventoryProto(
            "Wall_Scrap" // what the object is
            ));


    }
    public Tile GetTileAt(int x, int y)
    {
        if (x >= 0 && x <= Width && y >= 0 && y <= Height) return Tiles[x, y];
        
        // TODO: clean this later
        Debug.LogError("Tile (" + x + " , " + y + ") is out of range for this world.");
        return null;

    }

    public bool IsFurniturePlacementValid(string furnitureType, Tile tile) {
        // return if you could place that furniture on that tile
        return FurniturePrototypes[furnitureType].ValidatePositionOfFurniture(tile);

    }

    public bool IsTilePlacementValid(Tile.TileType tileType, Tile tile) {
        // returns if you're changing the tile : true if you send different tile type
        return tile.ValidateTileChange(tileType);

    }

    public void PlaceTileAt(Tile.TileType tileType, Tile t) {

        if (t.Type == Tile.TileType.Floor) {
            if (tileType == Tile.TileType.Empty) {

                DropInventoryAfterDeconstruction(tileType.ToString(), t);
            }
        }
        t.Type = tileType;
        
    }

    private void DropInventoryAfterDeconstruction(string objectType, Tile t) {

        //TODO: Consider objectType
        string s = "Wall_Scrap";
        
        Inventory inv = Inventory.PlaceInstance(InventoryPrototypes[s], t);

        if (inv == null) return;

        _cbInventoryCreated?.Invoke(inv);

        if (t.looseObject == null) return;
        // inventory exists on this tile.

        Job j = new Job(t, false, inv ,(theJob) => {
            GetTheInventory(s, theJob.Tile);

            t.pendingHaulJob = null;

        }, Job.JobType.InventoryManagement );

        t.pendingHaulJob = j;
        j.RegisterJobCancelCallback((theJob) => { theJob.Tile.pendingHaulJob = null; });
            
        WorldController.Instance.World.JobQueue.Enqueue(j);

    }
    
    private void DropOffInventoryAtHaulingEnd(string objectType, Tile tile) {

        Inventory inv = Inventory.PlaceInstance(InventoryPrototypes[objectType], tile);

        if (inv == null) return;

        _cbInventoryCreated?.Invoke(inv);

        //TODO: Keep track of inventories that exist in current word so we only create if no exists
        // .Add(inv);
    }
    private void GetTheInventory(string objectType, Tile tile) {

        if (InventoryPrototypes.ContainsKey(objectType) == false) {
            Debug.LogError("inventoryPrototypes doesn't contain key: " + objectType);
            return;
        }

        List<Designation> desigs = GetDesignationOfType(Designation.DesignationType.TradeGoods);

        Tile destination = tile; // at worst we'll put where we found it ?

        // now that the inventory is removed from ground
        // we'll create a job only the worker that took this could complete

        if (desigs == null) return;

        foreach (var t1 in desigs)
        {
            foreach (var t in t1.Tiles.Where
                         (t => t.looseObject == null && t.pendingHaulJob == null && IsHaulPlacementValid(t)))
            {
                destination = t;
                break;
            }

            if (destination != tile) break;
        }

        if (destination == tile){
            DropInventoryAfterDeconstruction(objectType, tile);
            return;
        }

        Job j = new Job(destination, true, InventoryPrototypes[objectType] , (theJob) => {
            DropOffInventoryAtHaulingEnd(objectType, theJob.Tile);

            destination.pendingHaulJob = null;

        }, Job.JobType.InventoryManagement);

        destination.pendingHaulJob = j;
        j.RegisterJobCancelCallback((theJob) => { theJob.Tile.pendingHaulJob = null; });

        WorldController.Instance.World.JobQueue.Enqueue(j);

        Inventory.PickInventoryUp(tile); // removes inventory on given tile

    }


    public bool IsHaulPlacementValid(Tile destination) {
        if (destination.furniture == null) {
            return true;
        }
        else return false;
    }

    public void PlaceFurnitureAt(string objectType, Tile t)
    {
        // if width or height bigger than 1 on world call PlaceFurnitureAt for those other tiles too

        if (FurniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }
        Furniture furniture = Furniture.PlaceInstance(FurniturePrototypes[objectType], t);

        // should this type of furniture create furniture instances on multiple tiles ?
        List<Furniture> furnitures = new List<Furniture> {
            furniture
        };

        // not my produest hard code but it works ?
        // fuck yeah it works :D

        switch (FurniturePrototypes[objectType].width) {
            case 1:
                switch (FurniturePrototypes[objectType].height) {
                    case 1:
                        // already handling as default
                        break;
                    case 2:
                        Furniture f2x1c1c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        furnitures.Add(f2x1c1c2);
                        break;
                    case 3:
                        Furniture f2x1c1c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        Furniture f3x1c1c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North());
                        furnitures.Add(f2x1c1c3);
                        furnitures.Add(f3x1c1c3);
                        break;
                }
                break;
            case 2:
                switch (FurniturePrototypes[objectType].height) {
                    case 1:
                        Furniture f1x2c2c1 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        furnitures.Add(f1x2c2c1);
                        break;
                    case 2:
                        Furniture f1x2c2c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        Furniture f2x1c2c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        Furniture f2x2c2c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().East());

                        furnitures.Add(f1x2c2c2);
                        furnitures.Add(f2x1c2c2);
                        furnitures.Add(f2x2c2c2);
                        break;
                    case 3:
                        Furniture f1x2c2c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        Furniture f2x1c2c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        Furniture f2x2c2c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().East());
                        Furniture f3x1c2c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North());
                        Furniture f3x2c2c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North().East());

                        furnitures.Add(f1x2c2c3);
                        furnitures.Add(f2x1c2c3);
                        furnitures.Add(f2x2c2c3);
                        furnitures.Add(f3x1c2c3);
                        furnitures.Add(f3x2c2c3);

                        break;
                }
                break;
            case 3:
                switch (FurniturePrototypes[objectType].height) {
                    case 1:
                        Furniture f1x2c3c1 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        Furniture f1x3c3c1 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East().East());
                        furnitures.Add(f1x2c3c1);
                        furnitures.Add(f1x3c3c1);

                        break;
                    case 2:
                        Furniture f1x2c3c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        Furniture f2x1c3c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        Furniture f2x2c3c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().East());
                        Furniture f1x3c3c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East().East());
                        Furniture f2x3c3c2 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East().East().North());

                        furnitures.Add(f1x2c3c2);
                        furnitures.Add(f2x1c3c2);
                        furnitures.Add(f2x2c3c2);
                        furnitures.Add(f1x3c3c2);
                        furnitures.Add(f2x3c3c2);
                        break;
                    case 3:
                        Furniture f1x2c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East());
                        Furniture f2x1c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North());
                        Furniture f2x2c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().East());
                        Furniture f1x3c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East().East());
                        Furniture f2x3c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.East().East().North());
                        Furniture f3x1c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North());
                        Furniture f3x2c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North().East());
                        Furniture f3x3c3c3 = Furniture.PlaceInstance(FurniturePrototypes[objectType], t.North().North().East().East());

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

            if (_cbFurnitureCreated != null) {
                _cbFurnitureCreated(furn);
            }

        }

    }
    public void RemoveFurnitureAt(string objectType, Tile t) {

        if (FurniturePrototypes.ContainsKey(objectType) == false) {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }

        //TODO: Multiple tile deconstruction not supported
        Furniture whatFurnitureWas = FurniturePrototypes[objectType];
        Furniture.DismantleFurniture(FurniturePrototypes[objectType], t);

        DropInventoryAfterDeconstruction(objectType, t);
        

        // if this is an exterior tile we need to recalculate rooms
        if (whatFurnitureWas.stationExterior == false) return;

        // if this is not an exterior we don't need to calculate rooms

        // bu tiledaki bu obje furnitureini kaldir.
        // tile in 4 yanini kontrol et + bu 4 tile icin furniturechange callback at komsularinin gitmesine gore duzelsinler
        // 4 yanda room u null olanlari discard et
        // kalan odalardan index numarasi kucuk olani bul 
        // index numarasi buyuk olan odalardaki butun tile lari da kucuk olanin tilelari arasina kat birlesmis olsunlar
        // buyuk indexli artik ici bos odalari dunya listesinden sil

        List<Room> neighboorRooms = new List<Room>();

        int northIndex = int.MaxValue;
        int southIndex = int.MaxValue;
        int eastIndex = int.MaxValue;
        int westIndex = int.MaxValue;


        if (t.North().room != null) {
            northIndex = Rooms.IndexOf(t.North().room);
            neighboorRooms.Add(Rooms[northIndex]);
        }
        if (t.South().room != null) {
            southIndex = Rooms.IndexOf(t.South().room);
            neighboorRooms.Add(Rooms[southIndex]);
        }
        if (t.East().room != null) {
            eastIndex = Rooms.IndexOf(t.East().room);
            neighboorRooms.Add(Rooms[eastIndex]);
        }
        if (t.West().room != null) {
            westIndex = Rooms.IndexOf(t.West().room);
            neighboorRooms.Add(Rooms[westIndex]);
        }

        List<int> minIndexList = new List<int>();
        minIndexList.Add(northIndex);
        minIndexList.Add(southIndex);
        minIndexList.Add(eastIndex);
        minIndexList.Add(westIndex);

        //TODO: what should we do if we remove something and a neighbooring furniture is present ?
        minIndexList.RemoveAll((i) => i == -1); // This removes nearby furnitures so they don't conflict

        int minIndex = minIndexList.Any() ? minIndexList.Min() : -1; // if there is any ? for true get min else(:) get -1

        Room minIndexedRoom = Rooms[minIndex];

        Room.DoReverseFloodFillRoom(neighboorRooms, minIndexedRoom);

        minIndexedRoom.AssignTile(t);


    }
    
    //TODO: This is not how we want to create characters
    public Character CreateCharacter(int jobType,Tile t) {
        GameObject c = null;

        switch (jobType) {
            case 0: // construction worker
                c = GameObject.Instantiate(WorkerPrefab, ShipTilePos, Quaternion.identity);
                c.tag = "Worker";
                break;

            case 1: // visitor
                c = GameObject.Instantiate(EngineerPrefab, ShipTilePos, Quaternion.identity);
                c.tag = "Visitor";
                break;

        }

        if (c == null) return null;

        Character cScript = c.GetComponent<Character>();

        Characters.Add(cScript);

        if (WorkerPrefab.GetComponent<WorkerAI>() != null) Workers.Add(WorkerPrefab.GetComponent<WorkerAI>());
        
        if (WorkerPrefab.GetComponent<VisitorAI>() != null) Engineers.Add(WorkerPrefab.GetComponent<VisitorAI>());

        if (_cbCharacterCreated != null) {
            _cbCharacterCreated(c);
        }
        return cScript;
    }
    /*
    public void SetUpExampleStation() {

        //TODO: Debug get rid of in the final build

        int l = width / 2 - 5;
        int b = height / 2 - 5;

        for (int x = l - 10; x < l + 15; x++) {
            for (int y = b - 10; y < b + 15; y++) {
                tiles[x, y].Type = Tile.TileType.Floor;

                if ((x == l + 3) || (x == (l + 4)) || (y == b - 3) || (y == (b + 4))) {
                    if (x != (l + 3) && y != (b + 9)) {
                        PlaceFurnitureAt("Wall", tiles[x, y]);
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
    */
    public void RegisterDesignationChanged(Action<Designation> callbackFunc) {
        _cbDesigChanged += callbackFunc;
    }
    public void UnregisterDesignationChanged(Action<Designation> callbackFunc) {
        _cbDesigChanged -= callbackFunc;
    }

    public void RegisterTileChanged(Action<Tile> callbackFunc) {
        _cbTileChanged += callbackFunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackFunc) {
        _cbTileChanged -= callbackFunc;
    }

    public void RegisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        _cbFurnitureCreated += callbackFunc;
    }
    public void UnregisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        _cbFurnitureCreated -= callbackFunc;
    }
    public void RegisterInventoryCreated(Action<Inventory> callbackFunc) {
        _cbInventoryCreated += callbackFunc;
    }
    public void UnregisterInventoryCreated(Action<Inventory> callbackFunc) {
        _cbInventoryCreated -= callbackFunc;
    }

    public void RegisterCharacterCreated(Action<GameObject> callbackFunc) {
        _cbCharacterCreated += callbackFunc;
    }
    public void UnregisterCharacterCreated(Action<GameObject> callbackFunc) {
        _cbCharacterCreated -= callbackFunc;
    }

    void OnTileChanged(Tile t) {
        if(_cbTileChanged == null) {
            return;
        }

        _cbTileChanged(t);
    }

    private void OnDesignationChanged(Designation d) {
        
        if(_cbDesigChanged == null) {
            return;
        }

        _cbDesigChanged(d);
    }

}
