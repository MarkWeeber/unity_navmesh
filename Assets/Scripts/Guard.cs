using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [SerializeField] PersonList personList;
    [SerializeField] private float checkPersonTime = 4f;
    [SerializeField] private float startWaitTime = 1f;
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
        // keep tracking person
        if(target != null)
        {
            navMeshAgent.SetDestination(target.position);
            // if person slipped away go to original post
            if(!isTargetReachable(target.position))
            {
                target = null;
                GoToInitialPost();
            }
        }
        // if reached any target try next target within given time
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (checkPersonTimer == 0) // the very first target pick up at start
            {
                CheckNextRandomPerson();
                checkPersonTimer = checkPersonTime;
                return;
            }
            if (checkPersonTimer > 0)
            {
                checkPersonTimer -= Time.deltaTime;    
            }
            else // next pickup
            {
                CheckNextRandomPerson();
                checkPersonTimer = checkPersonTime;
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
