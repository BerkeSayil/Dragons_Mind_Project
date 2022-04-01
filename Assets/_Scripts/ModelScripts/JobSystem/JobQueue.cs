using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class JobQueue
{
    // This is a wrapper class for bending stuff to our own will :P
   

    /*
     *  just store all jobs unsorted in an ArrayList or similar unsorted data-structure
     * When an actor needs a new job, it iterates that whole datastructure and uses a rating function to assign an own priority rating
     * to each job and pick the job with the highest score for itself.
     * 
     */

    protected Queue<Job> jobQueue;
    protected ArrayList jobQueueList; // Be sure to get only Job class in here.

    Action<Job> cbJobCreated;

    public JobQueue() {
        //jobQueue = new Queue<Job>();
        jobQueueList = new ArrayList();
    }

    public void Enqueue(Job j) {
        //jobQueue.Enqueue(j);

        jobQueueList.Add(j);

        if(cbJobCreated != null) {
            cbJobCreated(j);
        }

    }
    public ArrayList Dequeue() {
        if(jobQueueList.Count == 0) {
            return null;
        }

        return jobQueueList; // We'll just return the whole list so characters decide themselves what to do

    }
    
    public void RemoveMyJob(Job j) {
        jobQueueList.Remove(j);
    }

    public void RegisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated += cb;
    }
    public void UnregisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated -= cb;
    }

}
