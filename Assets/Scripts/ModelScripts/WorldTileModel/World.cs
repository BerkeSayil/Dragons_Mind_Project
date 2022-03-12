using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    Dictionary<string, Furniture> furniturePrototypes;

    public int width { get; }
    public int height { get; }

    Action<Furniture> cbFurnitureCreated;
    Action<Tile> cbTileChanged;

    // TODO: This should be replaced with a dedicated
    // class for managing job queues (plural!!!)
    // might look into making it semi-static or self initializing...
    public Queue<Job> jobQueue;


    public World(int width = 128, int height = 128)
    {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }

        CreateFurniturePrototypes();

        jobQueue = new Queue<Job>();
    }

    protected void CreateFurniturePrototypes()
    {
        furniturePrototypes = new Dictionary<string, Furniture>();

        // TODO: Make this get read from a data, XML, json or some other file
        furniturePrototypes.Add("Wall", 
            Furniture.CreatePrototype( 
            "Wall", 
            0, // impassible
            1, //width
            1, //height
            true // links to neighboors to look like linked.
            ));

    }

    public Tile GetTileAt(int x, int y)
    {
        if(x < 0 || x > width || y < 0 || y > height)
        {
            // clean this later
            Debug.LogError("Tile (" + x + " , " + y + ") is out of range for this world.");
            return null;
        }

        return tiles[x, y];
    }

    public void PlaceFurnitureAt(string objectType, Tile t)
    {
        //TODO: We only assume 1x1 tiles change later...

        if(furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes doesn't contain key: " + objectType);
            return;
        }

        Furniture obj = Furniture.PlaceInstance(furniturePrototypes[objectType], t);
        if(obj == null) {
            //There was something already there probly so no cally cally the callback
            return;
        }

        if(cbFurnitureCreated != null)
        {
            cbFurnitureCreated(obj);
        }

    }
    public bool IsFurniturePlacementValid(string furnitureType, Tile tile) {
        return furniturePrototypes[furnitureType].ValidatePositionOfFurniture(tile);


    }
    public void RegisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged += callbackFunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackFunc) {
        cbTileChanged -= callbackFunc;
    }

    public void RegisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        cbFurnitureCreated += callbackFunc;
    }
    public void UnregisterFurnitureCreated(Action<Furniture> callbackFunc)
    {
        cbFurnitureCreated -= callbackFunc;
    }

    void OnTileChanged(Tile t) {
        if(cbTileChanged == null) {
            return;
        }

        cbTileChanged(t);
    }
}
