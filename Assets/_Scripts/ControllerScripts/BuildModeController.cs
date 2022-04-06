using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{
    Tile.TileType buildModeTile = Tile.TileType.Empty;
    string buildModeObjectType;
    bool buildModeIsFurniture = false;
    public bool areWeBuilding = false;

    
    public void SetModeBuildFloor()
    {
        buildModeIsFurniture = false;
        buildModeTile = Tile.TileType.Floor;
    }
    public void SetModeMineMode()
    {
        buildModeIsFurniture = false;
        buildModeTile = Tile.TileType.Empty;

    }
    public void UninstallInstalledObject() {

        buildModeIsFurniture = false;
        // This check the marked tiles for furniture objects
        // Removes them
        // Creates an inventory (looseObject) in the given tile
        // Which should in turn would get hauled back to tradegoods designation if any exists in designations list ?




    }
    public void SetModeBuildInstalledObject( string objectType)
    {
        buildModeIsFurniture = true;
        buildModeObjectType = objectType;
        
    }

    public void DoBuild(Tile t) {

        if (buildModeIsFurniture) {
            // Create the installed objects on the tile given. And assign.

            // Furniture type here exist in the scope so it prevents furniture changing mid build
            // you remember that problem... yeah this is the fix.

            string furnitureType = buildModeObjectType;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t)
                && t.pendingFurnitureJob == null

                ) {
                Job j = new Job(t, furnitureType, (theJob) => {
                    WorldController.Instance.world.
                    PlaceFurnitureAt(furnitureType, theJob.tile);

                    t.pendingFurnitureJob = null;
                },
                Job.JobType.Construction
                );


                // TODO: This being this way very easy to clear or forget make it automated in
                // some other way possible
                t.pendingFurnitureJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

                WorldController.Instance.world.jobQueue.Enqueue(j);


            }
        }else if (buildModeTile == Tile.TileType.Floor) {
            // mid change fix
            Tile.TileType buildModeTile = this.buildModeTile;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (WorldController.Instance.world.IsTilePlacementValid(buildModeTile, t)
                && t.pendingTileJob == null) {
                Job j = new Job(t, buildModeTile, (theJob) => {
                    WorldController.Instance.world.
                    PlaceTileAt(buildModeTile, theJob.tile);

                    t.pendingTileJob = null;
                },
                Job.JobType.Construction
                );


                // TODO: This being this way very easy to clear or forget make it automated in some other way possible
                t.pendingTileJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingTileJob = null; });

                WorldController.Instance.world.jobQueue.Enqueue(j);


            }
        }
        else if (buildModeTile == Tile.TileType.Empty) {

            // mid change fix
            Tile.TileType buildModeTile = this.buildModeTile;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (t.furniture != null) {
                // dismantle furniture and create an inventory in its place.

                string furnitureType = t.furniture.objectType;

                if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t) == false
                    && t.pendingFurnitureJob == null
                    ) {
                    Job j = new Job(t, furnitureType, (theJob) => {
                        WorldController.Instance.world.
                        RemoveFurnitureAt(furnitureType, theJob.tile);

                        t.pendingFurnitureJob = null;
                    },
                    Job.JobType.Construction
                    );


                    // TODO: This being this way very easy to clear or forget make it automated in
                    // some other way possible
                    t.pendingFurnitureJob = j;
                    j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

                    WorldController.Instance.world.jobQueue.Enqueue(j);



                }
                else if (WorldController.Instance.world.IsTilePlacementValid(buildModeTile, t)
                && t.pendingTileJob == null) {
                    Job j = new Job(t, buildModeTile, (theJob) => {
                        WorldController.Instance.world.
                        PlaceTileAt(buildModeTile, theJob.tile);

                        t.pendingTileJob = null;
                    },
                    Job.JobType.Construction
                    );


                    // TODO: This being this way very easy to clear or forget make it automated in some other way possible
                    t.pendingTileJob = j;
                    j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingTileJob = null; });

                    WorldController.Instance.world.jobQueue.Enqueue(j);


                }
            }

            //t.Type = buildModeTile;

        }
    }
    public void SetBuildingMode(bool isBuilding) {
        areWeBuilding = isBuilding;
    }
}
