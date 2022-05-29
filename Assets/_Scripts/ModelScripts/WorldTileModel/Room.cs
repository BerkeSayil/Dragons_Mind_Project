using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public float AtmosO2 = 0;
    public float AtmosN2 = 0;
    public float AtmosCo2 = 0;

    private List<Tile> _tiles;
    

    public Room() {
        _tiles = new List<Tile>();
       
    }

    public void AssignTile(Tile t) {
        if (_tiles.Contains(t)) {
            return;
        }

        // belongs to another room.
        // ? is a check for null... you learn something everyday
        t.Room?._tiles.Remove(t);

        t.Room = this;
        _tiles.Add(t);

    }

    public void UnassignAllTiles()
    {
        foreach (var t in _tiles)
        {
            t.Room = t.World.GetOutsideRoom(); // Assigns outside
        }

        _tiles = new List<Tile>(); // cleared
    }

    public static void DoFloodFillRoom(Furniture sourceFurniture) {
        // source furniture is the hypothetical furniture that creates / separates the rooms.

        World world = sourceFurniture.Tile.World;

        Room oldRoom = sourceFurniture.Tile.Room;

        // try to build a new room starting from north
            ActualFloodFill(sourceFurniture.Tile.North(), oldRoom);
            ActualFloodFill(sourceFurniture.Tile.South(), oldRoom);
            ActualFloodFill(sourceFurniture.Tile.East(), oldRoom);
            ActualFloodFill(sourceFurniture.Tile.West(), oldRoom);


        sourceFurniture.Tile.Room = null; // wall is not in any room ?
        oldRoom._tiles.Remove(sourceFurniture.Tile);

        // Check adjacent furns 
        // outside is one big room called room 0 so always assume we have at least 1
        // (might be null room is outside but having 0 th room might be good too)
        // delete the room assigned to the furn then check flood-fill discover any new rooms.

        // we know this rooms tiles .
        

        if(sourceFurniture.Tile.Room != world.GetOutsideRoom()) {
            // unassigned to become an outside room and then removes this room
            
            world.DeleteRooms(oldRoom);
        }

    }
    public static void DoReverseFloodFillRoom(List<Room> neighboorRoomsList, Room minIndexedRoom) {

        
        World world = WorldController.Instance.World;

        foreach (Room room in neighboorRoomsList)
        {
            if (room.Equals(minIndexedRoom)) continue;
            
            // this room is not the min index room so all of these tiles should become minindexroom's
            // also delete this room if it's not outsideRoom 
            if(room.Equals(world.GetOutsideRoom()) == false) {
                    
                List<Tile> tilesToReassign = new List<Tile>(room._tiles);

                foreach (Tile tile in tilesToReassign) {
                    minIndexedRoom.AssignTile(tile);
                }

                world.DeleteRooms(room);
            }
            else {
                // this is not min index but outside room

                foreach (Tile tile in room._tiles) {
                    minIndexedRoom.AssignTile(tile);
                }
            }

        }


    }


    private static void ActualFloodFill(Tile tile, Room oldRoom) {
        if (tile == null) {
            return; // we try to flood fill off the map.
        }
        //TODO: This kinda be not working when we break walls.
        if (tile.Room != oldRoom) {
            // means we already identified that "new" room 
            return;
        }

        if (tile.Furniture != null && tile.Furniture.StationExterior) {
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

            if (t == null) break;
        
            if (t.Room != oldRoom) continue;
            
            newRoom.AssignTile(t);

            Tile t2;

            // we hit open space by edge of map or empty tile
            // this room is actually part of outside so bail out early
            // it takes a lot of unnecessary time otherwise
            // delete the new room and re assign all tiles to outside
            // repeated for every immediate neighbor

            t2 = t.North();

            if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                newRoom.UnassignAllTiles();
                return;
            }

            if (t2.Room == oldRoom && (t2.Furniture == null || t2.Furniture.StationExterior == false)) {
                tilesToCheck.Enqueue(t2);
            }

            t2 = t.South();

            if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                newRoom.UnassignAllTiles();
                return;
            }
            if (t2.Room == oldRoom && (t2.Furniture == null || t2.Furniture.StationExterior == false)) {
                tilesToCheck.Enqueue(t2);
            }

            t2 = t.East();

            if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                newRoom.UnassignAllTiles();
                return;
            }
            if (t2.Room == oldRoom && (t2.Furniture == null || t2.Furniture.StationExterior == false)) {
                tilesToCheck.Enqueue(t2);
            }

            t2 = t.West();

            if (t2 == null || t2.Type == Tile.TileType.Empty) {
                    
                newRoom.UnassignAllTiles();
                return;
            }

            if (t2.Room == oldRoom && (t2.Furniture == null || t2.Furniture.StationExterior == false)) {
                tilesToCheck.Enqueue(t2);
            }
        }
        newRoom.AtmosO2 = oldRoom.AtmosO2;
        newRoom.AtmosCo2 = oldRoom.AtmosCo2;
        newRoom.AtmosN2 = oldRoom.AtmosN2;


        // Tell the world a new room has been created.
        tile.World.AddRoom(newRoom);

    }

    
    public bool Equals(Room obj) {
        
        return _tiles.Equals(obj._tiles);
    }


}
