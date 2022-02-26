using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinBasedGridCreator : MonoBehaviour
{

    private Dictionary<int, GameObject> tileset;
    private Dictionary<int, GameObject> tileGroups;


    public GameObject spaceVoidTile;
    [SerializeField] GameObject buildMatResourceTile;
    [SerializeField] GameObject gasResourceTiles;

    private int mapWidth = 64;
    private int mapHeight = 64;

    List<List<int>> noiseGrid = new List<List<int>>(); // Initialize outer list
    List<List<GameObject>> tileGrid = new List<List<GameObject>>(); // Initialize  lists
    public List<GameObject> spaceTiles = new List<GameObject>();
    public List<GameObject> rockyTiles = new List<GameObject>();
    public List<GameObject> resourceTiles = new List<GameObject>();


    float magnification = 7.0f; //4 - 20 are values recommended this zoomes in and out of the perlin depending on the need

    float xOffset = 0; // perlin noise shift in X axis
    float yOffset = 0; // perlin noise shift in Y axis

    public const string BUILDRESTILE = "BuildMaterilaResourceTile";
    public const string SPACETILETAG = "SpaceTile";
    public const string GASTILETAG = "GasResourceTile";


    private void Awake()
    {
        CreateTileset();

        CreateTileGroups();

        MixPerlinNoise();

        GenerateMap();

        
        
    }

    private void MixPerlinNoise()
    {
        float columnOffset = UnityEngine.Random.Range(-100, 100);
        float rowOffset = UnityEngine.Random.Range(-100, 100);

        float magg = UnityEngine.Random.Range(4.2f, 12.324f);

        magnification = magg;
        xOffset = columnOffset;
        yOffset = rowOffset;
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

                if (tileId == 0 || tileId == 2 || tileId == 3 || tileId == 4)
                {
                    tileId = CheckIfTrapped(x, y, tileGrid, tileId);
                }
                noiseGrid[x].Add(tileId);
                CreateTile(tileId, x, y);

                if (tileGrid[x][y].CompareTag(SPACETILETAG))
                {
                    spaceTiles.Add(tileGrid[x][y]);
                }
                if (tileGrid[x][y].CompareTag(BUILDRESTILE))
                {
                    rockyTiles.Add(tileGrid[x][y]);
                }
                if (tileGrid[x][y].CompareTag(GASTILETAG))
                {
                    resourceTiles.Add(tileGrid[x][y]);
                }

            }
        }
    }
    
    private int CheckIfTrapped(int x, int y, List<List<GameObject>> tileGrid, int tileId)
    {
        if(x == 0 || y == 0)
        {

            return tileId;
        }
        else
        {
            GameObject downTile = tileGrid[x][y - 1];
            GameObject leftTile = tileGrid[x - 1][y];
            
            if (downTile.CompareTag(BUILDRESTILE)){
                
                if (leftTile.CompareTag(BUILDRESTILE))
                {
     
                    return 1; }
                else
                { return tileId; }
            }
            else { return tileId; }
              
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
        tileset.Add(2, spaceVoidTile); //
        tileset.Add(3, spaceVoidTile); //
        tileset.Add(4, spaceVoidTile); //
        tileset.Add(5, gasResourceTiles);


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
    public GameObject GetUpTile(int x, int y)
    { 
        return tileGrid[x+1][y];
    }
    public GameObject GetDownTile(int x, int y)
    {
        return tileGrid[x-1][y];
    }
    public GameObject GetLeftTile(int x, int y)
    {
        return tileGrid[x][y-1];
    }
    public GameObject GetRightTile(int x, int y)
    {
        return tileGrid[x][y+1];
    }
    public void AddTile(int x, int y, GameObject tile)
    {
        tileGrid[x][y] = tile;
    }

}
