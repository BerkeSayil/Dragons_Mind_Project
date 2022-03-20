using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public float atmosO2 = 0;
    public float atmosN2 = 0;
    public float atmosCO2 = 0;

    List<Tile> tiles;

    public Room() {
        tiles = new List<Tile>();
    }

    public void AssignTile(Tile t) {
        if (tiles.Contains(t)) {
            return;
        }
        if(t.room != null) { 
            // belongs to another room.
            t.room.tiles.Remove(t);
        }

        t.room = this;
        tiles.Add(t);

    }

    public void UnassignAllTiles() {
        for (int i = 0; i < tiles.Count; i++) {
            tiles[i].room = tiles[i].World.GetOutsideRoom(); // Assigns outside
        }

        tiles = new List<Tile>(); // cleared

    }

    public static void DoFloodFillRoom(Furniture sourceFurniture) {
        // source furniture is the hypoteticall furniture that creates / seperates the rooms.

        World world = sourceFurniture.tile.World;

        Room oldRoom = sourceFurniture.tile.room;

        // try to build a new room starting from north
            ActualFloodFill(sourceFurniture.tile.North(), oldRoom);
            ActualFloodFill(sourceFurniture.tile.South(), oldRoom);
            ActualFloodFill(sourceFurniture.tile.East(), oldRoom);
            ActualFloodFill(sourceFurniture.tile.West(), oldRoom);


        sourceFurniture.tile.room = null; // wall is not in any room ?
        oldRoom.tiles.Remove(sourceFurniture.tile);

        // Check adjecent furns 
        // outside is one big room called room 0 so always assume we have at least 1
        // (might be null room is outside but having 0 th room might be good too)
        // delete the room assigned to the furn then check floodfill discover any new rooms.

        // we know this rooms tiles .
        

        if(sourceFurniture.tile.room != world.GetOutsideRoom()) {
            // unassignes to become an outside room and then removes this room
            
            world.DeleteRooms(oldRoom);
        }

    }


    protected static void ActualFloodFill(Tile tile, Room oldRoom) {
        if (tile == null) {
            return; // we try to flood fill off the map.
        }

        if (tile.room != oldRoom) {
            // means we already identified that "new" room 
            return;
        }

        if (tile.furniture != null && tile.furniture.stationExterior) {
            // this is an exterior piece next to our tile probably continuation of wall or door or whatever.
            return;
        }

        if (tile.Type == Tile.TileType.Empty) {
            // this is empty space vacum and must remain as outside
            return;
        }

        // at this point we know we need a new room.

        Room newRoom = new Room();

        Queue<Tile> tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(tile);

        while (tilesToCheck.Count > 0) {
            Tile t = tilesToCheck.Dequeue();


            if (t.room == oldRoom) {
                newRoom.AssignTile(t);

                Tile t2;

                // we hit open space by edge of map or empty tile
                // this room is actually part of outside so bail out early
                // it takes a lot of unneccasry time otherwise
                // delete the new room and re assign all tiles to outside
                // repeated for every immediate neighboor

                t2 = t.North();

                    if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                        newRoom.UnassignAllTiles();
                        return;
                    }

                    if (t2 != null && t2.room == oldRoom && (t2.furniture == null || t2.furniture.stationExterior == false)) {
                        tilesToCheck.Enqueue(t2);
                    }

                t2 = t.South();

                    if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                        newRoom.UnassignAllTiles();
                        return;
                    }
                    if (t2 != null && t2.room == oldRoom && (t2.furniture == null || t2.furniture.stationExterior == false)) {
                        tilesToCheck.Enqueue(t2);
                    }

                t2 = t.East();

                    if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                        newRoom.UnassignAllTiles();
                        return;
                    }
                    if (t2 != null && t2.room == oldRoom && (t2.furniture == null || t2.furniture.stationExterior == false)) {
                        tilesToCheck.Enqueue(t2);
                    }

                t2 = t.West();

                    if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                        newRoom.UnassignAllTiles();
                        return;
                    }

                    if (t2 != null && t2.room == oldRoom && (t2.furniture == null || t2.furniture.stationExterior == false)) {
                        tilesToCheck.Enqueue(t2);
                    }


            }
        }

        // Tell the world a new room has been created.
        tile.World.AddRoom(newRoom);

    }
    
}
