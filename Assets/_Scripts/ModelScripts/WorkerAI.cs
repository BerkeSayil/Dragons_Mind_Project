
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType construction = Job.JobType.Construction;
    private const Job.JobType hauling = Job.JobType.ConstructionSecond;


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
            if ( job.jobOccupation == hauling) jobsList.Add(job);
              
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

        return minDistJob;
    }
}
