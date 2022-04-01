using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CrewmateBTWander : Node
{
    Transform transform;
    Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float waitTime = 1; // seconds
    private float waitCounter = 0;
    private bool isWaiting = false;

    [SerializeField]private float speed = 1f;

    public CrewmateBTWander(Transform transform, Transform[] waypoints)
    {
        this.transform = transform;
        this.waypoints = waypoints;
    }


    public override NodeState Evaluate()
    {
        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime) 
                isWaiting = false;
        }
        else
        {
            Transform waypoint = waypoints[currentWaypointIndex];
            if(Vector2.Distance(transform.position, waypoint.position) < 0.01f)
            {
                transform.position = waypoint.position;
                waitCounter = 0;
                isWaiting = true;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);
                transform.LookAt(waypoint.position);
            }

        }

        state = NodeState.RUNNING;
        return state;
    }
}
