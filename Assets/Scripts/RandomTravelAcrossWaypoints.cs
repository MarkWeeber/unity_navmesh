using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomTravelAcrossWaypoints : MonoBehaviour
{
    [SerializeField] private WaypointsBuilder waypointsBuilder;
    private int waypointsRange;
    private List<Vector3> _waypoints;
    private NavMeshAgent navMeshAgent;
    private void Start()
    {
        Invoke(nameof(SetCourse), 2f);
    }

    private void SetCourse()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _waypoints = WaypointsBuilder.Waypoints;
        if(_waypoints == null)
        {
            Debug.LogError("NOT FOUND");
        }
        else
        {
            waypointsRange = _waypoints.Count;
            Vector3 destination = new Vector3();
            destination = _waypoints[UnityEngine.Random.Range(0, waypointsRange)];
            //Debug.Log(destination);
            navMeshAgent.SetDestination(destination);    
        }
    }
}
