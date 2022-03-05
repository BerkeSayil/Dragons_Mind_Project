using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //game manager related
    [SerializeField] Texture2D gameCursor;
    [SerializeField] AstarPath AiPathfinding;

    //world objects to access
    [SerializeField] public PerlinBasedGridCreator gridCreator;
    [SerializeField] ShipController spaceShip;

    //build mode prefabs
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject gasTapPrefab;
    [SerializeField] GameObject gasPipePrefab;
    [SerializeField] GameObject gasExhaustPrefab;

    //game manager
    private GameObject placeObject;
    bool onMineMode = false;
    bool onBuildMode = false;
    bool onDesignationMode = false;
    private List<GameObject> crewMembers = new List<GameObject>();


    //to give acces to another script
    public List<GameObject> minableTiles = new List<GameObject>();
    public List<GameObject> gasPipesTiles = new List<GameObject>();
    public List<GameObject> designatableTiles = new List<GameObject>();


    //multiple selection for build mode
    [SerializeField]GameObject selectionObj;
    private Vector2 startPos;

    List<GameObject> selectedTiles = new List<GameObject>();
    public List<GameObject> buildableTiles = new List<GameObject>();

    Image selectionImage;
    RectTransform selectionBox;

    void Start()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        AiPathfinding.Scan();
        minableTiles = gridCreator.rockyTiles;

        selectionBox = selectionObj.GetComponent<RectTransform>();
        selectionImage = selectionObj.GetComponent<Image>();
        buildableTiles = gridCreator.spaceTiles;
    }


    private void Update()
    {

        if (onMineMode) //UI mine mode
        {

                    
        }
        else if (onBuildMode) //UI build mode
        {

            //this shit will work in screen positions not particularly enjoying that
            if (selectionObj.activeInHierarchy && placeObject != null)
            {
                if (Input.GetMouseButtonDown(0)) // left click
                {
                    startPos = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0)) // left release
                {
                    ReleaseSelectionBox();
                }
                if (Input.GetMouseButton(0)) // left hold down
                {
                    UpdateSelectionBox(Input.mousePosition);
                }
            }
        }
        else if (onDesignationMode) //designation mode
        {
            

        }
        
    }
    public void SwitchTile(GameObject tileBuiltOn, GameObject tileToBuild)
    {
        GameObject tileBuilt = null;
        TileScript tile = tileToBuild.GetComponent<TileScript>();
        if (tile.isSpaceShipFloor)
        {
            tileBuilt = SwitchPrimer(tileBuiltOn, tileToBuild);

            designatableTiles.Add(tileBuilt);

        }
        else if (tile.isMinable && tile.isSpaceShipExterior)
        {
            tileBuilt = SwitchPrimer(tileBuiltOn, tileToBuild);

        }
        else if (tile.isGasPipe && ConnectedToPipes(tileBuiltOn))
        {
            tileBuilt = SwitchPrimer(tileBuiltOn, tileToBuild);
            gasPipesTiles.Add(tileBuilt);

        }
        else if (tile.isGasTap)
        {

            tileBuilt = SwitchPrimer(tileBuiltOn, tileToBuild);
        }
        else if (tile.isGasExhaust && ConnectedToPipes(tileBuiltOn))
        {
            tileBuilt = SwitchPrimer(tileBuiltOn, tileToBuild);
        }


        if (tile.isMinable)
        {
            minableTiles.Add(tileBuilt);
        }
    }

    private GameObject SwitchPrimer(GameObject tileBuiltOn, GameObject tileToBuild)
    {
        //Tile gets created space tile get deactivated and grid gets updated with the new tile. Pathfinding gets scanned
        //return a gameobject to be inclued in the lists
        GameObject tilePlaced = Instantiate(tileToBuild, tileBuiltOn.transform.position, Quaternion.identity, gridCreator.transform);
        gridCreator.AddTile((int)tilePlaced.transform.position.x, (int)tilePlaced.transform.position.y, tilePlaced);
        tileBuiltOn.SetActive(false);
        AiPathfinding.Scan();
        return tilePlaced;
    }
    
    private bool ConnectedToPipes(GameObject pipe)
    {
        //checks to see if any 4 direction has either a gas pipe or gas tap
       
        Vector2 pipePos = pipe.transform.position;
        GameObject upTile = gridCreator.GetUpTile((int)pipePos.x, (int)pipePos.y);
        GameObject downTile = gridCreator.GetDownTile((int)pipePos.x, (int)pipePos.y);
        GameObject leftTile = gridCreator.GetLeftTile((int)pipePos.x, (int)pipePos.y);
        GameObject rightTile = gridCreator.GetRightTile((int)pipePos.x, (int)pipePos.y);
        

        if (upTile.GetComponent<TileScript>().isGasTap || upTile.GetComponent<TileScript>().isSpaceShipExterior ||
           downTile.GetComponent<TileScript>().isGasTap || downTile.GetComponent<TileScript>().isSpaceShipExterior ||
           leftTile.GetComponent<TileScript>().isGasTap || leftTile.GetComponent<TileScript>().isSpaceShipExterior ||
           rightTile.GetComponent<TileScript>().isGasTap || rightTile.GetComponent<TileScript>().isSpaceShipExterior)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void UpdateSelectionBox(Vector2 currentPos)
    {
        if (selectionObj.activeInHierarchy) //build button activates and if active does the size calculations.
        {
            selectionImage.enabled = true;
            float width = currentPos.x - startPos.x;
            float height = currentPos.y - startPos.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

        }
    }
    void ReleaseSelectionBox()
    {
        selectionImage.enabled = false;

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (GameObject tiles in buildableTiles)
        {
            if (tiles != null)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(tiles.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    //we only get space tiles for this phase so they all should be buildable without a problem
                    selectedTiles.Add(tiles);
                }
            }


        }
        StartCoroutine(BuildToSelectedTiles(selectedTiles, placeObject));

        //remove them so don't overlap building
        foreach (GameObject tile in selectedTiles)
        {
            buildableTiles.Remove(tile);

        }

    }

    IEnumerator BuildToSelectedTiles(List<GameObject> selectedTiles, GameObject placeObject)
    {
        if (onBuildMode)
        {

            //input is a list of tilestobuild
            //first get a random crewmate
            //second get closest tilestobuild for that crewmate
            //third make crewmate build that tile
            //repeat between second and third for each

            crewMembers = spaceShip.crewMembers;
            int crewMemberIndex = (int)UnityEngine.Random.Range(0, crewMembers.Capacity);

            while (selectedTiles.Count != 0)
            {
                float minDist = 900f;
                GameObject tileClosest = null;
                foreach (GameObject tile in selectedTiles)
                {
                    //Finds the closest tile
                    if (Vector2.Distance(crewMembers[crewMemberIndex].transform.position, tile.transform.position) < minDist)
                    {
                        minDist = Vector2.Distance(crewMembers[crewMemberIndex].transform.position, tile.transform.position);
                        tileClosest = tile;
                    }
                }

                if (tileClosest == null)
                {
                    Debug.Log("Something wrong with tile celection");
                }
                else
                {
                    crewMembers[crewMemberIndex].GetComponent<CrewmateScript>().BuildTile(tileClosest, placeObject);
                    yield return null;
                }
                
                selectedTiles.Remove(tileClosest);
                AiPathfinding.Scan();

                yield return new WaitForSeconds(0.5f);
            }


        }

    }
    public void MineTiles(List<GameObject> tilesToMine)
    {
        StartCoroutine(MineTile(tilesToMine));

    }
    IEnumerator MineTile(List<GameObject> tilesToMine)
    {
        if (onMineMode)
        {

            //input is a list of tilestomine
            //first get a random crewmate
            //second get closest tilestomine for that crewmate
            //third make crewmate mine that tile
            //repeat between second and third for each

            crewMembers = spaceShip.crewMembers;
            int crewMemberIndex = (int)UnityEngine.Random.Range(0, crewMembers.Capacity);

            while (tilesToMine.Count != 0)
            {
                float minDist = 900f;
                GameObject tileClosest = null;
                foreach (GameObject tile in tilesToMine)
                {
                    //Finds the closest tile
                    if (Vector2.Distance(crewMembers[crewMemberIndex].transform.position, tile.transform.position) < minDist)
                    {
                        minDist = Vector2.Distance(crewMembers[crewMemberIndex].transform.position, tile.transform.position);
                        tileClosest = tile;
                    }
                }

                if (tileClosest == null)
                {
                    Debug.Log("Something wrong with tile celection");
                }
                else
                {
                    crewMembers[crewMemberIndex].GetComponent<CrewmateScript>().MineTile(tileClosest);
                }
                if (tileClosest.GetComponent<TileScript>().isSpaceShipFloor)
                {
                    designatableTiles.Remove(tileClosest);
                }
                tilesToMine.Remove(tileClosest);
                AiPathfinding.Scan();

                yield return new WaitForSeconds(0.5f);
            }



        }
    }


    //these changes occur when pressed from buttons

    public void PlaceWall()
    {
        placeObject = wallPrefab;
    }
    public void PlaceFloor()
    {
        placeObject = floorPrefab;
    }
    public void PlaceGasTap()
    {
        placeObject = gasTapPrefab;
    }
    public void PlaceGasPipe()
    {
        placeObject = gasPipePrefab;
    }
    public void PlaceGasExhaust()
    {
        placeObject = gasExhaustPrefab;
    }

    public void MineTileMode()
    {
        onMineMode = true;
    }
    public void BuildTileMode()
    {
        onBuildMode = true;
    }
    public void DesignationMode()
    {
        onDesignationMode = true;
    }
    public void CancelModes()
    {
        onBuildMode = false;
        onMineMode = false;
        onDesignationMode = false;
    }
}
