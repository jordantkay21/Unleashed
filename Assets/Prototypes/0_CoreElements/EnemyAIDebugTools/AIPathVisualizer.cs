using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class AIPathVisualizer : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.pathStatus != NavMeshPathStatus.PathInvalid)
            UpdatePathVisualization();
    }

    private void UpdatePathVisualization()
    {
        lineRenderer.positionCount = agent.path.corners.Length;
        lineRenderer.SetPositions(agent.path.corners);
        lineRenderer.enabled = true;
    }
}
