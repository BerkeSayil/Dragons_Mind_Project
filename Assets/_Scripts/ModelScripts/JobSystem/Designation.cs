using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designation {
    public const int MAGICNUMBER_ONE = 1;

    public float width { get; protected set; }
    public float height { get; protected set; }

    // find biggest x and substract min x to get width
    int minX = int.MaxValue;
    int maxX = 0;
    int minY = int.MaxValue;
    int maxY = 0;

    public int centerX { get; protected set; }
    public int centerY { get; protected set; }

    DesignationType type = DesignationType.None;
    public List<Tile> tiles { get; protected set; } // The tiles that's designated

    public List<string> furnitures { get; protected set; } // all the furnitures.
    public List<string> neighbooringFurnitures { get; protected set; } // furnitures that are around a designation zone

    Action<Designation> cbDesignationChanged;

    public bool canInSpace = false; // determines if this designation can be on empty tiles

    World world = WorldController.Instance.world;

    public DesignationType Type {
        get {
            return type;
        }
        set {
            DesignationType oldType = type;
            type = value;
            // call callback to let things know we changed this
            if (cbDesignationChanged != null && oldType != type)
                cbDesignationChanged(this);
        }
    }

    public Designation(List<Tile> tiles, Designation.DesignationType type) {
        //TODO: room = tile.room;
        this.tiles = new List<Tile>(tiles);
        this.type = type;
 
        this.furnitures = new List<string>();
        this.neighbooringFurnitures = new List<string>();
        // do canInSpace true for designable of space
        if (type == DesignationType.TradeGoods) {
            canInSpace = true;
        }

        foreach (Tile tile in tiles) {
            if (tile.x <= minX) { minX = tile.x; }
            if(tile.x >= maxX) { maxX = tile.x; }
            if(tile.y <= minY) { minY = tile.y; }
            if(tile.y >= maxY) { maxY = tile.y; }

            //Debug.Log( " minx "+minX + " maxx" + maxX + " miny" + minY + " maxy" + maxY + " ");

            if (canInSpace == false) {
                // if a designation can't be in open space we check if they belong in a valid room
                // if no valid room then no designation.
                if (tile.room == WorldController.Instance.world.GetOutsideRoom()) {
                    Debug.Log("Can't designate in a non valid room");
                    this.type = DesignationType.None;
                    //TODO: Give some kind of visual feedback for this behaviour.
                    return;
                }
            }
            if(tile.furniture != null) {
                furnitures.Add(tile.furniture.objectType);
            }

        }


        GetNeighbooringTilesFurnitures();
       
        this.width = maxX - minX + MAGICNUMBER_ONE;
        this.height = maxY - minY + MAGICNUMBER_ONE;

        centerX = (int) (minX + width / 2);
        centerY = (int) (minY + height / 2);


        WorldController.Instance.world.AddDesignation(this);
    }

    private void GetNeighbooringTilesFurnitures() {
        // top most
        for (int i = minX; i <= maxX; i++) {
            Tile t = world.GetTileAt(i, maxY);
           
            Tile t3 = t.North();
            if (t3.furniture != null) {
                neighbooringFurnitures.Add(t3.furniture.objectType);
            }
        }
        // bottom
        for (int i = minX; i <= maxX; i++) {
            Tile t = world.GetTileAt(i, minY);
            
            Tile t3 = t.South();
            if (t3.furniture != null) {
                neighbooringFurnitures.Add(t3.furniture.objectType);
            }
        }
        // left 
        for (int i = minY; i <= maxY; i++) {
            Tile t = world.GetTileAt(minX, i);
            
            Tile t3 = t.West();
            if (t3.furniture != null) {
                neighbooringFurnitures.Add(t3.furniture.objectType);
            }
        }
        // right
        for (int i = minY; i <= maxY; i++) {
            Tile t = world.GetTileAt(maxX, i);
            
            Tile t3 = t.East();
            if (t3.furniture != null) {
                neighbooringFurnitures.Add(t3.furniture.objectType);
            }
        }

    }

    private bool IsRoomItself() {
        //This function is needed if a designation has to be room by themselves and can't be a common place with an open floor plan.

        int wallAmount = NumberOfInList(neighbooringFurnitures, "Wall");
        int doorAmount = NumberOfInList(neighbooringFurnitures, "Door");

        int sorroundingAmount =(int)(2 * width + 2 * height);

        if (wallAmount + doorAmount >= sorroundingAmount) return true;
     
        return false;
    }

    public bool IsFunctional() {
        //So the way these designations work with world is they are the inner area of walls and doors.
        //walls and doors should not be in the designation itself!

        
        if (Type == DesignationType.PersonalCrewRoom) {

            if (furnitures.Contains("SleepingPod") && IsRoomItself()) {
                Debug.Log("Crew Room is functional");
                return true;
            }
            return false;
        }else if(Type == DesignationType.Kitchen) {
            if (furnitures.Contains("FoodProcesser") && neighbooringFurnitures.Contains("Door")) {
                Debug.Log("Kitchen is functional");
                return true;
            }
            return false;
        }
        else if(Type == DesignationType.Cafeteria) {
            if (NumberOfInList(furnitures, "Desk") > 2 && neighbooringFurnitures.Contains("Door")) {
                Debug.Log("Cafeteria is functional");
                return true;
            }
            return false;
        }
        else if(Type == DesignationType.Engine) {
            //TODO: MIGHT LOOK INTO ELECTRIC SYSTEM NEXT WE'LL USE TILE NEIGHBOOR NORTHSOUTHEASTWEST METHODS THERE
            if (furnitures.Contains("Engine")) {
                Debug.Log("Engine is functional");
                return true;
            }
            return false;
        }
        else if(Type == DesignationType.LifeSupport) {
            //TODO: WHILE YOU MENTION IT ROOMS HAVE ATMOS BUT WE CAN ONLY IMPLEMENT THAT AFTER THIS ATMOSPROVIDER FURNITURE
            //TODO: IS FUNCTIONAL WE MIGHT DO SOME SPECIAL CASE FOR THIS.
            if (furnitures.Contains("LifeSupportMaintainer") && furnitures.Contains("AtmosProvider")) {
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

    private int NumberOfInList(List<String> list, string whatWeWantCountOf) {
        int numberOfOccurences = 0;
        foreach (String str in list) {
            if (str == whatWeWantCountOf) numberOfOccurences += 1;
        }
        return numberOfOccurences;

    }

    public bool IsValidDesignation(List<Tile> tiles) {
        bool areWeValid = true;
        foreach (Tile t in tiles) {

            if (t == null) {
                Debug.Log("Tile null while trying to designate");
                areWeValid = false;
            }
            if (t.Type == Tile.TileType.Empty && canInSpace && t.designationType == Designation.DesignationType.None) {

                //areWeValid = true;
            }
            if (t.Type == Tile.TileType.Floor && t.furniture == null) {
                if (t.designationType == Designation.DesignationType.None) {
                    //areWeValid = true;
                }
                else { areWeValid = false; }
            }
            else if (t.Type == Tile.TileType.Floor && t.furniture.stationExterior == false && t.designationType == Designation.DesignationType.None) {
                //areWeValid =  true;
            }
            
            
        }
        return areWeValid;
    }
    public void RegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        
        cbDesignationChanged += callback;
    }
    public void UnRegisterDesignationTypeChangedCallback(Action<Designation> callback) {
        cbDesignationChanged -= callback;
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

        WorldController.Instance.world.RemoveDesignation(this);
    }


    public void UpdateDesignationFurnitures() {
        foreach (Tile tile in tiles) {
            
            if(tile.furniture != null) { //TODO: Find out how many of an object is available
                if (furnitures.Contains(tile.furniture.objectType) == false) {
                    furnitures.Add(tile.furniture.objectType);
                }
            }
        }

    }



    
}


