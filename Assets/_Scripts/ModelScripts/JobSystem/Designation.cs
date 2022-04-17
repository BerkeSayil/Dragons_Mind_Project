using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designation {
    private  const int MagicNumberOne = 1;

    // readonly makes it so it can't get re-assigned after constructor exits
    private readonly float _width;

    private readonly float _height;
    
    // find biggest x and substract min x to get width
    private readonly int _minX = int.MaxValue;
    private readonly int _maxX = 0;
    private readonly int _minY = int.MaxValue;
    private readonly int _maxY = 0;

    public int CenterX { get; protected set; }
    public int CenterY { get; protected set; }

    private DesignationType _type;
    public List<Tile> Tiles { get; protected set; } // The tiles that's designated

    public List<string> Furnitures { get; protected set; } // all the furniture.
    public List<string> NeighbooringFurnitures { get; protected set; } // furnitures that are around a designation zone

    private Action<Designation> _cbDesignationChanged;

    private readonly bool _canInSpace = false; // determines if this designation can be on empty tiles

    private static World World = WorldController.Instance.World;

    public DesignationType Type {
        get => _type;
        set {
            DesignationType oldType = _type;
            _type = value;
            // call callback to let things know we changed this
            if (_cbDesignationChanged != null && oldType != _type)
                _cbDesignationChanged(this);
        }
    }

    public Designation(List<Tile> tiles, Designation.DesignationType type) {
        this.Tiles = new List<Tile>(tiles);
        this._type = type;
 
        this.Furnitures = new List<string>();
        this.NeighbooringFurnitures = new List<string>();
        // do canInSpace true for designable of space
        if (type == DesignationType.TradeGoods) {
            _canInSpace = true;
        }

        foreach (Tile tile in tiles) {
            if (tile.x <= _minX) { _minX = tile.x; }
            if(tile.x >= _maxX) { _maxX = tile.x; }
            if(tile.y <= _minY) { _minY = tile.y; }
            if(tile.y >= _maxY) { _maxY = tile.y; }

            //Debug.Log( " minx "+minX + " maxx" + maxX + " miny" + minY + " maxy" + maxY + " ");

            if (_canInSpace == false) {
                // if a designation can't be in open space we check if they belong in a valid room
                // if no valid room then no designation.
                if (tile.Room == WorldController.Instance.World.GetOutsideRoom()) {
                    Debug.Log("Can't designate in a non valid room");
                    this._type = DesignationType.None;
                    //TODO: Give some kind of visual feedback for this behaviour.
                    return;
                }
            }
            if(tile.Furniture != null) {
                Furnitures.Add(tile.Furniture.ObjectType);
            }

        }


        GetNeighbooringTilesFurnitures();
       
        this._width = _maxX - _minX + MagicNumberOne;
        this._height = _maxY - _minY + MagicNumberOne;

        CenterX = (int) (_minX + _width / 2);
        CenterY = (int) (_minY + _height / 2);


        WorldController.Instance.World.AddDesignation(this);
    }

    private void GetNeighbooringTilesFurnitures() {
        /*
         * XXXXX
         * X   X
         * X   X
         * XXXXX
         * 
         * 
         */
        // top most
        for (int i = _minX; i <= _maxX; i++) {
            Tile t = World.GetTileAt(i, _maxY);
           
            Tile t3 = t.North();
            if (t3.Furniture != null) {
                NeighbooringFurnitures.Add(t3.Furniture.ObjectType);

            }
        }
        // bottom
        for (int i = _minX; i <= _maxX; i++) {
            Tile t = World.GetTileAt(i, _minY);
            
            Tile t3 = t.South();
            if (t3.Furniture != null) {
                NeighbooringFurnitures.Add(t3.Furniture.ObjectType);

            }
        }
        // left 
        for (int i = _minY; i <= _maxY; i++) {
            Tile t = World.GetTileAt(_minX, i);
            
            Tile t3 = t.West();
            if (t3.Furniture != null) {
                NeighbooringFurnitures.Add(t3.Furniture.ObjectType);

            }
        }
        // right
        for (int i = _minY; i <= _maxY; i++) {
            Tile t = World.GetTileAt(_maxX, i);
            
            Tile t3 = t.East();
            if (t3.Furniture != null) {
                NeighbooringFurnitures.Add(t3.Furniture.ObjectType);

            }
        }

    }

    private bool IsRoomItself() {
        //This function is needed if a designation has to be room by themselves and can't be a common place with an open floor plan.
        int wallAmount = NumberOfInList(NeighbooringFurnitures, "Wall");
        int doorAmount = NumberOfInList(NeighbooringFurnitures, "Door");

        int sorroundingAmount =(int)(2 * _width + 2 * _height);

        if (wallAmount + doorAmount >= sorroundingAmount) return true;
        

        return false;
    }

    private int NumberOfInList(List<String> list, string whatWeWantCountOf) {
        int numberOfOccurences = 0;
        foreach (String str in list) {
            if (str == whatWeWantCountOf) numberOfOccurences += 1;
        }
        return numberOfOccurences;

    }

    public bool IsFunctional() {
        //TODO: This gets called way more than neccessary.

        //So the way these designations work with world is they are the inner area of walls and doors.
        //walls and doors should not be in the designation itself!

        GetNeighbooringTilesFurnitures();

        if (Type == DesignationType.PersonalCrewRoom) {


            if (Furnitures.Contains("SleepingPod") && IsRoomItself()) {
                Debug.Log("Crew Room is functional");

                return true;
            }
            return false;
        }else if(Type == DesignationType.Kitchen) {


            if (Furnitures.Contains("FoodProcesser") && NeighbooringFurnitures.Contains("Door")) {
                Debug.Log("Kitchen is functional");

                return true;
            }
            return false;
        }
        else if(Type == DesignationType.Cafeteria) {


            if (NumberOfInList(Furnitures, "Desk") > 2 && NeighbooringFurnitures.Contains("Door")) {
                Debug.Log("Cafeteria is functional");

                return true;
            }
            return false;
        }
        else if(Type == DesignationType.Engine) {
            //TODO: MIGHT LOOK INTO ELECTRIC SYSTEM NEXT WE'LL USE TILE NEIGHBOOR NORTHSOUTHEASTWEST METHODS THERE
            if (Furnitures.Contains("Engine")) {
                Debug.Log("Engine is functional");

                return true;
            }
            return false;
        }
        else if(Type == DesignationType.LifeSupport) {
            //TODO: WHILE YOU MENTION IT ROOMS HAVE ATMOS BUT WE CAN ONLY IMPLEMENT THAT AFTER THIS ATMOSPROVIDER FURNITURE
            //TODO: IS FUNCTIONAL WE MIGHT DO SOME SPECIAL CASE FOR THIS.
            if (Furnitures.Contains("LifeSupportMaintainer") && Furnitures.Contains("AtmosProvider")) {
                Debug.Log("LifeSupport is functional");

                return true;
            }
            return false;
        }
        else if(Type == DesignationType.TradeGoods) {
            
            // Trade goods not really picky

            Debug.Log("TradeGoods is functional");

            return true;

        }
        else {
            return false;
        }


    }

    public bool IsValidDesignation(List<Tile> tiles) {
        bool areWeValid = true;
        foreach (Tile t in tiles) {

            if (t == null) {
                Debug.Log("Tile null while trying to designate");
                areWeValid = false;
            }
            if (t.Type == Tile.TileType.Empty && _canInSpace && t.DesignationType == Designation.DesignationType.None) {

                //areWeValid = true;
            }
            if (t.Type == Tile.TileType.Floor && t.Furniture == null) {
                if (t.DesignationType == Designation.DesignationType.None) {
                    //areWeValid = true;
                }
                else { areWeValid = false; }
            }
            else if (t.Type == Tile.TileType.Floor && t.Furniture.StationExterior == false && t.DesignationType == Designation.DesignationType.None) {
                //areWeValid =  true;
            }
            
            
        }
        return areWeValid;
    }
    public void RegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        
        _cbDesignationChanged += callback;
    }
    public void UnRegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        _cbDesignationChanged -= callback;
    }


    public enum DesignationType { // Tells what designation color to render.

        None,             // default tile
        PersonalCrewRoom, // where they sleep n stuff
        Kitchen,          // responsible for cooking
        Cafeteria,        // where they eat n drink
        Engine,           // making sure no problems on electricty side
        LifeSupport,      // responsible for not making everyone die (athmos balance)
        TradeGoods        // where we receive bought goods.


    }

    public void DestroyDesignation() {

        WorldController.Instance.World.RemoveDesignation(this);
    }


    public void UpdateDesignationFurnitures() {
        foreach (Tile tile in Tiles) {
            
            if(tile.Furniture != null) { //TODO: Find out how many of an object is available
                if (Furnitures.Contains(tile.Furniture.ObjectType) == false) {
                    Furnitures.Add(tile.Furniture.ObjectType);
                }
            }
        }

    }



    
}


