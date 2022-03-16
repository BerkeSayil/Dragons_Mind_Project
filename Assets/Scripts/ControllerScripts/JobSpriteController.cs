using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{
    // mostly built on top of furniture sprite controller
    // TODO: change later
    Dictionary<Job, GameObject> jobGameObjectMap;
    FurnitureSpriteController fcs;

    private void Start() {
        fcs = GameObject.FindObjectOfType<FurnitureSpriteController>();

        WorldController.Instance.world.
            jobQueue.RegisterJobCreationCallback(OnJobCreated);
        jobGameObjectMap = new Dictionary<Job, GameObject>();
    }
    
    void OnJobCreated(Job job) {
        // TODO: We only do furniture building jobs
        // expands on this.
        GameObject jobGO = new GameObject();

        jobGameObjectMap.Add(job, jobGO);


        jobGO.name = "Job_" + job.jobObjectType + "_" + job.tile.x + "_" + job.tile.y;
        jobGO.transform.position = new Vector2(job.tile.x, job.tile.y);
        jobGO.transform.SetParent(this.transform, true);
        
        jobGO.AddComponent<SpriteRenderer>().sprite = fcs.GetSpriteForFurniture(job.jobObjectType);
        SpriteRenderer sr = jobGO.GetComponent<SpriteRenderer>();

        // Floor sort order is 1 and furn order is 2 to ensure it comes on top.
        // this will make alpha lower so its more transparent
        sr.sortingLayerName = "Jobs";
        sr.color = new Color(0.5f,0.5f,1f,0.25f);

        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);

    }
    void OnJobEnded(Job j) {

        // TODO: We only do furniture building jobs
        // expands on this.

        // TODO: Delete Sprite ghostly

        GameObject jobGO = jobGameObjectMap[j];

        j.UnregisterJobCancelCallback(OnJobEnded);
        j.UnregisterJobCompleteCallback(OnJobEnded);

        Destroy(jobGO);

    }

}
