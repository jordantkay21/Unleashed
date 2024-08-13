using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiAgent : KSMonoBehaviour
{
    [SerializeField]
    public ZoneDatabase zoneDatabase; //Reference to the ScriptableObject containing zones

    [HideInInspector]
    public string selectedZone; //The Zone selected from the dropdown

    [SerializeField]
    [Tooltip("Points between which the enemy patrols")]
    private List<Transform> patrolPoints;
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;

    [SerializeField] GameObject selectionIndicator;
    [SerializeField] private Transform spottedPlayer;
    [SerializeField] private LayerMask targetMask; //Layer on which the targets (e.g. player) resides
    [SerializeField] private LayerMask obstacleMask; //Layer on which the obstacles (e.g. walls) resides
    [SerializeField] public float checkRate = 0.2f;
    [SerializeField] public bool playerInSight;

    public float sightRange = 15f;
    public Transform eyePosition; //The point from which sight starts, typically the head of the AI
    public float fieldOfViewAngle = 120f; //120 degrees of vision

    private Coroutine currentRoutine;

    //Materials to highlight and revert objects
    public Material highlightMaterial;
    public Material defaultMaterial;

    private List<Renderer> highlightedRenderers = new List<Renderer>();

    public enum State { Patrol, Chase, Attack }
    public State currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent == null)
        {
            Debug.LogError("NavMeshAgent Component is missing from this GameObject!");
            return;
        }
        SetPatrolZone(selectedZone);
        SetState(State.Patrol);
        StartCoroutine(SightCheck());
    }

    private void SetPatrolZone(string zoneName)
    {
        patrolPoints = ZoneManager.instance.GetPatrolPointsForZone(zoneName);

        if (patrolPoints == null || patrolPoints.Count == 0)
            if (verbose) Debug.LogError($"No patrol points found for zone: {zoneName}");
        else
            if (verbose) Debug.Log($"Assigned {patrolPoints.Count} patrol points from zone: {zoneName}");

        currentPatrolIndex = 0; // Reset patrol index when changing zone
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
            case State.Chase:
                currentRoutine = StartCoroutine(Chase());
                break;
                /*
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
            if (patrolPoints == null || patrolPoints.Count == 0)
            {
                if (verbose) Debug.LogError("Patrol points list is empty or null.");
                yield break;
            }
             
            if (agent == null)
            {
                if (verbose) Debug.LogError("NavMeshAgent is not initialized.");
                yield break;
            }

            //Set the agent to go to the currently selected destination
            agent.destination = patrolPoints[currentPatrolIndex].position;

            //Wait until the agent has reached its destination before moving to the next one
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                yield return null; //wait for next frame

            //Choose the next point in the array as the destination,
            //cycling to the start if necessary
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            yield return null;

            //Add a small delay if needed before moving to the next point
            yield return new WaitForSeconds(0.5f); 
        }
    }

    
    IEnumerator Chase()
    {
        while (true)
        {
            if (agent == null || playerInSight == false || !playerInSight)
                yield break;

            //Set the agent to go to the player's position
            agent.destination = spottedPlayer.position;

            //Wait until the agent reaches the player's last known position
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                yield return null; // wait for the next frame
            
            yield return null;
        }

    }

    /*
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

            CheckSightAndhighlightObjects();

            if (CanSeePlayer())
            {
                if (!playerInSight)
                {
                    playerInSight = true;
                    EventManager.instance.PlayerSpotted(); //Trigger an event when the player is spotted
                                                           //Switch to the Chase state when the player is spotted
                    SetState(State.Chase);
                }
            }
            else
            {
                if (playerInSight)
                {
                    playerInSight = false;
                    spottedPlayer = null;
                    EventManager.instance.PlayerOutOfSight(); // Trigger an event when the player is out of sight
                                                              //Switch to the Patrol state when the player is out of sight
                    SetState(State.Patrol);
                }
            }

        }
    }

    private void CheckSightAndhighlightObjects()
    {
        //Clear previous highlights
        ClearHighlightedObjects();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, sightRange, targetMask);

        foreach (var target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - eyePosition.position).normalized;

            if(Vector3.Angle(eyePosition.forward, dirToTarget) < fieldOfViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(eyePosition.position, targetTransform.position);

                if (!Physics.Raycast(eyePosition.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    //The Object is in sight, apply the highlight
                    Renderer renderer = targetTransform.GetComponent<Renderer>();
                    if(renderer != null)
                    {
                        renderer.material = highlightMaterial;
                        highlightedRenderers.Add(renderer);
                    }

                    //Check if the object is the player
                    if (target.CompareTag("Player"))
                        spottedPlayer = targetTransform; //Dynamically assign the player as the target
                }
            }
        }
    }

    private void ClearHighlightedObjects()
    {
        foreach (Renderer renderer in highlightedRenderers)
        {
            if (renderer != null)
                renderer.material = defaultMaterial;
        }
        highlightedRenderers.Clear();
    }

    private bool CanSeePlayer()
    {
        if (spottedPlayer == null)
            return false;

        Vector3 dirToTarget = (spottedPlayer.position - eyePosition.position).normalized;

        if (Vector3.Angle(eyePosition.forward, dirToTarget) < fieldOfViewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(eyePosition.position, spottedPlayer.position);

            if (!Physics.Raycast(eyePosition.position, dirToTarget, dstToTarget, obstacleMask))
                return true; //Player is visible
        }

        return false;
    }

    public void SetSelected(bool isSelected, bool isOverlookCameraActive)
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(isSelected && isOverlookCameraActive);
    }
}
