using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : MonoBehaviour
{
    public Tile CurrTile { get; protected set; }
    protected Tile DestTile; // if not moving this equals to currTile
    protected Vector3 DestTilePos;
    protected Vector3 CurrTilePos;

    private bool _areWeCloseEnough = false;
    private float _reachDist = 1.2f;
    private float _timeDeltaTime;

    private AIPath _path;
    private GraphNode _node1;
    private GraphNode _node2;

    protected Job MyJob;
    
    private Action<Character> _cbCharacterChanged;


    private void Awake() {
        CurrTile = DestTile = WorldController.Instance.World.GetTileAt((int) transform.position.x, (int) transform.position.y);

        CurrTilePos = new Vector3(CurrTile.x, CurrTile.y);

        _path = gameObject.GetComponent<AIPath>();

        AstarPath.active.Scan();

        

        _node1 = GetNodeOnTile(CurrTilePos);
        _node2 = GetNodeOnTile(DestTilePos);
    }
    private GraphNode GetNodeOnTile(Vector3 pos) {
   
        return AstarPath.active.GetNearest(pos, NNConstraint.Default).node;
      

    }
    public void Update() {

        _timeDeltaTime = Time.deltaTime;
        // if don't have job, get a job
        if (MyJob == null) {

            GrabJob();

            if(MyJob != null) {
                 // Check if the job is reachable. If not abandon job.
                if (IsPathPossible(MyJob) == false && MyJob != null) {
                    AbandonJob(MyJob.Tile, MyJob.JobObjectType);
                    return;
                }
                // we have a new job.
                if (MyJob.Tile != DestTile) {
                    SetDestination(MyJob.Tile);
                }
                
                // if a cancel or complete callback occurs we call onjobended
                MyJob.RegisterJobCancelCallback(OnJobEnded);
                MyJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        
        if ((transform.position - DestTilePos).sqrMagnitude < _reachDist * _reachDist) {
            _areWeCloseEnough = true;
        }
        

        if (_areWeCloseEnough) {
            if (MyJob == null) return;
            
            MyJob.DoWork(_timeDeltaTime);
            _areWeCloseEnough = false;

            return;
        }

        _cbCharacterChanged?.Invoke(this);

    }

    private void GrabJob() {
        if (WorldController.Instance.World.JobQueue.Dequeue() == null) return;

        MyJob = PrioritizedJob(WorldController.Instance.World.JobQueue.Dequeue());

    }

    protected virtual Job PrioritizedJob(ArrayList jobsList ) { //TODO: Well I mean do this.
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

    protected bool IsPathPossible(Job myJob) {
        
        _node1 = GetNodeOnTile(CurrTilePos);
        _node2 = GetNodeOnTile(new Vector2(myJob.Tile.x, myJob.Tile.y));

        return PathUtilities.IsPathPossible(_node1, _node2);
    }
    public bool IsPathPossible(Vector2 one, Vector2 two) {

        _node1 = GetNodeOnTile(one);
        _node2 = GetNodeOnTile(two);

        return PathUtilities.IsPathPossible(_node1, _node2);
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

        _node1 = GetNodeOnTile(CurrTilePos);
        _node2 = GetNodeOnTile(DestTilePos);

    }

    private void SetDestination(Tile tile)
    {
        if (tile == null) return;
        
        DestTile = tile;
        DestTilePos = new Vector3(tile.x, tile.y);

        _path.destination = new Vector3(tile.x, tile.y);


        _node1 = GetNodeOnTile(CurrTilePos);
        _node2 = GetNodeOnTile(DestTilePos);

    }
    

    public void RegisterOnChangedCallback(Action<Character> cb) {
        _cbCharacterChanged += cb;
    }
    public void UnregisterOnChangedCallback(Action<Character> cb) {
        _cbCharacterChanged -= cb;
    }

    protected virtual void OnJobEnded(Job j) {
        if(j != MyJob) {
            Debug.LogError("Character is thinking about a job that isn't theirs. You nforgot to unregister  something.");
            return;
        }

        AstarPath.active.Scan();
        CurrTile = DestTile;
        CurrTilePos = DestTilePos = new Vector3(j.Tile.x, j.Tile.y);
        MyJob = null;

    }

}
