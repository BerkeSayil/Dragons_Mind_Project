using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    public int width { get; }
    public int height { get; }
    
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
            }
        }

        

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

    
}
