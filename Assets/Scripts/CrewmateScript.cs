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

    GameManager manager;
    PerlinBasedGridCreator gridder;

    private void Start()
    {
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        path = gameObject.GetComponent<AIPath>();


        //Don't do this its very consuming
        //memory eating potential
        gridder = FindObjectOfType<PerlinBasedGridCreator>();
        manager = FindObjectOfType<GameManager>();


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
    public void BuildTile(GameObject tileBuiltOn, GameObject tileBuilt)
    {
        StartCoroutine(TileBuilderCoroutine(tileBuiltOn, tileBuilt));

    }
    IEnumerator TileBuilderCoroutine(GameObject tileBuiltOn, GameObject tileBuilt)
    {

        while (notInPosition)
        {
            SetDestination(tileBuiltOn.transform);
            CheckIfInPosition();
            yield return null;
        }
        
        Build(tileBuiltOn, tileBuilt);
        yield return new WaitForSeconds(0.5f);
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
    private void Build(GameObject tileToBuildOn, GameObject tileToBuild)
    {
        if (tileToBuildOn != null)
        {
            //ozellestirilmis gas tap tarzi tillarin kontrolu yukarida
            if (tileToBuildOn.CompareTag(PerlinBasedGridCreator.GASTILETAG)
                && tileToBuild.GetComponent<TileScript>().isGasTap) // gas tap only placed on gas tile check
            {
                manager.SwitchTile(tileToBuildOn, tileToBuild);
            }
            else if (tileToBuildOn.CompareTag(PerlinBasedGridCreator.SPACETILETAG)
                && tileToBuild.GetComponent<TileScript>().isSpaceShipExterior) // spaceship exterior only placed on space tile check
            {
                manager.SwitchTile(tileToBuildOn, tileToBuild);
            }
            else if (tileToBuildOn.CompareTag(PerlinBasedGridCreator.SPACETILETAG)
               && tileToBuild.GetComponent<TileScript>().isSpaceShipFloor) // spaceship floor only placed on space tile check
            {
                manager.SwitchTile(tileToBuildOn, tileToBuild);
            }

            notInPosition = true;
            
            
        }


    }


    public void SetDestination(Transform dest)
    {
        destinationSetter.target = dest;
    }
    
}
