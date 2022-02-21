using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Texture2D gameCursor;
    [SerializeField] AstarPath AiPathfinding;

    bool onMineMode = false;
    bool onBuildMode = false;
    bool mining = false;
    private int mineList = 0;

    [SerializeField] PerlinBasedGridCreator gridCreator;
    [SerializeField] ShipController spaceShip;

    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;

    private List<GameObject> tilesToMine = new List<GameObject>();

    private GameObject placeObject;

    void Start()
    {
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        AiPathfinding.Scan();

        StartCoroutine(MineTile(tilesToMine));
    }


    private void Update()
    {

        if (onMineMode)
        {
            
            //do mine mode stuff
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null)
                {
                    Debug.Log("Null collider");

                } else if (hit.collider.gameObject.CompareTag(PerlinBasedGridCreator.BUILDRESTILE) || hit.collider.gameObject.CompareTag(wallPrefab.tag))
                {
                    tilesToMine.Add(hit.collider.gameObject);
                    mining = true;
                }

            }
        }else if (onBuildMode)
        {

            //do build mode stuff

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
    IEnumerator MineTile(List<GameObject> toMine)
    {   
        while(true)
        {
            yield return new WaitUntil(() => mining);
            
            int randomizedMember = (int)Random.Range(0, spaceShip.crewMembers.Count);
            spaceShip.crewMembers[randomizedMember].GetComponent<CrewmateScript>().SetDestination(toMine[mineList].transform);

            yield return new WaitUntil(() => spaceShip.crewMembers[randomizedMember].GetComponent<CrewmateScript>().isReachedDestination());
            Instantiate(gridCreator.spaceVoidTile, toMine[mineList].transform.position, Quaternion.identity, gridCreator.transform);
            toMine[mineList].SetActive(false);
            AiPathfinding.Scan();
            mineList += 1;
            mining = false;

            Debug.Log(mineList);
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
