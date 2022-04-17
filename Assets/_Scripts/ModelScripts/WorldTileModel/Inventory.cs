
// LooseObjects are things that are lying on the floor/stockpiles, like a bunch
// of metal bars or potentially a non-installed copy of a furniture.
using System;
using UnityEngine;

public class Inventory {

    public string ObjectType { get; protected set; } // like "Steel Plate"
    public Tile Tile { get; protected set; }

    private Action<Inventory> _cbOnChanged;
    private Action<Inventory> _cbOnRemoved;

    private Inventory() {

    }

    static public Inventory CreateInventoryProto(string objectType) {

        Inventory inventory = new Inventory();

        inventory.ObjectType = objectType;
        
   
        return inventory;
    }

    static public Inventory PlaceInstance(Inventory proto, Tile tile) {

        Inventory inv = new Inventory();

        inv.ObjectType = proto.ObjectType;
        

        inv.Tile = tile;

        if (tile.PlaceInventoryObject(inv) == false) {
            // means we can't place here probly something already placed.

            // so we can't return our inventory and give null.
            // inv will be garbage collected.
            return null;
        }

      
        return inv;
    }

    public static void PickInventoryUp(Tile t) {

        // tells tile that there is no inventory than updates for sprite
        if (t.LooseObject == null) return;


        t.LooseObject._cbOnRemoved?.Invoke(t.LooseObject);


    }

    public void RegisterOnChangedCallback(Action<Inventory> callbackFunc) {
        _cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Inventory> callbackFunc) {
        _cbOnChanged -= callbackFunc;
    }
    public void RegisterOnRemovedCallback(Action<Inventory> callbackFunc) {
        _cbOnRemoved += callbackFunc;
    }

    public void UnregisterOnRemovedCallback(Action<Inventory> callbackFunc) {
        _cbOnRemoved -= callbackFunc;
    }

}