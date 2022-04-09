
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType construction = Job.JobType.Construction;
    private const Job.JobType constructionSecond = Job.JobType.ConstructionSecond;

    Inventory inventory;
    //TODO: For better performance this should be modified to be couroutine or async.

    public override Job PrioritizedJob(ArrayList jobsListTotal) { 
        // get construction jobs first
        // hauling inventories as second

        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        foreach (Job job in jobsListTotal) {

            if (job.jobOccupation == construction) jobsList.Add(job);
            //TODO: Implement dismantle jobs too and figure which will be priority?

            // Worker isn't carrying anything and it finds a hauling job so it picks up inventory
            if (job.jobOccupation == constructionSecond) {

                // this job is hauling job but I don't have the inventory at hand already
                if (job.haulingJob == false && inventory == null) jobsList.Add(job);

                if (inventory == null) continue;

                // this job is hauling job and I have the inventory at hand already
                if (job.haulingJob == true && job.inventory.objectType == inventory.objectType) jobsList.Add(job);
                
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

        if (j.haulingJob == false && j.inventory != null) inventory = j.inventory; // not a hauling job but a picking up job
        if (j.haulingJob == true) inventory = null; // if this is a hauling job character know it dropped off inventory

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.tile.x, j.tile.y);
        MyJob = null;

    }
}
