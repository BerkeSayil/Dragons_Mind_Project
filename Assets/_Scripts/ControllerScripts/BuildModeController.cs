using System;
using UnityEngine;

public class BuildModeController : MonoBehaviour
{   
    //TODO: This method being a async or coroutine would benefit us in fps very generously

    private Tile.TileType _buildModeTile = Tile.TileType.Empty;
    private string _buildModeObjectType;
    private bool _buildModeIsFurniture = false;
    public bool areWeBuilding = false;

    private Action<Job> _cbConstructionJobRequested;
    public void SetModeBuildFloor()
    {
        _buildModeIsFurniture = false;
        _buildModeTile = Tile.TileType.Floor;
    }
    public void SetModeMineMode()
    {
        _buildModeIsFurniture = false;
        _buildModeTile = Tile.TileType.Empty;

    }
    public void UninstallInstalledObject() {

        _buildModeIsFurniture = false;
        // This check the marked tiles for furniture objects
        // Removes them
        




    }
    public void SetModeBuildInstalledObject( string objectType)
    {
        _buildModeIsFurniture = true;
        _buildModeObjectType = objectType;
        
    }

    public void DoBuild(Tile t) {
        

        if (_buildModeIsFurniture) {
            // Create the installed objects on the tile given. And assign.

            // Furniture type here exist in the scope so it prevents furniture changing mid build
            // you remember that problem... yeah this is the fix.

            string furnitureType = _buildModeObjectType;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (WorldController.Instance.World.IsFurniturePlacementValid(furnitureType, t)
                && t.PendingFurnitureJob == null

                ) {
                Job j = new Job(t, furnitureType, (theJob) => {
                    WorldController.Instance.World.
                    PlaceFurnitureAt(furnitureType, theJob.Tile);

                    t.PendingFurnitureJob = null;
                },
                Job.JobType.Construction
                );


                // TODO: This being this way very easy to clear or forget make it automated in
                // some other way possible
                t.PendingFurnitureJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.Tile.PendingFurnitureJob = null; });

                WorldController.Instance.World.JobQueue.Enqueue(j);

                if (_cbConstructionJobRequested != null) {
                    _cbConstructionJobRequested(j);
                }

            }
        }
        if (_buildModeTile == Tile.TileType.Floor && _buildModeIsFurniture == false) {
            // mid change fix
            Tile.TileType buildModeTile = this._buildModeTile;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (WorldController.Instance.World.IsTilePlacementValid(buildModeTile, t)
                && t.PendingTileJob == null) {
                Job j = new Job(t, buildModeTile, (theJob) => {
                    WorldController.Instance.World.
                    PlaceTileAt(buildModeTile, theJob.Tile);

                    t.PendingTileJob = null;
                },
                Job.JobType.Construction
                );


                // TODO: This being this way very easy to clear or forget make it automated in some other way possible
                t.PendingTileJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.Tile.PendingTileJob = null; });

                WorldController.Instance.World.JobQueue.Enqueue(j);

                if (_cbConstructionJobRequested != null) {
                    _cbConstructionJobRequested(j);
                }
            }
        }
        if (_buildModeTile == Tile.TileType.Empty && _buildModeIsFurniture == false) {

            // mid change fix
            Tile.TileType buildModeTile = this._buildModeTile;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (t.Furniture != null) {
                // dismantle furniture and create an inventory in its place.

                string furnitureType = t.Furniture.ObjectType;

                // are we removing furniture ?
                if (WorldController.Instance.World.IsFurniturePlacementValid(furnitureType, t) == false
                    && t.PendingFurnitureJob == null
                    ) {
                    Job j = new Job(t, furnitureType, (theJob) => {
                        WorldController.Instance.World.
                        RemoveFurnitureAt(furnitureType, theJob.Tile);

                        t.PendingFurnitureJob = null;
                    },
                    Job.JobType.Deconstruction
                    );
                    
                    t.PendingFurnitureJob = j;
                    j.RegisterJobCancelCallback((theJob) => { theJob.Tile.PendingFurnitureJob = null; });

                    WorldController.Instance.World.JobQueue.Enqueue(j);



                }// are we removing tile ? 
                //TODO: Removing Tile doesn't work
                else if (WorldController.Instance.World.IsTilePlacementValid(buildModeTile, t) == false
                && t.PendingTileJob == null) {
                    Job j = new Job(t, buildModeTile, (theJob) => {
                        WorldController.Instance.World.
                        PlaceTileAt(buildModeTile, theJob.Tile);

                        t.PendingTileJob = null;
                    },
                    Job.JobType.Construction
                    );


                    // TODO: This being this way very easy to clear or forget make it automated in some other way possible
                    t.PendingTileJob = j;
                    j.RegisterJobCancelCallback((theJob) => { theJob.Tile.PendingTileJob = null; });

                    WorldController.Instance.World.JobQueue.Enqueue(j);


                }


            }

            //t.Type = buildModeTile;

        }
    }
    public void SetBuildingMode(bool isBuilding) {
        areWeBuilding = isBuilding;
    }
    public void RegisterContructionJobCreated(Action<Job> callbackFunc) {
        _cbConstructionJobRequested += callbackFunc;
    }
    public void UnregisterContructionJobCreated(Action<Job> callbackFunc) {
        _cbConstructionJobRequested -= callbackFunc;
    }

}
