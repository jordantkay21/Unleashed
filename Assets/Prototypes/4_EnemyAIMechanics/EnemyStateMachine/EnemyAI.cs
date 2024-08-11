using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Tooltip("Points between which the enemy patrols")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    private NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentPatrolIndex = 0;
        MoveToNextPatrolPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            MoveToNextPatrolPoint();
    }
    private void MoveToNextPatrolPoint()
    {
        //There are no points to patrol to
        if (patrolPoints.Length == 0) return;

        //Set the agent to go to the currently selected destination
        agent.destination = patrolPoints[currentPatrolIndex].position;

        //Choose the next point in the array as the destination,
        //cycling to the start if necessary
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

}
