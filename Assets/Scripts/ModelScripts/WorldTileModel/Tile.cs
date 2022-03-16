using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
    // Tile can have 1 of these
    LooseObject looseObject;
    public Furniture furniture { get; protected set; }

    public Job pendingFurnitureJob;

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

    public bool PlaceInstalledObject(Furniture objInstance)
    {
        if(objInstance == null)
        {
            // TODO: we should be ablo to send null to uninstall whatever was here.
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
}
