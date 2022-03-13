using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job 
{
    // This class hold info for a queued up job
    // Which can include placing furniture, moving stored inventory (from trading dock to construction), 
    // working at a bar or kitchen, constructing tiles or walls...

    public Tile tile { get; protected set; }
    float jobTime;

    public string jobObjectType { get; protected set; }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;
    public Job(Tile tile, string jobObjectType , Action<Job>cbJobComplete ,float jobTime = 1f) {
        this.tile = tile;
        this.jobObjectType = jobObjectType;

        this.cbJobComplete += cbJobComplete;

    }

    public void RegisterJobCompleteCallback(Action<Job> cb) {
        cbJobComplete += cb;
    }
    public void RegisterJobCancelCallback(Action<Job> cb) {
        cbJobCancel += cb;

    }
    public void UnregisterJobCompleteCallback(Action<Job> cb) {
        cbJobComplete -= cb;
    }
    public void UnregisterJobCancelCallback(Action<Job> cb) {
        cbJobCancel -= cb;

    }

    public void DoWork(float workTime) {
        jobTime -= workTime;
        
        if(jobTime <= 0) {
            if(cbJobComplete != null) {
                cbJobComplete(this);
            }
        }
    }
    public void CancelWork() {
        if (cbJobCancel != null) {
            cbJobCancel(this);
        }
        
    }
}
