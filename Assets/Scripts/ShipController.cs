using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    
    [SerializeField] PerlinBasedGridCreator gridder;
    [SerializeField] GameObject crewmate;

    private Vector2 startPos;
    bool once = true;
    private int crewInsideShip = 3;

    public List<GameObject> crewMembers = new List<GameObject>(); //list of crewmembers


    private void Update()
    {
        if (once)
        {
            startPos = SpawnOnSpaceTile(gridder.spaceTiles);

            Camera.main.transform.position = new Vector3(startPos.x, startPos.y, Camera.main.transform.position.z);
            transform.position = startPos;

        }
        
    }

    private Vector2 SpawnOnSpaceTile(List<GameObject> tiles)
    {

        int spawnIndex = (int)Random.Range(0, tiles.Count-10);
        GameObject tile = tiles[spawnIndex];

        once = false;

        SpawnCrewmates(spawnIndex, tiles, crewInsideShip);

        return new Vector2(tile.transform.position.x, tile.transform.position.y);

    }


    private void SpawnCrewmates(int spaceShipIndex, List<GameObject> tiles, int crewmateAmount)
    {

        for (int i = 1; i < crewmateAmount + 1; i++)
        {
            Vector2 crewmatePos = tiles[spaceShipIndex + i].transform.position;

            GameObject crewMemberCreated = (GameObject)Instantiate(crewmate, crewmatePos, Quaternion.identity);

            crewMembers[i - 1] = crewMemberCreated;

        }

    }
    

}
