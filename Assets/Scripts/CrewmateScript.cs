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
    PerlinBasedGridCreator gridder;

    private void Start()
    {
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        path = gameObject.GetComponent<AIPath>();
        //Don't do this its very consuming
        gridder = FindObjectOfType<PerlinBasedGridCreator>();
        
    }
    public void MineTile(GameObject tile)
    {
        StartCoroutine(TileMinerCoroutine(tile));
        
    }
    IEnumerator TileMinerCoroutine(GameObject tileToMine)
    {
        
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
            GameObject space = Instantiate(spaceVoid, tile.transform.position, Quaternion.identity);
            Destroy(tile);
            notInPosition = true;

            gridder.AddTile((int)space.transform.position.x, (int)space.transform.position.y, space);
            
        }
        

    }

    public void SetDestination(Transform dest)
    {
        destinationSetter.target = dest;
    }
    
}
