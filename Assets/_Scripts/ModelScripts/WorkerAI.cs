
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType construction = Job.JobType.Construction;
    private const Job.JobType inventoryManagement = Job.JobType.InventoryManagement;
    private const Job.JobType deconstruction = Job.JobType.Deconstruction;

    Inventory inventory;
    //TODO: For better performance this should be modified to be couroutine or async.

    public override Job PrioritizedJob(ArrayList jobsListTotal) { 
        // get construction jobs first
        // hauling inventories as second

        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        foreach (Job job in jobsListTotal) {

            // Type of jobs there are as priority
            // PickFrom Haul   => inventory null , job haul true
            // Carry Haul to BuildJob => this doesn't exist you do this when you build
            // Build => job jobobjecttype != null, inventory != null, inventory object type == job object type
            // Deconstruct => job objecttype != null 
            // Pick inv => inventory null, job haul false
            // Carry inv to haul => inventory != null, job haul true



            if (job.JobOccupation == inventoryManagement) {

                if(inventory != null && job.HaulingJob == true) { // carrying to haul
                    jobsList.Add(job);
                    continue;
                }

                if (inventory == null && job.HaulingJob == false) { // picking up from ground
                    jobsList.Add(job);
                    continue;
                }


            }

            if (job.JobOccupation == construction) {
                //TODO: Fix so we get and use same objectType of inventory to building jobs

                if (inventory == null && job.HaulingJob == true) { //wants to pick up from haul
                    jobsList.Add(job);
                    continue;
                }

                if (job.HaulingJob == false) { // want to build something
                    if (inventory != null) {
                        jobsList.Add(job);
                        continue;
                    }
                }

            }

            if (job.JobOccupation == deconstruction) {

                jobsList.Add(job);
                continue;
                

            }
        }

        if (jobsList.Count == 0) return null;

        float minDist = Mathf.Infinity;
        Job minDistJob = null;

        foreach (Job job in jobsList) {

            if (IsPathPossible(job)) {

                float distanceToJob = Vector2.Distance
                            (new Vector2(transform.position.x, transform.position.y), new Vector2(job.Tile.x, job.Tile.y));

                if (distanceToJob < minDist) {
                    minDist = distanceToJob;
                    minDistJob = job;
                }

            }

        }
        if (minDistJob == null) return null;

        WorldController.Instance.World.JobQueue.RemoveMyJob(minDistJob);

        //TODO: make a callback charachter sprite changed to also display a smaller version of inventory at our hand

        return minDistJob;
    }

    public override void OnJobEnded(Job j) {
        if (j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }
        

        if(j.JobOccupation == Job.JobType.InventoryManagement) {

            if (j.HaulingJob == true) inventory = null;
            if (j.HaulingJob == false) inventory = j.Inventory;

        }
        else if (j.JobOccupation == Job.JobType.Construction) {

            if (j.HaulingJob == true) inventory = j.Inventory;
            if (j.HaulingJob == false) inventory = null;
        }

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.Tile.x, j.Tile.y);
        MyJob = null;

    }
}
