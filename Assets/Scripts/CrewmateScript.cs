using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class CrewmateScript : MonoBehaviour
{
    AIDestinationSetter destinationSetter;
    AIPath path;

    private bool notInPosition = true;

    [SerializeField] GameObject spaceVoid;
    
    private void Start()
    {
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        path = gameObject.GetComponent<AIPath>();
        
    }
    public void MineTile(GameObject tile)
    {
        StartCoroutine(TileMinerCoroutine(tile));
        
    }
    IEnumerator TileMinerCoroutine(GameObject tileToMine)
    {
        //TO DO type coroutine structure
        
        while (notInPosition)
        {
            SetDestination(tileToMine.transform);
            CheckIfInPosition();
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Mine(tileToMine);
    }
    private void CheckIfInPosition()
    {
        if (Vector2.Distance(gameObject.transform.position, destinationSetter.target.position) < 1.5f)
        {
            notInPosition = false;
        }
    }

    private void Mine(GameObject tile)
    {
        if(tile != null)
        {
            Instantiate(spaceVoid, tile.transform.position, Quaternion.identity);

            Destroy(tile);

            notInPosition = true;
        }
        

    }

    public void SetDestination(Transform dest)
    {
        destinationSetter.target = dest;
    }
    
}
