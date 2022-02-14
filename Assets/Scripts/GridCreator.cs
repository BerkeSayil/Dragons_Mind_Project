using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] int rows = 10;
    [SerializeField] int cols = 10;

    [SerializeField] int cellSize = 1;

    bool clickedOnGrid = false;
    private int spacePrefabs = 0;

    [SerializeField] //TODO MAKE THIS AUTOMIZED
    private GameObject[] worldGenBlock = new GameObject[6];
    private Vector2[] moonObjectCoord = new Vector2[2501];
   

    private void Start()
    {
        GenerateGrid();
        
    }

    private void GenerateGrid()
    {
        

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float posX = col * cellSize;
                float posY = row * -cellSize;
                int[] moonHeavyDist = new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1};
                int tileRand = (int)moonHeavyDist.GetValue(UnityEngine.Random.Range(0, 10));


                if (tileRand != 0) // Random Space Background Degilse
                {
                    
                    moonObjectCoord[spacePrefabs] = new Vector2(posX, posY);

                    Vector2 currentPos = new Vector2(posX, posY);
                    for (int i = 0; i < moonObjectCoord.Length; i++)
                    {
                        if ((currentPos - moonObjectCoord[i]).magnitude > cellSize)
                        {
                            tileRand = 0;
                            Debug.Log("Execute");
                        }
                    
                    }
                    
                    spacePrefabs += 1;

                }
                
                GameObject tile = (GameObject)Instantiate(worldGenBlock[tileRand], transform);


                tile.transform.position = new Vector2(posX, posY);


            }
        }

        float gridW = cols * cellSize;
        float gridH = rows * cellSize;

        float pivot = (float)cellSize / 2;
        float x = (gridW / -2 + pivot);
        float y = (gridH / 2 - pivot);
        

        transform.position = new Vector2(x,y);

        

       
    
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseRaycast();
            if (clickedOnGrid) // is mouse in grid?
            {

                GameObject objectUnderMouse = MouseRaycast();
                Destroy(objectUnderMouse);

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
            clickedOnGrid = true;
            return hit.collider.gameObject;
            
        }
        else
        {
            clickedOnGrid = false;
            return null;
        }
    }

    private Vector2 MousePosFinder()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
 

    }

    


}
