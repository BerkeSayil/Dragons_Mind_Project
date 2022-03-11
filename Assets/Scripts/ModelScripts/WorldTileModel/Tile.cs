using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
    // Tile can have 1 of these
    LooseObject looseObject;
    public Furniture furniture { get; protected set; }

    // A tile is self aware
    public World World { get; protected set; }
    public int x { get; protected set; }
    public int y { get; protected set; }

    // callback
    Action<Tile> cbTileTypeChanged;

    public enum TileType
    {
        Empty,
        Floor
    }

    TileType type = TileType.Empty;


    public Tile(World world, int x, int y)
    {
        this.World = world;
        this.x = x;
        this.y = y;
    }
    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        // the place where this gets called is subscribed to the cbTileTypeChanged
        // so when cbTileTypeChanged Action gets called in return this function callsback to
        // original script where its called.
        cbTileTypeChanged += callback;
    }
    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public bool PlaceInstalledObject(Furniture objInstance)
    {
        if(objInstance == null)
        {
            // If we send null it means uninstall whatever was here.
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
    
    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            TileType oldType = type;
            type = value;
            // call callback to let things now we changed this
            if(cbTileTypeChanged != null && oldType != type)
                cbTileTypeChanged(this);
        }
    }
    
}
