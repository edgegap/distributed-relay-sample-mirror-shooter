using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform[] waypoints; 
    int numWaypoint = 0;

    private void Start()
    {
        navAgent.SetDestination(waypoints[numWaypoint].position);
    }
    public void NextWaypoint()
    {
        if (numWaypoint <= waypoints.Length && gameObject != null)
        {
            navAgent.SetDestination(waypoints[numWaypoint].position); 
        }
    }

    private void OnTriggerEnter(Collider InfoCol)
    {
        if (InfoCol.gameObject.tag == "waypoint")
        { 
            numWaypoint += 1;
            if (numWaypoint == waypoints.Length)
            {
                numWaypoint = 0;
            }
            NextWaypoint();
        }
    }
}
