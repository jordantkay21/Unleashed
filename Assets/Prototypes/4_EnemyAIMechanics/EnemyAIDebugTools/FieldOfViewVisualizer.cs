using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAI), typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfViewVisualizer : MonoBehaviour
{
    private EnemyAI enemyAI;
    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    [SerializeField]
    private bool debugMode = true;  // Toggle to enable/disable visualization

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }
    void LateUpdate()
    {
        if (debugMode)
        {
            DrawFieldOfView();
        }
        else
        {
            viewMesh.Clear();
        }
    }

    private void DrawFieldOfView()
    {
        if (enemyAI == null) return;

        // Calculate the edges of the field of view
        Vector3 forward = enemyAI.eyePosition.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -enemyAI.fieldOfViewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, enemyAI.fieldOfViewAngle / 2, 0) * forward;

        // Number of segments to use for the FOV visualization
        int segments = 20;
        float angleStep = enemyAI.fieldOfViewAngle / segments;

        // Create arrays for vertices and triangles
        Vector3[] vertices = new Vector3[segments + 2]; // +2 for the center and the last vertex
        int[] triangles = new int[segments * 3];

        // Set the first vertex at the center
        vertices[0] = enemyAI.eyePosition.position - enemyAI.transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -enemyAI.fieldOfViewAngle / 2 + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;
            vertices[i + 1] = enemyAI.transform.InverseTransformPoint(enemyAI.eyePosition.position + direction * enemyAI.sightRange);

            if (i < segments)
            {
                // Define the triangles
                int triangleIndex = i * 3;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = i + 2;
            }
        }

        // Assign the vertices and triangles to the mesh
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
}
