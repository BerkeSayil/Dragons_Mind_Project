using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    
    [SerializeField] PerlinBasedGridCreator gridder;
    private Vector2 startPos;
    bool once = true;

    private void LateUpdate()
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

        int spawnIndex = (int)Random.Range(0, tiles.Count);
        GameObject tile = tiles[spawnIndex];

        once = false;
        
        return new Vector2(tile.transform.position.x, tile.transform.position.y);



    }


}
