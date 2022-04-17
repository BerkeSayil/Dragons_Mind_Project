using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class JobQueue
{
    // This is a wrapper class for bending stuff to our own will :P
   

    /*
     *  just store all jobs unsorted in an ArrayList or similar unsorted data-structure
     * When an actor needs a new job, it iterates that whole data structure and uses a rating function to assign an own priority rating
     * to each job and pick the job with the highest score for itself.
     * 
     */

    protected Queue<Job> jobQueue;
    private ArrayList _jobQueueList; // Be sure to get only Job class in here.

    private Action<Job> _cbJobCreated;

    public JobQueue() {
        //jobQueue = new Queue<Job>();
        _jobQueueList = new ArrayList();
    }

    public void Enqueue(Job j) {
        //jobQueue.Enqueue(j);

        _jobQueueList.Add(j);

        _cbJobCreated?.Invoke(j);

    }
    public ArrayList Dequeue()
    {
        return _jobQueueList.Count == 0 ? null : _jobQueueList;
    }
    
    public void RemoveMyJob(Job j) {
        _jobQueueList.Remove(j);
    }

    public void RegisterJobCreationCallback(Action<Job> cb) {
        _cbJobCreated += cb;
    }
    public void UnregisterJobCreationCallback(Action<Job> cb) {
        _cbJobCreated -= cb;
    }

}
