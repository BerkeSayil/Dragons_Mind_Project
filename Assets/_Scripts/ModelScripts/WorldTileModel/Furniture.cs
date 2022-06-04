using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//InstalledObjects are like walls, doors, furniture (carpet, desk, bar)
public class Furniture
{
    
    public Tile Tile{ get; protected set; } // base tile but objects could be multiple tiles
    public string ObjectType { get; protected set; } // Tells what sprite to render.

    // Multiple of cost ( value of 2 is twice as slow)
    // Tile types and other environmental effects can further increase the cost 
    // For example a metal floor (cost 1) with a table on it ( cost 2) that's on fire (cost 3)
    // would result in (1+2+3) (6 cost) so 1/6 th of movement speed.
    // 0 cost means it's impassible like a wall.
    public float MovementCost { get; protected set; }

    public bool StationExterior { get; protected set; } // determines if this protects from void so will it create a new room? 

    // a couch could be 3x2 so it has empty space before it too.
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public float Cost { get; protected set; }

    public bool LinksToNeighboor { get; protected set; } // if we want a sprite to change with regard to surrounding tiles.

    private Action<Furniture> _cbOnChanged;
    private Action<Furniture> _cbOnRemoved;

    private Func<Tile, bool> _funcToPositionValidate;

    //TODO: Implement object rotation

    private Furniture(){

    }

    // this is a prototypical version
    static public Furniture CreatePrototype(string objectType, float movementCost = 1f, 
        int width = 1, int height = 1, bool linksToNeighboor = false , bool stationExterior = false, float cost = 0f)
    {
        Furniture obj = new Furniture();

        obj.ObjectType = objectType;
        obj.MovementCost = movementCost;
        obj.Width = width;
        obj.Height = height;
        obj.LinksToNeighboor = linksToNeighboor;
        obj.StationExterior = stationExterior;
        obj.Cost = cost;

        obj._funcToPositionValidate = obj.IsValidPosition;


        return obj;
    }
    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {


        if(proto._funcToPositionValidate(tile) == false) {
            // Cannot place here.
            return null;
        }

        Furniture furn = new Furniture();

        furn.ObjectType = proto.ObjectType;
        furn.MovementCost = proto.MovementCost;
        furn.Width = proto.Width;
        furn.Height = proto.Height;
        furn.LinksToNeighboor = proto.LinksToNeighboor;
        furn.StationExterior = proto.StationExterior;
        furn.Cost = proto.Cost;

        furn.Tile = tile;

        if(tile.PlaceInstalledObject(furn) == false){
            // means we can't place here probly something already placed.

            // so we can't return our furniture and give null.
            // furn will be garbage collected.
            return null;
        }

        if (!furn.LinksToNeighboor) return furn;
        //this type might have some neighboors that need to update so callback them

          
        // Check N S E W neighboors
        Tile t = null;
        int x = furn.Tile.x;
        int y = furn.Tile.y;
        
        t = tile.World.GetTileAt(x, y + 1);
        if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
            // We have a neighboor with same object type so we callback and change it.
            t.Furniture._cbOnChanged(t.Furniture);
        }

        t = tile.World.GetTileAt(x, y - 1);
        if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
            t.Furniture._cbOnChanged(t.Furniture);
        }

        t = tile.World.GetTileAt(x + 1, y);
        if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
            t.Furniture._cbOnChanged(t.Furniture);
        }

        t = tile.World.GetTileAt(x - 1, y);
        if (t != null && t.Furniture != null && t.Furniture.ObjectType == furn.ObjectType) {
            t.Furniture._cbOnChanged(t.Furniture);
        }

        return furn;
    }

    public static void DismantleFurniture(Furniture furniture, Tile t) {
        
        Furniture prototypeOfDismantled = furniture;

        // tells tile that there is no furniture than updates neighboors for sprites
        if (t.Furniture == null) return;

        t.Furniture._cbOnRemoved(t.Furniture);

    }

    public bool ValidatePositionOfFurniture(Tile t) {

        //Multi functional until 3 by 3 

        /*        1 2 3 4
         *      4 X X X X
         *      3 X X X X
         *      2 X X X X
         *      1 X X X X
         */

        // check North and East tiles according to the size of the object
        for (int x = t.x; x < t.x + Width; x++) {
            for (int y = t.y; y < t.y + Height; y++) {
                
                Tile tile = t.World.GetTileAt(x, y);
                if(IsValidPosition(tile) == true){
                    continue;
                }
                else{
                    return false;
                }
            }
        }
        
        return true;
    }


    private bool IsValidPosition(Tile t) {
        // check if is there a base tile there ?
        if(t.Type != Tile.TileType.Floor) {
            return false;
        }
        // check if is there is another furniture already occupying that tile ?
        if(t.Furniture != null) {
            return false;
        }
        
        return true;

    }
    // TODO: Don't call this directly too so fix them being public
    public bool IsValidPositionDoor(int x, int y) {
        // also check if there is walls in both sides ?
        // check if is there a base tile there ?
        // check if is there is another furniture already occupying that tile ?


        return true;

    }

    public void RegisterOnChangedCallback(Action<Furniture> callbackFunc)
    {
        _cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc)
    {
        _cbOnChanged -= callbackFunc;
    }
    public void RegisterOnRemovedCallback(Action<Furniture> callbackFunc) {
        _cbOnRemoved += callbackFunc;
    }

    public void UnregisterOnRemovedCallback(Action<Furniture> callbackFunc) {
        _cbOnRemoved -= callbackFunc;
    }
}