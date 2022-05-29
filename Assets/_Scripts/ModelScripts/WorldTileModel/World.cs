using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World
{

    Tile[,] Tiles;
    List<Character> Characters;
    public List<WorkerAI> Workers { get; protected set; }

    public List<Room> Rooms;

    public Dictionary<string, Furniture> FurniturePrototypes { get; private set; }

    public int Width { get; }
    public int Height { get; }


    private Action<Tile> _cbTileChanged;
    private Action<Furniture> _cbFurnitureCreated;
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


        Tiles = new Tile[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                Tiles[x, y] = new Tile(this, x, y);
                Tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                Tiles[x, y].Room = GetOutsideRoom(); // they all belong to outside at start
            }
        }
        
        // This loads preset furniture and inventory info into their dictionaries
        CreateFurniturePrototypes();

        JobQueue = new JobQueue();

        // Creates characters for us. 
        Characters = new List<Character>();
        Workers = new List<WorkerAI>();

        
    }

    public void AddRoom(Room r) {
        Rooms.Add(r);
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
        FurniturePrototypes.Add("Bed01",
            Furniture.CreatePrototype(
            "Bed01",
            0, // impassible
            1, //width
            2, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("FoodDispenser",
            Furniture.CreatePrototype(
            "FoodDispenser",
            0, // impassible
            2, //width
            2, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("StorageBox",
            Furniture.CreatePrototype(
            "StorageBox",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("Engine01",
            Furniture.CreatePrototype(
            "Engine01",
            0, // impassible
            2, //width
            2, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("Thruster01",
            Furniture.CreatePrototype(
            "Thruster01",
            0, // impassible
            1, //width
            2, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));
        FurniturePrototypes.Add("OxygenGenerator",
            Furniture.CreatePrototype(
            "OxygenGenerator",
            0, // impassible
            1, //width
            1, //height
            false, // links to neighbours to look like linked.
            false // can this be used as exterior blockage ?
            ));


    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
        
        return Tiles[x, y];

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

        t.Type = tileType;
        
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

        switch (FurniturePrototypes[objectType].Width) {
            case 1:
                switch (FurniturePrototypes[objectType].Height) {
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
                switch (FurniturePrototypes[objectType].Height) {
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
                switch (FurniturePrototypes[objectType].Height) {
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
            if (furn.StationExterior) {
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


        // if this is an exterior tile we need to recalculate rooms
        if (whatFurnitureWas.StationExterior == false) return;

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


        if (t.North().Room != null) {
            northIndex = Rooms.IndexOf(t.North().Room);
            neighboorRooms.Add(Rooms[northIndex]);
        }
        if (t.South().Room != null) {
            southIndex = Rooms.IndexOf(t.South().Room);
            neighboorRooms.Add(Rooms[southIndex]);
        }
        if (t.East().Room != null) {
            eastIndex = Rooms.IndexOf(t.East().Room);
            neighboorRooms.Add(Rooms[eastIndex]);
        }
        if (t.West().Room != null) {
            westIndex = Rooms.IndexOf(t.West().Room);
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
    


    void OnTileChanged(Tile t) {
        if(_cbTileChanged == null) {
            return;
        }

        _cbTileChanged(t);
    }

    

}
