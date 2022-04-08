using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpriteController : MonoBehaviour
{
    Dictionary<Inventory, GameObject> inventoryGameObjectMap;
    Dictionary<string, Sprite> inventorySprites;

    const int INVENTORY_LAYER = 8;

    World world {
        get { return WorldController.Instance.world; }
    }

    void Start() {

        LoadSprites();

        inventoryGameObjectMap = new Dictionary<Inventory, GameObject>();

        world.RegisterInventoryCreated(OnInventoryCreated);

        
    }

    public void OnInventoryCreated(Inventory inv) {
        
        // Create a visual gameobject 
        GameObject invGO = new GameObject();

        
        inventoryGameObjectMap.Add(inv, invGO);


        invGO.name = inv.objectType + "_" + inv.tile.x + "_" + inv.tile.y;
        invGO.transform.position = new Vector2(inv.tile.x, inv.tile.y);
        invGO.transform.SetParent(this.transform, true);

        invGO.AddComponent<SpriteRenderer>().sprite = inventorySprites[inv.objectType];

        invGO.layer = INVENTORY_LAYER;
        
        //TODO: Make us also display the amount in the bottom maybe but not neccasry tbh ??!

        // Whenever object get removed
        inv.RegisterOnRemovedCallback(OnInventoryRemoved);
    }

   
    private void LoadSprites() {
        // Getting installed objects from the resources folder

        inventorySprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");
        foreach (Sprite s in sprites) {
            inventorySprites[s.name] = s;
        }
    }

    void OnInventoryRemoved(Inventory inv) {
        // Make sure furniture sprites are correct;

        if (inventoryGameObjectMap.ContainsKey(inv) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject invGO = inventoryGameObjectMap[inv];


        // gets rid of the furniture while at it and for it to work we get tile before getting rid of it
        inv.tile.PlaceInventoryObject(null);

        invGO.SetActive(false);
    }

}
