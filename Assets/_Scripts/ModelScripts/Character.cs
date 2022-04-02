using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : MonoBehaviour
{
    public Tile currTile;
    Tile destTile; // if not moving this equals to currTile
    Vector3 destTilePos;
    Vector3 currTilePos;

    bool areWeCloseEnough = false;
    float reachDist = 1f;

    float timeDeltaTime;
    AIPath path;
    GraphNode node1;
    GraphNode node2;

    Job myJob;
    Action<Character> cbCharacterChanged;


    private void Awake() {
        currTile = destTile = WorldController.Instance.world.GetTileAt
            (WorldController.Instance.world.width / 2, WorldController.Instance.world.height / 2);

        currTilePos = new Vector3(currTile.x, currTile.y);

        path = gameObject.GetComponent<AIPath>();

        AstarPath.active.Scan();

        

        node1 = GetNodeOnTile(currTilePos);
        node2 = GetNodeOnTile(destTilePos);
    }
    public GraphNode GetNodeOnTile(Vector3 pos) {
   
        return AstarPath.active.GetNearest(pos, NNConstraint.Default).node;
      

    }
    public void Update() {

        timeDeltaTime = Time.deltaTime;
        // if don't have job, get a job
        if (myJob == null) {

            GrabJob();

            if(myJob != null) {
                 // Check if the job is reachable. If not abandon job.
                if (IsPathPossible(myJob) == false && myJob != null) {
                    AbandonJob(myJob.tile, myJob.jobObjectType);
                    return;
                }
                // we have a new job.
                if (myJob.tile != destTile) {
                    SetDestination(myJob.tile);
                }
                
                // if a cancel or complete callback occurs we call onjobended
                myJob.RegisterJobCancelCallback(OnJobEnded);
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }


        if ((transform.position - destTilePos).sqrMagnitude < reachDist * reachDist) {
            areWeCloseEnough = true;
        }
        
        if (areWeCloseEnough) {
            if (myJob != null) {
                myJob.DoWork(timeDeltaTime);
                areWeCloseEnough = false;
            }
            
            return;
        }
        
        
        
        if(cbCharacterChanged != null) {
            cbCharacterChanged(this);
        }

        

    }

    private void GrabJob() {
        if (WorldController.Instance.world.jobQueue.Dequeue() == null) return;

        myJob = PrioritizedJob(WorldController.Instance.world.jobQueue.Dequeue());

    }

    public virtual Job PrioritizedJob(ArrayList jobsList ) { //TODO: Well I mean do this.
        /*
         * Check for the following criteria to understand who to prioritize
         * 
         * what is closer (closeness score ?)
         * what is mostImportant (construction, something of chaotic nature, ...)
         * this also should filter with character job in mind so we don't get another occupants jobs.
         * 
         */
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
        if(minDistJob == null) return null;

        WorldController.Instance.world.jobQueue.RemoveMyJob(minDistJob);

        return minDistJob;
    }

    public bool IsPathPossible(Job myJob) {
        
        node1 = GetNodeOnTile(currTilePos);
        node2 = GetNodeOnTile(new Vector2(myJob.tile.x, myJob.tile.y));

        return PathUtilities.IsPathPossible(node1, node2);
    }

    private void AbandonJob(Tile t, string furnitureType) {

        destTile = currTile;
        currTilePos = gameObject.transform.position;

        //TODO: This doesn't need to be here maybe figure out why it was here in the first place?

        /*
        Job j = new Job(t, furnitureType, (theJob) =>
        {
            WorldController.Instance.world.
            PlaceFurnitureAt(furnitureType, theJob.tile);

            t.pendingFurnitureJob = null;
        });
        */

        myJob = null;

        node1 = GetNodeOnTile(currTilePos);
        node2 = GetNodeOnTile(destTilePos);

    }

    public void SetDestination(Tile tile) {
        if(tile != null) {


            destTile = tile;
            destTilePos = new Vector3(tile.x, tile.y);

            path.destination = new Vector3(tile.x, tile.y);


            node1 = GetNodeOnTile(currTilePos);
            node2 = GetNodeOnTile(destTilePos);
        }

    }
    

    public void RegisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged += cb;
    }
    public void UnregisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged -= cb;
    }

    void OnJobEnded(Job j) {
        if(j != myJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }
        AstarPath.active.Scan();
        currTile = destTile;
        currTilePos = destTilePos = new Vector3(j.tile.x, j.tile.y);
        myJob = null;


    }

}
