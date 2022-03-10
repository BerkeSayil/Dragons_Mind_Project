using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//InstalledObjects are like walls, doors, furniture (carpet, desk, bar)
public class InstalledObject
{
    // base tile but objects could be multiple tiles
    Tile tile;

    // Tells what sprite to render.
    string objectType;

    // Multipler of cost ( value of 2 is twice as slow)
    // Tile types and other enviromental effects can further increase the cost 
    // For example a metal floor (cost 1) with a table on it ( cost 2) that's on fire (cost 3)
    // would result in (1+2+3) (6 cost) so 1/6 th of movement speed.
    // 0 cost means it's impassible like a wall.
    float movementCost;

    // a couch could be 3x2 so it has empty space before it too.
    int width;
    int height;

    protected InstalledObject(){

    }

    // this is a prototypical version
    static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1 )
    {
        InstalledObject obj = new InstalledObject();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        return obj;
    }
    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = new InstalledObject();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;

        obj.tile = tile;

        //TODO:
        //tile.installedObject = this;

        return obj;
    }

}