using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//InstalledObjects are like walls, doors, furniture (carpet, desk, bar)
public class Furniture
{
    // base tile but objects could be multiple tiles
    public Tile tile{ get; protected set; }

    // Tells what sprite to render.
    public string objectType { get; protected set; }

    // Multipler of cost ( value of 2 is twice as slow)
    // Tile types and other enviromental effects can further increase the cost 
    // For example a metal floor (cost 1) with a table on it ( cost 2) that's on fire (cost 3)
    // would result in (1+2+3) (6 cost) so 1/6 th of movement speed.
    // 0 cost means it's impassible like a wall.
    float movementCost;

    // a couch could be 3x2 so it has empty space before it too.
    int width;
    int height;

    public bool linksToNeighboor { get; protected set; }

    Action<Furniture> cbOnChanged;

    Func<Tile, bool> FuncToPositionValidate;

    //TOFIX: Implement larger objects
    //TOFIX: Implement object rotation
     
    protected Furniture(){

    }

    // this is a prototypical version
    static public Furniture CreatePrototype(string objectType, float movementCost = 1f, 
        int width = 1, int height = 1, bool linksToNeighboor = false )
    {
        Furniture obj = new Furniture();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighboor = linksToNeighboor;

        obj.FuncToPositionValidate = obj.IsValidPosition;


        return obj;
    }
    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if(proto.FuncToPositionValidate(tile) == false) {
            // Cannot place here.
            return null;
        }

        Furniture furn = new Furniture();

        furn.objectType = proto.objectType;
        furn.movementCost = proto.movementCost;
        furn.width = proto.width;
        furn.height = proto.height;
        furn.linksToNeighboor = proto.linksToNeighboor;



        furn.tile = tile;

        if(tile.PlaceInstalledObject(furn) == false){
            // means we can't place here probly something already placed.

            // so we can't return our furniture and give null.
            // furn will be garbage collected.
            return null;
        }

        if (furn.linksToNeighboor) {
            //this type might have some neighboors that need to update so callback them

          
            // Check N S E W neighboors
            Tile t;
            int x = furn.tile.x;
            int y = furn.tile.y;

            t = tile.World.GetTileAt(x, y + 1);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                // We have a neighboor with same object type so we callback and change it.
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.World.GetTileAt(x, y - 1);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.World.GetTileAt(x + 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

            t = tile.World.GetTileAt(x - 1, y);
            if (t != null && t.furniture != null && t.furniture.objectType == furn.objectType) {
                t.furniture.cbOnChanged(t.furniture);
            }

        }

        return furn;
    }

    public bool ValidatePositionOfFurniture(Tile t) {
        return FuncToPositionValidate(t);
    }


    // TODO: Shouldn't call this directly fix it being public
    public bool IsValidPosition(Tile t) {
        // check if is there a base tile there ?
        if(t.Type != Tile.TileType.Floor) {
            return false;
        }
        // check if is there is another furniture already occupying that tile ?
        if(t.furniture != null) {
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
        cbOnChanged += callbackFunc;
    }
    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc)
    {
        cbOnChanged -= callbackFunc;
    }
}