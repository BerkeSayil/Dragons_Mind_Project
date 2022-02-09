using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;

    private int[,] gridArray; //2D array
   
    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
                
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, 100f); //draws colomn lines
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.white, 100f); //draws row lines

            }


        }
        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f); // top line
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);  // most right line

    }

    private Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

}
