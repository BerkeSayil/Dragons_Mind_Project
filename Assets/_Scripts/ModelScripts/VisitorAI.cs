using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorAI : Character
{
    private const Job.JobType visitor = Job.JobType.Visitor;

    float Money;
    float Anonymity;

    //TODO: For better performance this should be modified to be couroutine or async.
    protected override Job PrioritizedJob(ArrayList jobsListTotal) {
        

        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        foreach (Job job in jobsListTotal) {

            /* 
            * Type of jobs there are as priority:
            * 
            * 
            * 
            * 
            * 
            */

            if (job.JobOccupation == visitor) {

                if (true) { // Do that
                    jobsList.Add(job);
                    continue;
                }

                

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

        return minDistJob;
    }

    protected override void OnJobEnded(Job j) {
        if (j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }


        if (j.JobOccupation == Job.JobType.Visitor) {

            // Do any visitor specific job end changed applied

        }
        
        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.Tile.x, j.Tile.y);
        MyJob = null;

    }
}
