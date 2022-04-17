using System;

public class Job 
{
    // This class hold info for a queued up job
    // Which can include placing furniture, moving stored inventory (from trading dock to construction), 
    // working at a bar or kitchen, constructing tiles or walls...

    public Tile Tile { get; protected set; }
    
    private float _jobTime;
    public JobType JobOccupation { get; protected set; }
    public bool HaulingJob { get; protected set; }

    public string JobObjectType { get; protected set; }
    public Inventory Inventory { get; protected set; }

    public Tile.TileType JobTileType { get; protected set; }

    private Action<Job> _cbJobComplete;
    private Action<Job> _cbJobCancel;

    //Constructor for furniture placement / removal
    public Job(Tile tile, string jobObjectType , Action<Job>cbJobComplete, JobType occupationType ,float jobTime = 1f) {
        Tile = tile;
        JobObjectType = jobObjectType;
        _jobTime = jobTime;
        JobOccupation = occupationType;
       

        _cbJobComplete += cbJobComplete;

    }

    //Constructor for tile placement / removal
    public Job(Tile tile, Tile.TileType tileType, Action<Job> cbJobComplete, JobType occupationType, float jobTime = 1f) {
        Tile = tile;
        JobTileType = tileType;
        _jobTime = jobTime;
        JobOccupation = occupationType;

        _cbJobComplete += cbJobComplete;

    }

    //Constructor for hauling inventory
    public Job(Tile tile, bool haulingJob, Inventory inv ,Action<Job> cbJobComplete, JobType occupationType,float jobTime = 0.2f) {
        Tile = tile;
        HaulingJob = haulingJob;
        Inventory = inv;
        _jobTime = jobTime;
        JobOccupation = occupationType;


        _cbJobComplete += cbJobComplete;

    }


    public void RegisterJobCompleteCallback(Action<Job> cb) {
        _cbJobComplete += cb;
    }
    public void RegisterJobCancelCallback(Action<Job> cb) {
        _cbJobCancel += cb;

    }
    public void UnregisterJobCompleteCallback(Action<Job> cb) {
        _cbJobComplete -= cb;
    }
    public void UnregisterJobCancelCallback(Action<Job> cb) {
        _cbJobCancel -= cb;

    }

    public void DoWork(float workTime) {
        _jobTime -= workTime;

        if (!(_jobTime <= 0)) return;

        _cbJobComplete?.Invoke(this);
        
    }
    public void CancelWork()
    {
        _cbJobCancel?.Invoke(this);

    }

    public enum JobType { // These jobs are based on what type of occupant will do it
        Construction, // build, dismantle furniture and haul goods 
        InventoryManagement, // this is for picking up inventory and hauling it off
        Deconstruction,
        Engineering,
        Trader,
        Visitor,
        Pirate,
        NotImplemented
            //TODO: Add more job types as game evolves.
    }
}
