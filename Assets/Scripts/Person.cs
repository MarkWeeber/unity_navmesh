using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    [SerializeField] private WaypointsBuilder waypointsBuilder;
    [SerializeField] private float waitTime = 4f;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private NavMeshModifierVolume danceFloorVolume;
    private Animator animator;
    private float waitTimer = 0f;
    private float startWaitTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;
    private bool started = false;
    private NavMeshHit navMeshHit;
    // animator flags
    private bool onDanceFloor = false;
    private int danceState = 0;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startWaitTimer = startWaitTime;
        animator = GetComponent<Animator>();
        CheckIfOnDanceFloor();
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
                CheckIfOnDanceFloor();
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
                CheckIfOnDanceFloor();
            }
            else // next pickup
            {
                SetNextRandomDestination();
                waitTimer = waitTime;
            }
        }
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool("AtDanceFloor", onDanceFloor);
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        animator.SetInteger("DanceState", danceState);
    }

    private void CheckIfOnDanceFloor()
    {
        if (NavMesh.SamplePosition(this.transform.position, out navMeshHit, 1f, NavMesh.AllAreas))
        {
            if (navMeshHit.mask == 1 << danceFloorVolume.area)
            {
                onDanceFloor = true;
                danceState = UnityEngine.Random.Range(1, 2 + 1);
            }
            else
            {
                onDanceFloor = false;
            }
        }
        else
        {
            onDanceFloor = false;
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
