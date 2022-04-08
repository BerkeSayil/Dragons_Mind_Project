using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
    // Tile can have 1 of these
    public Inventory looseObject { get; protected set; }
    public Furniture furniture { get; protected set; }

    public Job pendingFurnitureJob;

    public Job pendingHaulJob;

    public Job pendingTileJob;

    public Room room;

    public Designation.DesignationType designationType;

    // A tile is self aware
    public World World { get; protected set; }
    public int x { get; protected set; }
    public int y { get; protected set; }

    // callback
    Action<Tile> cbTileChanged;

    public Tile(World world, int x, int y) {
        this.World = world;
        this.x = x;
        this.y = y;
    }
    public enum TileType
    {
        Empty,
        Floor
    }

    TileType type = TileType.Empty;
    
    public TileType Type {
        get {
            return type;
        }
        set {
            TileType oldType = type;
            type = value;
            // call callback to let things know we changed this
            if (cbTileChanged != null && oldType != type)
                cbTileChanged(this);
        }
    }
    public bool PlaceInventoryObject(Inventory looseObjectInstance) {
        if (looseObjectInstance == null) {
            // sending null removes from tile
            looseObject = null;
            return true;
        }

        if (looseObject != null) {
            Debug.LogError("Trying to place inventory on Tile but there already is one.");
            return false;
        }

        // If it didn't fall into those traps than we are good to go.

        looseObject = looseObjectInstance;
        return true;

    }

    public bool PlaceInstalledObject(Furniture objInstance)
    {
        if(objInstance == null)
        {
            furniture = null;
            return true;
        }
        
        if(furniture != null)
        {
            Debug.LogError("Trying to install object but there already is one.");
            return false;
        }

        // If it didn't fall into those traps than we are good to go.

        furniture = objInstance;
        return true;

    }

    public bool IsNeighboor(Tile tile, bool diagOkay = false) {

        // same X but only 1 diff in Y
        if(this.x == tile.x && ( this.y == tile.y + 1 || this.y == tile.y - 1)) {
            return true;
        }
        if (this.y == tile.y && (this.x == tile.x + 1 || this.x == tile.x - 1)) {
            return true;
        }
        if (diagOkay) {
            if (this.x == tile.x + 1 && (this.y == tile.y + 1 || this.y == tile.y - 1)) {
                return true;
            }
            if (this.x == tile.x - 1 && (this.y == tile.y + 1 || this.y == tile.y - 1)) {
                return true;
            }
        }

        return false;
    }


    public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
        // the place where this gets called is subscribed to the cbTileTypeChanged
        // so when cbTileTypeChanged Action gets called in return this function callsback to
        // original script where its called.
        cbTileChanged += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileChanged -= callback;
    }

    public Tile North() {
        return World.GetTileAt(x,y + 1);
    }
    public Tile South() {
        return World.GetTileAt(x, y - 1);
    }
    public Tile East() {
        return World.GetTileAt(x + 1, y);
    }
    public Tile West() {
        return World.GetTileAt(x - 1, y);
    }

    public bool ValidateTileChange(TileType tileType) {
        if (tileType != type) return true;

        return false;
    }
}
