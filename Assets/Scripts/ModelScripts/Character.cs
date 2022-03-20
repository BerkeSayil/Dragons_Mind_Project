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
        currTile = destTile = WorldController.Instance.world.GetTileAt(WorldController.Instance.world.width / 2, WorldController.Instance.world.height / 2);
        currTilePos = gameObject.transform.position;
        path = gameObject.GetComponent<AIPath>();

        AstarPath.active.Scan();

        node1 = AstarPath.active.GetNearest(currTilePos, NNConstraint.Default).node;
        node2 = AstarPath.active.GetNearest(destTilePos, NNConstraint.Default).node;
    }
    
    public void Update() {

        timeDeltaTime = Time.deltaTime;
        // if don't have job, get a job
        if (myJob == null) {
            // grab a job.
            myJob = currTile.World.jobQueue.Dequeue();
            if(myJob != null) {
                //TODO: Check if the job is reachable.
                if (isPathPossible() == false && myJob != null) {
                    AbandonJob();
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

    private bool isPathPossible() {
        node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
        node2 = AstarPath.active.GetNearest(new Vector2(myJob.tile.x, myJob.tile.y), NNConstraint.Default).node;

        return PathUtilities.IsPathPossible(node1, node2);
    }

    private void AbandonJob() {

        destTile = currTile;
        currTilePos = gameObject.transform.position;

        currTile.World.jobQueue.Enqueue(myJob);
        myJob = null;

        node1 = AstarPath.active.GetNearest(currTilePos, NNConstraint.Default).node;
        node2 = AstarPath.active.GetNearest(destTilePos, NNConstraint.Default).node;

    }

    public void SetDestination(Tile tile) {
        if(tile != null) {


            destTile = tile;
            destTilePos = new Vector3(tile.x, tile.y);

            path.destination = new Vector3(tile.x, tile.y);

            node1 = AstarPath.active.GetNearest(currTilePos, NNConstraint.Default).node;
            node2 = AstarPath.active.GetNearest(destTilePos, NNConstraint.Default).node;
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
