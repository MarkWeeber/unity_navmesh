using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointsBuilder : MonoBehaviour
{
    public List<Vector3> Waypoints;
    [SerializeField] private Vector3 stepping = Vector3.one;
    private void Awake()
    {
        Waypoints = new List<Vector3>();
        // grab necessary components
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        // setting dimensions for loop search
        float startX = transform.position.x + boxCollider.center.x - boxCollider.size.x / 2f;
        float endX = transform.position.x + boxCollider.center.x + boxCollider.size.x / 2f;
        float startY = transform.position.y + boxCollider.center.y - boxCollider.size.y / 2f;
        float endY = transform.position.y + boxCollider.center.y + boxCollider.size.y / 2f;
        float startZ = transform.position.z + boxCollider.center.z - boxCollider.size.z / 2f;
        float endZ = transform.position.z + boxCollider.center.z + boxCollider.size.z / 2f;
        // loop search for valid path for waypoints
        NavMeshPath navMeshPath = new NavMeshPath();
        for (float x = startX; x <= endX; x += stepping.x)
        {
            for (float z = startZ; z <= endZ; z += stepping.z)
            {
                for (float y = startY; y <= endY; y += stepping.y)
                {
                    Vector3 target = new Vector3(x, y, z);
                    // check if the location is valid, if valid then add to waypoints list
                    if (navMeshAgent.CalculatePath(target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                    {
                        Waypoints.Add(target);
                    }
                }
            }
        }
    }

}
