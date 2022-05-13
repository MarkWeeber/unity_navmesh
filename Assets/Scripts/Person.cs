using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    [SerializeField] private WaypointsBuilder waypointsBuilder;
    [SerializeField] private float waitTime = 4f;
    [SerializeField] private float startWaitTime = 1f;
    private float waitTimer = 0f;
    private float startWaitTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;
    private bool started = false;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startWaitTimer = startWaitTime;
    }

    private void Update()
    {
        if (!started)
        {
            if (startWaitTimer <= 0)
            {
                started = true;
            }
            if (startWaitTimer > 0)
            {
                startWaitTimer -= Time.deltaTime;
            }
            return;
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) // if destination is reached
        {
            if (waitTimer == 0) // the very first destination pick up at start
            {
                SetNextRandomDestination();
                waitTimer = waitTime;
                return;
            }
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;    
            }
            else // next pickup
            {
                SetNextRandomDestination();
                waitTimer = waitTime;
            }
        }
    }

    private void SetNextRandomDestination()
    {
        if (waypointsBuilder.Waypoints != null)
        {
            destination = waypointsBuilder.Waypoints[
                UnityEngine.Random.Range(0, waypointsBuilder.Waypoints.Count)
                ];
            navMeshAgent.SetDestination(destination);
        }
        else
        {
            Debug.LogError("Waypoints not found");
        }
    }
}
