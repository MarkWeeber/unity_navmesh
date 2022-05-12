using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointsBuilder : MonoBehaviour
{
    public static List<Vector3> Waypoints
    {
        get { return waypoints; }
    }
    private static List<Vector3> waypoints;
    [SerializeField] private Vector3 stepping = Vector3.one;
    private void Awake()
    {
        waypoints = new List<Vector3>();
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
        //Debug.Log("START: " + startX + " : " + startY + " : " + startZ);
        //Debug.Log("END: " + endX + " : " + endY + " : " + endZ);
        // loop search for valid path for waypoints
        NavMeshPath navMeshPath = new NavMeshPath();
        for (float x = startX; x <= endX; x += stepping.x)
        {
            for (float z = startZ; z <= endZ; z += stepping.z)
            {
                for (float y = startY; y <= endY; y += stepping.y)
                {
                    Vector3 target = new Vector3(x, y, z);
                    //Debug.Log(x + "_" + y + "_" + z);
                    // check if the location is valid
                    if (navMeshAgent.CalculatePath(target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                    {
                        //Debug.Log(target);
                        waypoints.Add(target);
                        //Debug.Log("ADDED");
                    }
                }
            }
        }
    }

}
