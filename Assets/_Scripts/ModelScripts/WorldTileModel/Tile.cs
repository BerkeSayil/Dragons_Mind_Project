using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
    // Tile can have 1 of these
    public Inventory LooseObject { get; protected set; }
    public Furniture Furniture { get; protected set; }

    public Job PendingFurnitureJob;

    public Job PendingHaulJob;

    public Job PendingTileJob;

    public Room Room;

    public Designation.DesignationType DesignationType;

    public bool IsSpaceShip;

    // A tile is self aware
    public World World { get; protected set; }
    public int x { get; protected set; }
    public int y { get; protected set; }

    // callback
    private Action<Tile> _cbTileChanged;

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

    TileType _type = TileType.Empty;
    
    public TileType Type {
        get {
            return _type;
        }
        set {
            TileType oldType = _type;
            _type = value;
            // call callback to let things know we changed this
            if (_cbTileChanged != null && oldType != _type)
                _cbTileChanged(this);
        }
    }
    public bool PlaceInventoryObject(Inventory looseObjectInstance) {
        if (looseObjectInstance == null) {
            // sending null removes from tile
            LooseObject = null;
            return true;
        }

        if (LooseObject != null) {
            Debug.LogError("Trying to place inventory on Tile but there already is one.");
            return false;
        }

        // If it didn't fall into those traps than we are good to go.

        LooseObject = looseObjectInstance;
        return true;

    }

    public bool PlaceInstalledObject(Furniture objInstance)
    {
        if(objInstance == null)
        {
            Furniture = null;
            return true;
        }
        
        if(Furniture != null)
        {
            Debug.LogError("Trying to install object but there already is one.");
            return false;
        }

        // If it didn't fall into those traps than we are good to go.

        Furniture = objInstance;
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
        _cbTileChanged += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
        _cbTileChanged -= callback;
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
        if (tileType != _type) return true;

        return false;
    }
}
