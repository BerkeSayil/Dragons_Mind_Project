using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CrewmateScript : MonoBehaviour
{
    AIDestinationSetter destinationSetter;
    AIPath pathfindingAI;

    float crewmateReach = 0.5f;


    private void Start()
    {
        destinationSetter = gameObject.GetComponent<AIDestinationSetter>();
        pathfindingAI = gameObject.GetComponent<AIPath>();

    }

    public void SetDestination(Transform dest)
    {
        destinationSetter.target = dest;
    }
    public bool isReachedDestination()
    {
        
        if (pathfindingAI.remainingDistance < crewmateReach)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
