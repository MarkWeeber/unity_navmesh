using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [SerializeField] PersonList personList;
    [SerializeField] private float checkPersonTime = 4f;
    [SerializeField] private float startWaitTime = 1f;
    [SerializeField] private float stopTreshold = 0.2f;
    private float checkPersonTimer = 0f;
    private float startWaitTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private Transform target = null;
    private bool started = false;
    private Vector3 initialPosition;
    private int numberOfPersons = 0;
    private System.Random random = new System.Random();
    private NavMeshPath navMeshPath;
    private List<Person> _personList;
    private Vector3 _targetPosition;
    private Person friskedPerson = null;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
        _targetPosition = new Vector3();
        startWaitTimer = startWaitTime;
        initialPosition = this.transform.position;
        numberOfPersons = personList.personList.Count;
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
        // if at post and no target assigned try looking for new one
        if(target == null)
        {
            // if at post
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                if(checkPersonTimer > 0) // make a little wait
                {
                    checkPersonTimer -= Time.deltaTime;
                }
                else // if wait is over pick try pick next target, or wait another time
                {
                    CheckNextRandomPerson();
                    checkPersonTimer = checkPersonTime;
                }
            }
        }
        // else work on target
        else
        {
            // keep traking target
            navMeshAgent.SetDestination(target.position);
            // if person slipped away go to original post
            if(!isTargetReachable(target.position))
            {
                target = null;
                friskedPerson = null;
                GoToInitialPost();
                return;
            }
            float distanceToTarget = Vector3.Distance(this.transform.position, target.position);
            // if reached any target - frisk target and try next target within given time
            if(distanceToTarget <= navMeshAgent.stoppingDistance + stopTreshold)
            {
                if (checkPersonTimer > 0)
                {
                    checkPersonTimer -= Time.deltaTime;
                    // frisking logic
                    if(friskedPerson != null)
                    {
                        friskedPerson.BeingFrisked();
                    }
                }
                else // next pickup
                {
                    if(friskedPerson != null)
                    {
                        friskedPerson.ReleaseFromFrisking();
                    }
                    CheckNextRandomPerson();
                    checkPersonTimer = checkPersonTime;
                }
            }
        }
    }

    private void CheckNextRandomPerson()
    {
        // shuffle the cloned list so it's always a random pick
        _personList = new List<Person>(personList.personList);
        ShuffleList(ref _personList);
        // pick target if no target found then go to original post
        target = null;
        int currentIndex = _personList.Count - 1;
        while (currentIndex >= 0)
        {
            _targetPosition = _personList[currentIndex].transform.position;
            // check if target is reachable
            if(isTargetReachable(_targetPosition))
            {
                target = _personList[currentIndex].transform;
                friskedPerson = target.GetComponent<Person>();
                break;
            }
            else
            {
                currentIndex--;
            }
        }
        // if no valid target was found then go to original post
        if(target == null)
        {
            GoToInitialPost();
        }
    }

    private void GoToInitialPost()
    {
        navMeshAgent.SetDestination(initialPosition);
    }

    private bool isTargetReachable(Vector3 targetPosition)
    {
        bool ans = false;
        if (navMeshAgent.CalculatePath(targetPosition, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            ans = true;
        }
        return ans;
    }

    private void ShuffleList<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1) {  
            n--;  
            int k = random.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
    }
}
