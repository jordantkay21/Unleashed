using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : KSMonoBehaviour
{
    [Tooltip("Points between which the enemy patrols")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    private NavMeshAgent agent;

    public enum State {Patrol, Chase, Attack}
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

        currentPatrolIndex = 0;
        EventManager.instance.ChangeAIState(State.Patrol);
    }

    private void SetState(State newState)
    {
        currentState = newState;
        if (verbose) Debug.Log($"Entered into {currentState} state.");
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        switch (currentState)
        {
            case State.Patrol:
                MoveToNextPatrolPoint();
                break;
            case State.Chase:
                //Implement Chase Logic Here
                break;
            case State.Attack:
                //Implement Attack Logic Here
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        //might want to handle continous actions like chasing or attacking here
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
