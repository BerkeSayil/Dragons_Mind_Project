
// LooseObjects are things that are lying on the floor/stockpiles, like a bunch
// of metal bars or potentially a non-installed copy of a furniture.
using System;
using UnityEngine;

public class Inventory {

    public string objectType { get; protected set; } // like "Steel Plate"
    public int maxStackSize { get; protected set; }  // how much could be in there
    public int stackSize { get; protected set; } // how much is there
    public Tile tile { get; protected set; }

    Action<Inventory> cbOnChanged;
    Action<Inventory> cbOnRemoved;


    protected Inventory() {

    }

    static public Inventory CreateInventoryAtTile(Tile t, string objectType, int stackSize, int maxStackSize = 32) {

        Inventory inventory = new Inventory();

        inventory.objectType = objectType;
        inventory.stackSize = stackSize;
        inventory.maxStackSize = maxStackSize;
        inventory.tile = t;
        
        t.PlaceInventoryObject(inventory);


        return inventory;
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