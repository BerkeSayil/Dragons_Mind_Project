using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : MonoBehaviour
{
    public Tile currTile;
    Tile destTile; // if not moving this equals to currTile

    float movementPercentage; // Goes to 1 from 0 as we progress fron currTile => destTile

    float timeDeltaTime;
    AIPath path;

    Job myJob;
    Action<Character> cbCharacterChanged;

    public float X {
        get {
            return Mathf.Lerp(currTile.x, destTile.x, movementPercentage);
        }
    }
    public float Y {
        get {
            return Mathf.Lerp(currTile.y, destTile.y, movementPercentage);
        }
    }
    private void Awake() {
        currTile = destTile = WorldController.Instance.world.GetTileAt(WorldController.Instance.world.width / 2, WorldController.Instance.world.height / 2);
        path = gameObject.GetComponent<AIPath>();
    }
    
    public void Update() {

        timeDeltaTime = Time.deltaTime;

        // if don't have job, get a job
        if (myJob == null) {
            // grab a job.
            myJob = currTile.World.jobQueue.Dequeue();
            if(myJob != null) {
                // we have a new job.
                if(myJob.tile != destTile) {
                    SetDestination(myJob.tile);
                }
                
                // if a cancel or complete callback occurs we call onjobended
                myJob.RegisterJobCancelCallback(OnJobEnded);
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        
        if (path.reachedDestination) {
            if (myJob != null) {
                myJob.DoWork(timeDeltaTime);
            }
            currTile = destTile;
            return;
        }
        
        
        
        if(cbCharacterChanged != null) {
            cbCharacterChanged(this);
        }

        

    }
    public void SetDestination(Tile tile) {
        if(tile != null) {
            destTile = tile;
            path.destination = new Vector3(tile.x, tile.y);
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
        myJob = null;
    }

    /*
    // TODO: Find a better way to get this script.
    [SerializeField] CharacterSpriteController characterSpriteController;

    private void Start() {

        characterSpriteController.RegisterCharacterReadyForAI(ReadyAI);

    }

    private void ReadyAI(GameObject character) {
        // Do config of AI

        AIPath aiPath = character.AddComponent<AIPath>();
        aiPath.gravity = Vector3.zero;



    }
    */
}
