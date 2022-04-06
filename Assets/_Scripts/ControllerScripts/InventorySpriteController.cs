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
        
        // Whenever objects anything changes (door animatons n stuff.)
        inv.RegisterOnChangedCallback(OnInventoryChanged);

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

    void OnInventoryChanged(Inventory inv) {
        // Make sure furniture sprites are correct;

        if (furnitureGameObjectMap.ContainsKey(inv) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGO = furnitureGameObjectMap[inv];

        SpriteRenderer furnSprite = furnGO.GetComponent<SpriteRenderer>();



        furnSprite.sprite = GetSpriteForFurniture(inv);
        furnSprite.sortingLayerName = "Furniture";
    }

    void OnInventoryRemoved(Inventory inv) {
        // Make sure furniture sprites are correct;

        if (furnitureGameObjectMap.ContainsKey(inv) == false) {
            Debug.Log("OnFurnitureChanged ~ something funky.");
            return;
        }

        // Updates sprites on a change function
        GameObject furnGO = furnitureGameObjectMap[inv];


        // gets rid of the furniture while at it and for it to work we get tile before getting rid of it
        Tile tileFurnGotRemoved = inv.tile;
        inv.tile.PlaceInstalledObject(null);

        // updates 4 direction furnitures to update
        if (tileFurnGotRemoved.North().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.North().furniture);
        if (tileFurnGotRemoved.South().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.South().furniture);
        if (tileFurnGotRemoved.East().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.East().furniture);
        if (tileFurnGotRemoved.West().furniture != null) OnFurnitureChanged(tileFurnGotRemoved.West().furniture);

        furnGO.SetActive(false);
    }

}
