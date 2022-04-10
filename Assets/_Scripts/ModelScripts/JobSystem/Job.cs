using System;

public class Job 
{
    // This class hold info for a queued up job
    // Which can include placing furniture, moving stored inventory (from trading dock to construction), 
    // working at a bar or kitchen, constructing tiles or walls...

    public Tile tile { get; protected set; }
    float jobTime;
    public JobType jobOccupation { get; protected set; }
    public bool haulingJob { get; protected set; }

    public string jobObjectType { get; protected set; }
    public Inventory inventory { get; protected set; }

    public Tile.TileType jobTileType { get; protected set; }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    //Constructor for furniture placement / removal
    public Job(Tile tile, string jobObjectType , Action<Job>cbJobComplete, JobType occupationType ,float jobTime = 1f) {
        this.tile = tile;
        this.jobObjectType = jobObjectType;
        this.jobTime = jobTime;
        this.jobOccupation = occupationType;
       

        this.cbJobComplete += cbJobComplete;

    }

    //Constructor for tile placement / removal
    public Job(Tile tile, Tile.TileType tileType, Action<Job> cbJobComplete, JobType occupationType, float jobTime = 1f) {
        this.tile = tile;
        this.jobTileType = tileType;
        this.jobTime = jobTime;
        this.jobOccupation = occupationType;

        this.cbJobComplete += cbJobComplete;

    }

    //Constructor for hauling inventory
    public Job(Tile tile, bool haulingJob, Inventory inv ,Action<Job> cbJobComplete, JobType occupationType,float jobTime = 0.2f) {
        this.tile = tile;
        this.haulingJob = haulingJob;
        this.inventory = inv;
        this.jobTime = jobTime;
        this.jobOccupation = occupationType;


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

    public enum JobType { // These jobs are based on what type of occupant will do it
        Construction, // build, dismantle furnitures and haul goods 
        InventoryManagement, // this is for picking up inventory and hauling it off
        Deconstruction,
        Engineering,
        Trader,
        Visitor,
        Pirate,
        TODO
            //TODO: Add more job types as game evolves.
    }
}
