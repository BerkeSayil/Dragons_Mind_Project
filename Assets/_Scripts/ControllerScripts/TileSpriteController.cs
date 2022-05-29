using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TileSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite floorSprite;
    [SerializeField] private Sprite defaultEmptySprite;

    private Dictionary<Tile, GameObject> _tileGameObjectMap;


    private const int EmptyLayer = 6;
    private const int FloorLayer = 7;

    private static World World => WorldController.Instance.World;


    private void Start() {

        _tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a game-object for every tile so we have visual representation.
        for (var x = 0; x < World.Width; x++) {
            for (var y = 0; y < World.Height; y++) {
                Tile tileData = World.GetTileAt(x, y);

                GameObject tileGo = new GameObject();
                tileGo.name = "Tile_" + x + "_" + y;
                tileGo.transform.position = new Vector2(tileData.x, tileData.y);
                tileGo.transform.SetParent(this.transform, true);

                // adding a sprite renderer and giving it the defaultyEmpty sprite.
                tileGo.AddComponent<SpriteRenderer>();
                SpriteRenderer renderer = tileGo.GetComponent<SpriteRenderer>();
                renderer.sprite = defaultEmptySprite;
                renderer.sortingLayerName = "Tiles";

                // setting collisionBox layer for pathfinding.
                tileGo.layer = EmptyLayer;
                BoxCollider2D collider =  tileGo.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(1f, 1f);
                

                _tileGameObjectMap.Add(tileData, tileGo);

            }
        }

        World.RegisterTileChanged(OnTileChanged);
        
    }

    public Sprite GetSpriteForTile(Tile.TileType jobTileType)
    {
        //TODO: When you fix serialize field sprite things you will need to fix these too!

        return jobTileType switch
        {
            Tile.TileType.Empty => defaultEmptySprite,
            Tile.TileType.Floor => floorSprite,
            _ => null
            
        };
    }

    private void OnTileChanged(Tile tileData)
    {

        if (_tileGameObjectMap.ContainsKey(tileData) == false) {
            Debug.Log("tileGameObjectMap doesn't contain key");
            return;
        }

        GameObject tileGo = _tileGameObjectMap[tileData];

        switch (tileData.Type)
        {
            case Tile.TileType.Floor:
                // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
                tileGo.GetComponent<SpriteRenderer>().sprite = floorSprite;
                tileGo.layer = FloorLayer;
                break;
            case Tile.TileType.Empty:
                tileGo.GetComponent<SpriteRenderer>().sprite = defaultEmptySprite;
                tileGo.layer = EmptyLayer;
                break;
            default:
                Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
                break;
        }

        
    }

    public Tile GetTileAtCoord(Vector3 coordinates)
    {
        var x = Mathf.FloorToInt(coordinates.x);
        var y = Mathf.FloorToInt(coordinates.y);

        return World.GetTileAt(x, y);

    }
}
