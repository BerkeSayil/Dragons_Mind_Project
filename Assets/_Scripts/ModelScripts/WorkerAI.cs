
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



            if (job.jobOccupation == inventoryManagement) {

                if(inventory != null && job.haulingJob == true) { // carrying to haul
                    jobsList.Add(job);
                    continue;
                }

                if (inventory == null && job.haulingJob == false) { // picking up from ground
                    jobsList.Add(job);
                    continue;
                }


            }

            if (job.jobOccupation == construction) {
                //TODO: Fix so we get and use same objectType of inventory to building jobs

                if (inventory == null && job.haulingJob == true) { //wants to pick up from haul
                    jobsList.Add(job);
                    continue;
                }

                if (job.haulingJob == false) { // want to build something
                    if (inventory != null) {
                        jobsList.Add(job);
                        continue;
                    }
                }

            }

            if (job.jobOccupation == deconstruction) {

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
                            (new Vector2(transform.position.x, transform.position.y), new Vector2(job.tile.x, job.tile.y));

                if (distanceToJob < minDist) {
                    minDist = distanceToJob;
                    minDistJob = job;
                }

            }

        }
        if (minDistJob == null) return null;

        WorldController.Instance.world.jobQueue.RemoveMyJob(minDistJob);

        //TODO: make a callback charachter sprite changed to also display a smaller version of inventory at our hand

        return minDistJob;
    }

    public override void OnJobEnded(Job j) {
        if (j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }
        

        if(j.jobOccupation == Job.JobType.InventoryManagement) {

            if (j.haulingJob == true) inventory = null;
            if (j.haulingJob == false) inventory = j.inventory;

        }
        else if (j.jobOccupation == Job.JobType.Construction) {

            if (j.haulingJob == true) inventory = j.inventory;
            if (j.haulingJob == false) inventory = null;
        }

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.tile.x, j.tile.y);
        MyJob = null;

    }
}
