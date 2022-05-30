
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAI : Character {

    private const Job.JobType Construction = Job.JobType.Construction;
    private const Job.JobType Deconstruction = Job.JobType.Deconstruction;
    
    protected override Job PrioritizedJob(ArrayList jobsListTotal) { 
        
        if (jobsListTotal.Count == 0) return null;

        ArrayList jobsList = new ArrayList();

        foreach (Job job in jobsListTotal) {
            switch (job.JobOccupation)
            {
                case Construction:
                {
                    jobsList.Add(job); 
                    break;
                }
                case Deconstruction:
                    jobsList.Add(job);
                    break;
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
        return minDistJob;
    }

    protected override void OnJobEnded(Job j) {
        if (j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You forgot to unregister  something.");
            return;
        }

        if (j.JobOccupation == Job.JobType.Deconstruction)
        {
            Furniture furniture = WorldController.Instance.World.FurniturePrototypes[j.JobObjectType];
            if (furniture.Width > 1 || furniture.Height > 1) {
                for (int x = j.Tile.x; x < j.Tile.x + furniture.Width; x++) {
                    for (int y = j.Tile.y; y < j.Tile.y + furniture.Height; y++) {
                        Tile tile = WorldController.Instance.World.GetTileAt(x, y);
                        if (tile != null && tile.Furniture != null) {
                            tile.SetFurnitureChild(null);
                        }
                    }
                }
            }
        }
        
        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.Tile.x, j.Tile.y);
        MyJob = null;

    }
}
