using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinBasedGridCreator : MonoBehaviour
{

    private Dictionary<int, GameObject> tileset;
    private Dictionary<int, GameObject> tileGroups;


    [SerializeField]
    GameObject spaceVoidTile;
    [SerializeField]
    GameObject buildMatResourceTile;
    [SerializeField]
    GameObject gasResourceTile;
    [SerializeField]
    GameObject astreoidTile;
    [SerializeField]
    GameObject dangerousStuffTile;
    [SerializeField]
    GameObject mineralTile;

    [SerializeField] int mapWidth = 160;
    [SerializeField] int mapHeight = 160;

    List<List<int>> noiseGrid = new List<List<int>>(); // Initialize outer list
    List<List<GameObject>> tileGrid = new List<List<GameObject>>(); // Initialize outer list

    float magnification = 7.0f; //4 - 20 are values recommended this zoomes in and out of the perlin depending on the need

    int xOffset = 0; // perlin noise shift in X axis
    int yOffset = 0; // perlin noise shift in Y axis


    private void Start()
    {
        CreateTileset();

        CreateTileGroups();

        GenerateMap();

    }

    private void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {

            noiseGrid.Add(new List<int>()); // Initialize inner lists
            tileGrid.Add(new List<GameObject>()); // Initialize inner lists


            for (int y = 0; y < mapHeight; y++)
            {
                int tileId = GetIdUsingPerlin(x, y);

                //this is a test

                if(tileId == 3 || tileId == 4) { tileId = 0; }

                // test end
                noiseGrid[x].Add(tileId);
                CreateTile(tileId, x, y);
            }
        }
    }

    private void CreateTile(int tileId, int x, int y)
    {
        GameObject tilePrefab = tileset[tileId]; // Bu tile id nin hangi prefab oldugu
        GameObject tileGroup = tileGroups[tileId]; // Hierachy de kimin child i olmasi gerektigi infosu

        
        GameObject tile = Instantiate(tilePrefab, tileGroup.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);

        tile.transform.localPosition = new Vector2(x, y); // localPos for under parent

        tileGrid[x].Add(tile);

    }

    private int GetIdUsingPerlin(int x , int y)
    {
        float rawPerlin = Mathf.PerlinNoise(
            (x - xOffset) / magnification,

            (y - yOffset) / magnification);

        float clamppedPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);
        float scaledPerlin = clamppedPerlin * tileset.Count;

        if(scaledPerlin == tileset.Count)
        {
            scaledPerlin -= 1;
        }

        return Mathf.FloorToInt(scaledPerlin);

    }

    private void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();

        tileset.Add(0, spaceVoidTile);
        tileset.Add(1, buildMatResourceTile);
        tileset.Add(2, gasResourceTile);
        tileset.Add(3, astreoidTile);
        tileset.Add(4, dangerousStuffTile);
        tileset.Add(5, mineralTile);


    }

    private void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefabPair in tileset)
        {
            GameObject tileGroup = new GameObject(prefabPair.Value.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = new Vector2(0, 0);

            tileGroups.Add(prefabPair.Key, tileGroup);
        }

    }   
}
