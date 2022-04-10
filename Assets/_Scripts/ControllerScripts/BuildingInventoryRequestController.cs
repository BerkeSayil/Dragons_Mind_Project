using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInventoryRequestController : MonoBehaviour {
    private BuildModeController _BmController;
    World world;

    private void Start() {
        _BmController = FindObjectOfType<BuildModeController>();

        _BmController.RegisterContructionJobCreated(OnRequestCreated);

        world = WorldController.Instance.world;
    }

    private void OnRequestCreated(Job job) {

        if (job.jobObjectType == null && job.jobTileType == Tile.TileType.Empty) return;

        // this means there was a job created in witch we want to build a thing using
        // job.object type so find a trade goods designation and deliver an inventory for this

        List<Designation> desigs = world.GetDesignationOfType(Designation.DesignationType.TradeGoods);

        Tile destination = null; // we can't deliver if no necessary designation is available

        if (desigs == null) return; // no available designation

        for (int i = 0; i < desigs.Count; i++) {
            foreach (Tile t in desigs[i].tiles) {
                // search for an empty spot in given designation
                if (t.looseObject == null && t.pendingHaulJob == null && world.IsHaulPlacementValid(t)) {
                    destination = t;
                    break;
                }
            }
            if (destination != null) break;
        }

        if (destination == null) return; // no available tile in designation

        //TODO: Implement a cool visualization for how we deliver this object (spacehip coming and dropping off ?)

        //TODO: Implement money cut and other managmental effects 

        // now we have a destination tile
        // I'll now place inventory we wanted to there

        if (job.jobOccupation == Job.JobType.Construction && job.haulingJob == false) {
            world.DeliverInventoryOnTile(job.jobObjectType, destination);
        }else

        if (job.jobTileType != Tile.TileType.Empty) {
            world.DeliverInventoryOnTile(job.jobTileType, destination);
        }
        // after this we'll create a haul job from this inventory to build job tile (as destination)

        //TODO: Fix this to represent invPrototype we want
        Job j = new Job(destination, true, world.inventoryPrototypes["Wall_Scrap"], (theJob) => {
            //call a what we want to run when this job gets finished

            Inventory.PickInventoryUp(destination); // removes inventory on given tile

            destination.pendingHaulJob = null;

        }, Job.JobType.Construction);

        destination.pendingHaulJob = j;
        j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingHaulJob = null; });

        WorldController.Instance.world.jobQueue.Enqueue(j);

        


        // we should also check as rules of hauling worker shouldn't build without required inventory is present on them


    }

}
