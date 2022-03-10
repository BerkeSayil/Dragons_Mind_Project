using System.Collections;
using System.Collections.Generic;
using System;


public class Tile
{
    // Tile can have 1 of these
    LooseObject looseObject;
    InstalledObject installedObject;

    // A tile is self aware
    World world;
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
        this.world = world;
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
