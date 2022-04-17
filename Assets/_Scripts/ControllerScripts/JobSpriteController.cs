using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{
    // mostly built on top of furniture sprite controller
    // TODO: change later
    private Dictionary<Job, GameObject> _jobGameObjectMap;
    private FurnitureSpriteController _fcs;
    private TileSpriteController _tcs;

    private void Start() {
        _fcs = FindObjectOfType<FurnitureSpriteController>();
        _tcs = FindObjectOfType<TileSpriteController>();


        WorldController.Instance.World.
            JobQueue.RegisterJobCreationCallback(OnJobCreated);
        _jobGameObjectMap = new Dictionary<Job, GameObject>();
    }
    
    private void OnJobCreated(Job job) {
        // TODO: We only do furniture building jobs expands on this.

        GameObject jobGo = new GameObject();

        if (_jobGameObjectMap.ContainsKey(job)) {
            Debug.LogError("Got an OnJobCreated for an GO that already exist. Probably RE-QUEUEing instead of creating?");
            return;
        }

        _jobGameObjectMap.Add(job, jobGo);


        jobGo.name = "Job_" + job.JobObjectType + "_" + job.Tile.x + "_" + job.Tile.y;
        jobGo.transform.position = new Vector2(job.Tile.x, job.Tile.y);
        jobGo.transform.SetParent(this.transform, true);


        // this is a inventory pickup or haul off job we don't need the sprite side for it
        if (job.JobOccupation == Job.JobType.InventoryManagement) {


            job.RegisterJobCompleteCallback(OnJobEnded);
            job.RegisterJobCancelCallback(OnJobEnded);
            return;
        }

        

        // if what we want to place is furniture
        if (job.JobObjectType != null) {     
            jobGo.AddComponent<SpriteRenderer>().sprite = _fcs.GetSpriteForFurniture(job.JobObjectType);
            SpriteRenderer sr = jobGo.GetComponent<SpriteRenderer>();
            // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
            // this will make alpha lower so its more transparent
            sr.sortingLayerName = "Jobs";
            sr.color = new Color(0.5f, 0.5f, 1f, 0.25f);

        }
        else if (job.JobTileType != Tile.TileType.Empty){

            jobGo.AddComponent<SpriteRenderer>().sprite = _tcs.GetSpriteForTile(job.JobTileType);
            SpriteRenderer sr = jobGo.GetComponent<SpriteRenderer>();
            // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
            // this will make alpha lower so its more transparent
            sr.sortingLayerName = "Jobs";
            sr.color = new Color(0.5f, 0.5f, 1f, 0.25f);
        }

        

        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);

    }

    
    private void OnJobEnded(Job j) {

        // TODO: We only do furniture building jobs
        // expands on this.

        // TODO: Delete Sprite ghostly

        GameObject jobGo = _jobGameObjectMap[j];

        j.UnregisterJobCancelCallback(OnJobEnded);
        j.UnregisterJobCompleteCallback(OnJobEnded);

        Destroy(jobGo);

    }

}
