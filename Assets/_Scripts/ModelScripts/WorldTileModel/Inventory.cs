
// LooseObjects are things that are lying on the floor/stockpiles, like a bunch
// of metal bars or potentially a non-installed copy of a furniture.
using System;
using UnityEngine;

public class Inventory {

    public string objectType { get; protected set; } // like "Steel Plate"
    public int maxStackSize { get; protected set; }  // how much could be in there : 1 for furnitures but 12 for resource type ?
    public int stackSize { get; protected set; } // how much is there
    public Tile tile { get; protected set; }

    Action<Inventory> cbOnChanged;
    Action<Inventory> cbOnRemoved;


    protected Inventory() {

    }

    static public Inventory CreateInventoryProto(string objectType, int stackSize, int maxStackSize = 12) {

        Inventory inventory = new Inventory();

        inventory.objectType = objectType;
        inventory.stackSize = stackSize;
        inventory.maxStackSize = maxStackSize;
   
        return inventory;
    }


    static public Inventory PlaceInstance(Inventory proto, Tile tile) {

        Inventory inv = new Inventory();

        inv.objectType = proto.objectType;
        inv.stackSize = proto.stackSize;
        inv.maxStackSize = proto.maxStackSize;


        inv.tile = tile;

        if (tile.PlaceInventoryObject(inv) == false) {
            // means we can't place here probly something already placed.

            // so we can't return our furniture and give null.
            // furn will be garbage collected.
            return null;
        }

      
        return inv;
    }

    public void RegisterOnChangedCallback(Action<Inventory> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Inventory> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }
    public void RegisterOnRemovedCallback(Action<Inventory> callbackFunc) {
        cbOnRemoved += callbackFunc;
    }

    public void UnregisterOnRemovedCallback(Action<Inventory> callbackFunc) {
        cbOnRemoved -= callbackFunc;
    }

}