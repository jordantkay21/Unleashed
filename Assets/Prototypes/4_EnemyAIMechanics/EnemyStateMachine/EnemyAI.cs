using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : KSMonoBehaviour
{
    [SerializeField]
    [Tooltip("Points between which the enemy patrols")]
    private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;

    [SerializeField] private Transform playerTarget; //The target player object
    [SerializeField] private LayerMask targetMask; //Layer on which the targets (e.g. player) resides
    [SerializeField] private LayerMask obstacleMask; //Layer on which the obstacles (e.g. walls) resides
    [SerializeField] private float checkRate = 0.2f;
    [SerializeField] private bool playerInSight;

    public float sightRange = 15f;
    public Transform eyePosition; //The point from which sight starts, typically the head of the AI
    public float fieldOfViewAngle = 120f; //120 degrees of vision

    private Coroutine currentRoutine;

    public enum State { Patrol, Chase, Attack }
    public State currentState;


    private void OnEnable()
    {
        EventManager.instance.OnChangeAIState += SetState;
    }

    private void OnDisable()
    {
        EventManager.instance.OnChangeAIState -= SetState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EventManager.instance.ChangeAIState(State.Patrol);
        StartCoroutine(SightCheck());
    }

    private void SetState(State newState)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentState = newState;
        switch (currentState)
        {
            case State.Patrol:
                currentRoutine = StartCoroutine(Patrol());
                break;
                /*
            case State.Chase:
                currentRoutine = StartCoroutine(Chase());
                break;
            case State.Attack:
                currentRoutine = StartCoroutine(Attack());
                break;
                */
        }
    }
    IEnumerator Patrol()
    {
        while (true)
        {
            //There are no points to patrol to
            if (patrolPoints.Length == 0)
                yield return null;

            //Set the agent to go to the currently selected destination
            agent.destination = patrolPoints[currentPatrolIndex].position;

            //Wait until the agent has reached its destination before moving to the next one
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                yield return null; //wait for next frame

            //Choose the next point in the array as the destination,
            //cycling to the start if necessary
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            yield return null;

            //Add a small delay if needed before moving to the next point
            yield return new WaitForSeconds(0.5f); 
        }
    }

    /*
    IEnumerator Chase()
    {
        while (true)
        {
            agent.destination = playerTarget.position;
            yield return null; // Continue updating the destination as long as the state doesn't change
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            // Implement attack logic here
            yield return new WaitForSeconds(1); //Example delay between attacks
        }
    }
    */

    IEnumerator SightCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkRate); //Wait for the specified check rate

            if (currentState == State.Patrol && CanSeePlayer())
            {
                if(!playerInSight) //Player was not previously in sight
                {
                    playerInSight = true;
                    EventManager.instance.PlayerSpotted();
                }
                else
                {
                    if(playerInSight) // Player was previously in sight
                    {
                        playerInSight = false;
                        EventManager.instance.PlayerOutOfSight();
                    }
                }
                Debug.Log("Player in sight!");
                //Here you can trigger the UI alert
                UIManager.instance.DisplayAlert("Player is in sight!");
            }
                    //EventManager.instance.ChangeAIState(State.Chase);
        }
    }

    private bool CanSeePlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sightRange, targetMask);

        foreach (var target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - eyePosition.position).normalized;

            if (Vector3.Angle(eyePosition.forward, dirToTarget) < fieldOfViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(eyePosition.position, targetTransform.position);

                if (!Physics.Raycast(eyePosition.position, dirToTarget, dstToTarget, obstacleMask))
                    return true; //Player is visible
            }
        }
        
        return false;
    }
}
