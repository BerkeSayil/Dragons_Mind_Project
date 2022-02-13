using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] int rows = 10;
    [SerializeField] int cols = 10;

    [SerializeField] int cellSize = 1;

    private void Start()
    {
        GenerateGrid();

    }

    private void GenerateGrid()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("DefaultBlock"));

        

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = col * cellSize;
                float posY = row * -cellSize;
                Debug.Log((row, col));
                tile.transform.position = new Vector2(posX, posY);


            }
        }

        float gridW = cols * cellSize;
        float gridH = rows * cellSize;

        transform.position = new Vector2(gridW / -2 + (cellSize/2) , gridH / 2 - (cellSize/2));

        Destroy(referenceTile);
    
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (IsInGrid()) // is mouse in grid?
            {

                GameObject objectUnderMouse = MouseRaycast();
                Destroy(objectUnderMouse);

                /*if so and there is a click then place it in the square by centering it
                Vector2 gridBlock;
                int gridRow = ((int)(mousePos.x / cellSize));
                int gridColumn = ((int)(mousePos.y / cellSize));

                gridBlock = new Vector2((gridRow * cellSize) + 2.5f , (gridColumn * cellSize) + 2.5f);
                */


            }
            
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

        }
    }
    private GameObject MouseRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(MousePosFinder(), Vector2.zero);

        if (hit.collider != null)
        {
            //We have reached the object under the mouse
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private Vector2 MousePosFinder()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
 

    }

    private bool IsInGrid()
    {
        Vector2 mousePos = MousePosFinder();
        return mousePos.x > 0 && mousePos.x < (cols * cellSize) && mousePos.y > 0 && mousePos.y < (rows * cellSize);
    }


}
