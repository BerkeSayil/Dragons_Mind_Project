using BehaviorTree;

public class CrewmateBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    protected override Node SetupTree()
    {
        Node root = new CrewmateBTWander(transform, waypoints);

        return root;
    }
}
