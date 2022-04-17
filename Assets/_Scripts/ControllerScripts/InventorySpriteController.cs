using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpriteController : MonoBehaviour
{
    private Dictionary<Inventory, GameObject> _inventoryGameObjectMap;
    private Dictionary<string, Sprite> _inventorySprites;

    private const int InventoryLayer = 8;

    private static World World => WorldController.Instance.World;

    private void Start() {

        LoadSprites();

        _inventoryGameObjectMap = new Dictionary<Inventory, GameObject>();

        World.RegisterInventoryCreated(OnInventoryCreated);

        
    }

    private  void OnInventoryCreated(Inventory inv) {
        
        // Create a visual game-object 
        GameObject invGo = new GameObject();

        
        _inventoryGameObjectMap.Add(inv, invGo);


        invGo.name = inv.objectType + "_" + inv.tile.x + "_" + inv.tile.y;
        invGo.transform.position = new Vector2(inv.tile.x, inv.tile.y);
        invGo.transform.SetParent(this.transform, true);

        invGo.AddComponent<SpriteRenderer>().sprite = _inventorySprites[inv.objectType];

        invGo.layer = InventoryLayer;
        
        //TODO: Make us also display the amount in the bottom maybe but not neccasry tbh ??!

        // Whenever object get removed
        inv.RegisterOnRemovedCallback(OnInventoryRemoved);
    }

   
    private void LoadSprites() {
        // Getting installed objects from the resources folder

        _inventorySprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            _inventorySprites[s.name] = s;
        }
    }

    private void OnInventoryRemoved(Inventory inv) {
        // Make sure furniture sprites are correct;

        if (_inventoryGameObjectMap.ContainsKey(inv) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject invGo = _inventoryGameObjectMap[inv];


        // gets rid of the furniture while at it and for it to work we get tile before getting rid of it
        inv.tile.PlaceInventoryObject(null);

        invGo.SetActive(false);
    }

}
