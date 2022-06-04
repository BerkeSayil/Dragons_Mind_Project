using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : MonoBehaviour
{
    public float speed = 10f;
    public Tile CurrTile { get; set; }
    protected Tile DestTile; // if not moving this equals to currTile
    protected Vector3 DestTilePos;
    protected Vector3 CurrTilePos;

    private float _timeDeltaTime;

    private AIPath _path;

    protected Job MyJob;
    
    private Action<Character> _cbCharacterChanged;
    
    
    private const Job.JobType Construction = Job.JobType.Construction;
    private const Job.JobType Deconstruction = Job.JobType.Deconstruction;


    
    //movement stuck checker
    private Transform objectTransfom;
 
    private float noMovementThreshold = 0.0000001f;
    private const int noMovementFrames = 600;
    Vector2[] previousLocations = new Vector2[noMovementFrames];
    private bool isMoving;


    private void Awake() {
        
        DestTilePos = CurrTilePos = new Vector3(transform.position.x, transform.position.y);

        _path = gameObject.GetComponent<AIPath>();

        AstarPath.active.Scan();
        
        //For good measure, set the previous locations
        for(int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector2.zero;
        }
        

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
        
        // if we have a job, move towards it
        if (MyJob != null) {
            if (DestTilePos != CurrTilePos) {
                SetDestination(MyJob.Tile);
            }

            Work(_timeDeltaTime);
    
        }
        
        //Store the newest vector at the end of the list of vectors
        for(int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i+1];
        }
        previousLocations[previousLocations.Length - 1] = transform.position;
 
        //Check the distances between the points in your previous locations
        //If for the past several updates, there are no movements smaller than the threshold,
        //you can most likely assume that the object is not moving
        for(int i = 0; i < previousLocations.Length - 1; i++)
        {
            if(Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                //The minimum movement has been detected between frames
                isMoving = true;
                break;
            }
            else
            {
                isMoving = false;
            }
        }
        _cbCharacterChanged?.Invoke(this);

    }

    private void Work(float timeDeltaTime)
    {
        if(_path.reachedDestination)
        {
            MyJob.DoWork(timeDeltaTime);
        }
        
        else if (!isMoving)
        {
            MyJob.CancelWork();
            _path.Teleport(new Vector3
                (WorldController.Instance.World.Width + 10, WorldController.Instance.World.Height/2));
            CurrTilePos = new Vector3
                (WorldController.Instance.World.Width + 10, WorldController.Instance.World.Height / 2);

        }
        
        
        
    }

    private void GrabJob() {
        if (WorldController.Instance.World.JobQueue.Dequeue() == null) return;

        MyJob = PrioritizedJob(WorldController.Instance.World.JobQueue.Dequeue());

    }

    protected virtual Job PrioritizedJob(ArrayList jobsListTotal ) { //TODO: Well I mean do this.
        
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

    protected bool IsPathPossible(Job myJob) {
        
        // Find the closest node to this GameObject's position
        GraphNode ourNode = AstarPath.active.GetNearest(transform.position).node;
        // Find the closest node to the job's tile's position
        GraphNode jobNode = AstarPath.active.GetNearest(new Vector3(myJob.Tile.x, myJob.Tile.y)).node;
        
        if (PathUtilities.IsPathPossible(ourNode, jobNode)) {
            return  true;
        }

        return false;

    }
    
    private void AbandonJob(Tile t, string furnitureType) {

        DestTile = CurrTile;
        CurrTilePos = transform.position;

        
        MyJob = null;


    }


    private void SetDestination(Tile tile)
    {
        
        DestTile = tile;
        DestTilePos = new Vector3(tile.x, tile.y);
        _path.destination = DestTilePos;

    }
    
    protected virtual void OnJobEnded(Job j) {
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
    
    public bool IsMoving
    {
        //Let other scripts see if the object is moving

        get{ return isMoving; }
    }
    
    

    
}
