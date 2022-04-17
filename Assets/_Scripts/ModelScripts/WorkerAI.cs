
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType Construction = Job.JobType.Construction;
    private const Job.JobType InventoryManagement = Job.JobType.InventoryManagement;
    private const Job.JobType Deconstruction = Job.JobType.Deconstruction;

    private Inventory _inventory;
    //TODO: For better performance this should be modified to be coroutine or async.

    protected override Job PrioritizedJob(ArrayList jobsListTotal) { 
        // get construction jobs first
        // hauling inventories as second

        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        
        // Type of jobs there are as priority
        // PickFrom Haul   => inventory null , job haul true
        // Carry Haul to BuildJob => this doesn't exist you do this when you build
        // Build => job job-object-type != null, inventory != null, inventory object type == job object type
        // Deconstruct => job object-type != null 
        // Pick inv => inventory null, job haul false
        // Carry inv to haul => inventory != null, job haul true
        
        foreach (Job job in jobsListTotal) {
            switch (job.JobOccupation)
            {
                case InventoryManagement when _inventory != null && job.HaulingJob == true: // carrying to haul
                    jobsList.Add(job);
                    continue;
                
                case InventoryManagement when _inventory == null && job.HaulingJob == false: // picking up from ground
                    jobsList.Add(job);
                    continue;
                //TODO: Fix so we get and use same objectType of inventory to building jobs
                case Construction when _inventory == null && job.HaulingJob == true: //wants to pick up from haul
                    jobsList.Add(job);
                    continue;
                case Construction:
                {
                    if (job.HaulingJob == false) { // want to build something
                        if (_inventory != null) {
                            jobsList.Add(job);
                            continue;
                        }
                    }

                    break;
                }
                case Deconstruction:
                    jobsList.Add(job);
                    continue;
            }
        }

        if (jobsList.Count == 0) return null;

        float minDist = Mathf.Infinity;
        Job minDistJob = null;

        foreach (Job job in jobsList)
        {
            if (!IsPathPossible(job)) continue;
            
            float distanceToJob = Vector2.Distance
                (new Vector2(transform.position.x, transform.position.y), new Vector2(job.Tile.x, job.Tile.y));

            if (!(distanceToJob < minDist)) continue;
            
            minDist = distanceToJob;
            minDistJob = job;

        }
        if (minDistJob == null) return null;

        WorldController.Instance.World.JobQueue.RemoveMyJob(minDistJob);

        //TODO: make a callback character sprite changed to also display a smaller version of inventory at our hand

        return minDistJob;
    }

    protected override void OnJobEnded(Job j) {
        if (j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }
        

        switch (j.JobOccupation)
        {
            case Job.JobType.InventoryManagement:
            {
                if (j.HaulingJob == true) _inventory = null;
                if (j.HaulingJob == false) _inventory = j.Inventory;
                break;
            }
            case Job.JobType.Construction:
            {
                if (j.HaulingJob == true) _inventory = j.Inventory;
                if (j.HaulingJob == false) _inventory = null;
                break;
            }
        }

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.Tile.x, j.Tile.y);
        MyJob = null;

    }
}
