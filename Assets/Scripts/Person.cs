using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    [SerializeField] private WaypointsBuilder waypointsBuilder;
    [SerializeField] private float waitTime = 4f;
    [SerializeField] private float waitTimeAtDanceFloor = 16f;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private NavMeshModifierVolume danceFloorVolume;
    private Animator animator;
    private float waitTimer = 0f;
    private float startWaitTimer = 0f;
    private float setWaitTime = 0f;
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;
    private Vector3 previousDestination;
    private bool started = false;
    private NavMeshHit navMeshHit;
    private bool stopTrigger = true;
    // animator flags
    private bool onDanceFloor = false;
    private int danceState = 0;
    private bool beinFrisked = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startWaitTimer = startWaitTime;
        animator = GetComponent<Animator>();
        previousDestination = this.transform.position - Vector3.one;
        setWaitTime = waitTime;
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
        // if destination is reached
        if (
                navMeshAgent.remainingDistance == 0 &&
                navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete &&
                Vector3.Distance(this.transform.position, previousDestination) > 0.5f
            )
        {
            if (!stopTrigger) // make this check only once per destination
            {
                CheckIfOnDanceFloor();
                waitTimer = setWaitTime;
            }
            if (waitTimer == 0) // the very first destination pick up at start
            {
                SetNextRandomDestination();
                return;
            }
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            else // next pickup
            {
                SetNextRandomDestination();
            }
        }
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool("AtDanceFloor", onDanceFloor);
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        animator.SetInteger("DanceState", danceState);
        animator.SetBool("BeingFrisked",beinFrisked);
    }

    private void CheckIfOnDanceFloor()
    {
        //Debug.Log("CHECKING DANCE FLOOR");
        stopTrigger = true;
        setWaitTime = waitTime;
        if (NavMesh.SamplePosition(this.transform.position, out navMeshHit, 1f, NavMesh.AllAreas))
        {
            if (navMeshHit.mask == 1 << danceFloorVolume.area)
            {
                danceState = UnityEngine.Random.Range(1, 7 + 1);
                onDanceFloor = true;
                setWaitTime = waitTimeAtDanceFloor;
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
        //Debug.Log("FALSE STOP TRIGGER");
        stopTrigger = false;
        onDanceFloor = false;
        if (waypointsBuilder.Waypoints != null)
        {
            previousDestination = this.transform.position;
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

    public void BeingFrisked()
    {
        beinFrisked = true;
        navMeshAgent.isStopped = true;
    }

    public void ReleaseFromFrisking()
    {
        beinFrisked = false;
        navMeshAgent.isStopped = false;
    }
}
