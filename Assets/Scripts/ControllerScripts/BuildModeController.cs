using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{
    Tile.TileType buildModeTile = Tile.TileType.Floor;
    string buildModeObjectType;
    bool buildModeIsObjects = false;


    public void SetModeBuildFloor()
    {
        buildModeIsObjects = false;
        buildModeTile = Tile.TileType.Floor;
    }
    public void SetModeMineMode()
    {
        buildModeIsObjects = false;
        buildModeTile = Tile.TileType.Empty;

    }
    public void SetModeBuildInstalledObject( string objectType)
    {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;

    }

    public void DoBuild(Tile t) {

        if (buildModeIsObjects) {
            // Create the installed objects on the tile given. And assign.

            // Furniture type here exist in the scope so it prevents furniture changing mid build
            // you remember that problem... yeah this is the fix.

            string furnitureType = buildModeObjectType;

            // We check if it's a valid position and if so
            // create a job and queue it for something else to come and do.
            if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t)
                && t.pendingFurnitureJob == null

                ) {
                Job j = new Job(t, furnitureType, (theJob) =>
                {
                    WorldController.Instance.world.
                    PlaceFurnitureAt(furnitureType, theJob.tile);

                    t.pendingFurnitureJob = null;
                });


                // TODO: This being this way very easy to clear or forget make it automated in
                // some other way possible
                t.pendingFurnitureJob = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingFurnitureJob = null; });

                WorldController.Instance.world.jobQueue.Enqueue(j);


            }
        }
        else {
            t.Type = buildModeTile;
        }
    }

}
