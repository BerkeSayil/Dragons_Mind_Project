using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{

    [SerializeField] int worldWidth = 35;
    [SerializeField] int worldHeight = 20;
    private int depthZ = 5;

    [SerializeField] float cellSize = 5f;

    [SerializeField] GameObject buildingBlock;


    void Start()
    {
        Grid grid = new Grid(worldWidth, worldHeight, cellSize, depthZ);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mousePosFinder();

            if (mousePos.x > 0 && mousePos.x < (worldWidth * cellSize) && mousePos.y > 0 && mousePos.y < (worldHeight * cellSize)) // is mouse in grid?
            {
                //if so and there is a click then place it in the square by centering it
                Vector2 gridBlock;
                int gridRow = ((int)(mousePos.x / cellSize));
                int gridColumn = ((int)(mousePos.y / cellSize));

                gridBlock = new Vector2((gridRow * cellSize) + 2.5f , (gridColumn * cellSize) + 2.5f);

                Instantiate(buildingBlock, gridBlock, Quaternion.identity);
            }
            
        }
    }

    private Vector2 mousePosFinder()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
 

    }



}
