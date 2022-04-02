
using Pathfinding;
using System.Collections;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType construction = Job.JobType.Construction;


  
    public override Job PrioritizedJob(ArrayList jobsListTotal) { 
        /*
         * Check for the following criteria to understand who to prioritize
         * 
         * what is closer (closeness score ?)
         * what is mostImportant (construction, something of chaotic nature, ...)
         * this also should filter with character job in mind so we don't get another occupants jobs.
         * 
         */
        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        foreach (Job job in jobsListTotal) {

            if (job.jobOccupation == construction) jobsList.Add(job);
            //TODO: Implement dismantle jobs too and figure which will be priority?

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
