using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInventoryRequestController : MonoBehaviour {
    
    private BuildModeController _bmController;
    private World _world;

    private void Start() {
        _bmController = FindObjectOfType<BuildModeController>();

        _bmController.RegisterContructionJobCreated(OnRequestCreated);

        _world = WorldController.Instance.World;
    }

    private void OnRequestCreated(Job job) {

        if (job.JobObjectType == null && job.JobTileType == Tile.TileType.Empty) return;

        // this means there was a job created in witch we want to build a thing using
        // job.object type so find a trade goods designation and deliver an inventory for this

        List<Designation> desigs = _world.GetDesignationOfType(Designation.DesignationType.TradeGoods);

        Tile destination = null; // we can't deliver if no necessary designation is available

        if (desigs == null) _world.ReturnToSender(job); // no available so we requeue

        for (int i = 0; i < desigs.Count; i++) {
            foreach (Tile t in desigs[i].Tiles)
            {
                // search for an empty spot in given designation
                if (t.looseObject != null || t.pendingHaulJob != null || !_world.IsHaulPlacementValid(t)) continue;
                
                // it's empty 
                destination = t;
                break;
            }
            if (destination != null) break;
        }

        if (destination == null) {
            Debug.LogError("No available tile to place inventory");

            _world.ReturnToSender(job);
            return;
        }

        //TODO: Implement a cool visualization for how we deliver this object (spacehip coming and dropping off ?)

        //TODO: Implement money cut and other managmental effects 

        // now we have a destination tile
        // I'll now place inventory we wanted to there

        if (job.JobOccupation == Job.JobType.Construction && job.HaulingJob == false) {
            _world.DeliverInventoryOnTile(job.JobObjectType, destination);
        }else

        if (job.JobTileType != Tile.TileType.Empty) {
            _world.DeliverInventoryOnTile(job.JobTileType, destination);
        }
        // after this we'll create a haul job from this inventory to build job tile (as destination)

        //TODO: Fix this to represent invPrototype we want
        Job j = new Job(destination, true, _world.InventoryPrototypes["Wall_Scrap"], (theJob) => {
            //call a what we want to run when this job gets finished

            Inventory.PickInventoryUp(destination); // removes inventory on given tile

            destination.pendingHaulJob = null;

        }, Job.JobType.Construction);

        destination.pendingHaulJob = j;
        j.RegisterJobCancelCallback((theJob) => { theJob.Tile.pendingHaulJob = null; });

        WorldController.Instance.World.JobQueue.Enqueue(j);

        


        // we should also check as rules of hauling worker shouldn't build without required inventory is present on them


    }

}
