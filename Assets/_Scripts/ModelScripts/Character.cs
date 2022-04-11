using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : MonoBehaviour
{
    public Tile CurrTile { get; protected set; }
    public Tile DestTile { get; protected set; } // if not moving this equals to currTile
    public Vector3 DestTilePos { get; protected set; }
    public Vector3 CurrTilePos { get; protected set; }

    private bool _AreWeCloseEnough = false;
    private float _ReachDist = 1.2f;
    private float _TimeDeltaTime;

    private AIPath _Path;
    private GraphNode _Node1;
    private GraphNode _Node2;

    public Job MyJob { get; protected set; }

    Action<Character> cbCharacterChanged;


    private void Awake() {
        CurrTile = DestTile = WorldController.Instance.world.GetTileAt
            (WorldController.Instance.world.width / 2, WorldController.Instance.world.height / 2);

        CurrTilePos = new Vector3(CurrTile.x, CurrTile.y);

        _Path = gameObject.GetComponent<AIPath>();

        AstarPath.active.Scan();

        

        _Node1 = GetNodeOnTile(CurrTilePos);
        _Node2 = GetNodeOnTile(DestTilePos);
    }
    public GraphNode GetNodeOnTile(Vector3 pos) {
   
        return AstarPath.active.GetNearest(pos, NNConstraint.Default).node;
      

    }
    public void Update() {

        _TimeDeltaTime = Time.deltaTime;
        // if don't have job, get a job
        if (MyJob == null) {

            GrabJob();

            if(MyJob != null) {
                 // Check if the job is reachable. If not abandon job.
                if (IsPathPossible(MyJob) == false && MyJob != null) {
                    AbandonJob(MyJob.tile, MyJob.jobObjectType);
                    return;
                }
                // we have a new job.
                if (MyJob.tile != DestTile) {
                    SetDestination(MyJob.tile);
                }
                
                // if a cancel or complete callback occurs we call onjobended
                MyJob.RegisterJobCancelCallback(OnJobEnded);
                MyJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        
        if ((transform.position - DestTilePos).sqrMagnitude < _ReachDist * _ReachDist) {
            _AreWeCloseEnough = true;
        }
        

        if (_AreWeCloseEnough) {
            if (MyJob != null) {
                MyJob.DoWork(_TimeDeltaTime);
                _AreWeCloseEnough = false;
            }
            
            return;
        }

        cbCharacterChanged?.Invoke(this);

    }

    private void GrabJob() {
        if (WorldController.Instance.world.jobQueue.Dequeue() == null) return;

        MyJob = PrioritizedJob(WorldController.Instance.world.jobQueue.Dequeue());

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
        /*
         *  Because this should be specialized per occupation
         *  this is not implemented but in case we want to give people jobs doable by everyone it's a core piece of character class
         * 
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

        */

        return null;


    }

    public bool IsPathPossible(Job myJob) {
        
        _Node1 = GetNodeOnTile(CurrTilePos);
        _Node2 = GetNodeOnTile(new Vector2(myJob.tile.x, myJob.tile.y));

        return PathUtilities.IsPathPossible(_Node1, _Node2);
    }
    public bool IsPathPossible(Vector2 one, Vector2 two) {

        _Node1 = GetNodeOnTile(one);
        _Node2 = GetNodeOnTile(two);

        return PathUtilities.IsPathPossible(_Node1, _Node2);
    }
    private void AbandonJob(Tile t, string furnitureType) {

        DestTile = CurrTile;
        CurrTilePos = gameObject.transform.position;

        //TODO: This doesn't need to be here maybe figure out why it was here in the first place?

        /*
        Job j = new Job(t, furnitureType, (theJob) =>
        {
            WorldController.Instance.world.
            PlaceFurnitureAt(furnitureType, theJob.tile);

            t.pendingFurnitureJob = null;
        });
        */

        MyJob = null;

        _Node1 = GetNodeOnTile(CurrTilePos);
        _Node2 = GetNodeOnTile(DestTilePos);

    }

    public void SetDestination(Tile tile) {
        if(tile != null) {


            DestTile = tile;
            DestTilePos = new Vector3(tile.x, tile.y);

            _Path.destination = new Vector3(tile.x, tile.y);


            _Node1 = GetNodeOnTile(CurrTilePos);
            _Node2 = GetNodeOnTile(DestTilePos);
        }

    }
    

    public void RegisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged += cb;
    }
    public void UnregisterOnChangedCallback(Action<Character> cb) {
        cbCharacterChanged -= cb;
    }

    public virtual void OnJobEnded(Job j) {
        if(j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.tile.x, j.tile.y);
        MyJob = null;

    }

}
