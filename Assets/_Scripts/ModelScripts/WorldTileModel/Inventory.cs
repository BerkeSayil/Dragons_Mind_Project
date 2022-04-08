
// LooseObjects are things that are lying on the floor/stockpiles, like a bunch
// of metal bars or potentially a non-installed copy of a furniture.
using System;
using UnityEngine;

public class Inventory {

    public string objectType { get; protected set; } // like "Steel Plate"
    public Tile tile { get; protected set; }

    Action<Inventory> cbOnChanged;
    Action<Inventory> cbOnRemoved;


    protected Inventory() {

    }

    static public Inventory CreateInventoryProto(string objectType) {

        Inventory inventory = new Inventory();

        inventory.objectType = objectType;
        
   
        return inventory;
    }

    static public Inventory PlaceInstance(Inventory proto, Tile tile) {

        Inventory inv = new Inventory();

        inv.objectType = proto.objectType;
        

        inv.tile = tile;

        if (tile.PlaceInventoryObject(inv) == false) {
            // means we can't place here probly something already placed.

            // so we can't return our furniture and give null.
            // furn will be garbage collected.
            return null;
        }

      
        return inv;
    }

    public static void PickInventoryUp(Tile t) {

        // tells tile that there is no inventory than updates for sprite
        if (t.looseObject == null) return;

        t.looseObject.cbOnRemoved(t.looseObject);

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