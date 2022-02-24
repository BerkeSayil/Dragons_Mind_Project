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
    [SerializeField] PerlinBasedGridCreator gridCreator;
    [SerializeField] ShipController spaceShip;

    //build mode items
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;


    //game manager
    private RaycastHit2D hit;
    private GameObject placeObject;
    bool onMineMode = false;
    bool onBuildMode = false;
    private List<GameObject> crewMembers = new List<GameObject>();

    void Start()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        AiPathfinding.Scan();

    }


    private void Update()
    {

        if (onMineMode) //UI mine mode
        {
            if (Input.GetMouseButtonDown(0)) //Clicking to mine
            {
                if (IsClickedOnTile()) // click is on tile [hit is the gameobject clicked]
                {
                    if ((hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.BUILDRESTILE)
                        || hit.collider.gameObject.CompareTag(wallPrefab.tag))) // is the tile wall or buildRes 
                    {
                        GameObject tileToMine = hit.collider.gameObject;
                        
                        //below checks if any of them is space tile if so its minable if not no 
                        
                        //mine
                        crewMembers = spaceShip.crewMembers;
                        float minDist = 900f;
                        GameObject crewClosest = null;
                        foreach (GameObject crew in crewMembers)
                        {
                            if(Vector2.Distance(crew.transform.position, tileToMine.transform.position) < minDist)
                            {
                                 minDist = Vector2.Distance(crew.transform.position, tileToMine.transform.position);
                                 crewClosest = crew;
                            }
                        }
                        if(crewClosest == null)
                        {
                            Debug.Log("Something wrong with crewmate celection");
                        }
                        else
                        {
                            crewClosest.GetComponent<CrewmateScript>().MineTile(tileToMine);
                        }

                    }
                }
            }
        }
        else if (onBuildMode) //UI build mode
        {
            if (Input.GetMouseButtonDown(0) && placeObject != null) 
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null)
                {
                    Debug.Log("Null collider");

                }
                else if (hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.SPACETILETAG))
                {
                    Instantiate(placeObject, hit.collider.transform.position, Quaternion.identity, gridCreator.transform);
                    hit.collider.gameObject.SetActive(false);
                    AiPathfinding.Scan();
                }
            }
        }
    }
    private bool IsClickedOnTile()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider == null)
        {
            Debug.Log("Null collider");
            return false;
        }
        else
        {
            return true;
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
    public void MineTileMode()
    {
        onMineMode = true;
    }
    public void BuildTileMode()
    {
        onBuildMode = true;
    }
    
    public void CancelModes()
    {
        onBuildMode = false;
        onMineMode = false;
    }
}
