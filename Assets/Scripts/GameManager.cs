using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<GameObject> floorTiles = new List<GameObject>();


    void Start()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        AiPathfinding.Scan();
        minableTiles = gridCreator.rockyTiles;

    }


    private void Update()
    {

        if (onMineMode) //UI mine mode
        {

                    
        }
        else if (onBuildMode) //UI build mode
        {
            if (Input.GetMouseButtonDown(0) && placeObject != null) 
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    // code here only executes if the raycast hit an object in a valid layer and is NOT over a UI object
                    if (hit.collider == null)
                    {
                        Debug.Log("Null collider");

                    }//ozellestirilmis gas tap tarzi tillarin kontrolu yukarida
                    else if (hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.GASTILETAG)
                        && placeObject.GetComponent<TileScript>().isGasTap)
                    {
                        SwitchTile(hit);
                    }
                    else if (hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.SPACETILETAG)
                        && placeObject.GetComponent<TileScript>().isSpaceShipExterior)
                    {
                        SwitchTile(hit);
                    }
                }
                
            }
        }
        if (onDesignationMode) //designation mode
        {


        }
    }

    private void SwitchTile(RaycastHit2D hit)
    {
        TileScript tile = placeObject.GetComponent<TileScript>();
        if (tile.isMinable)
        {
            GameObject tilePlaced = SwitchPrimer(hit);
            minableTiles.Add(tilePlaced);

        }else if (tile.isSpaceShipExterior && ConnectedToPipes(hit.collider.gameObject))
        {
            GameObject tilePlaced = SwitchPrimer(hit);
            gasPipesTiles.Add(tilePlaced);

        }
        else if(tile.isGasTap){

            SwitchPrimer(hit);
        }
        else if (tile.isGasExhaust && ConnectedToPipes(hit.collider.gameObject))
        {

            SwitchPrimer(hit);
        }else if (tile.isSpaceShipFloor)
        {
            GameObject tilePlaced = SwitchPrimer(hit);
            floorTiles.Add(tilePlaced);
        }

    }

    private GameObject SwitchPrimer(RaycastHit2D hit)
    {
        //Tile gets created space tile get deactivated and grid gets updated with the new tile. Pathfinding gets scanned
        //return a gameobject to be inclued in the lists
        GameObject tilePlaced = Instantiate(placeObject, hit.collider.transform.position, Quaternion.identity, gridCreator.transform);
        gridCreator.AddTile((int)tilePlaced.transform.position.x, (int)tilePlaced.transform.position.y, tilePlaced);
        hit.collider.gameObject.SetActive(false);
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

            while(tilesToMine.Count != 0)
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

                tilesToMine.Remove(tileClosest);
                AiPathfinding.Scan();

                yield return new WaitForSeconds(0.5f);
            }
            


        }
    }

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
