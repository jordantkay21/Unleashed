using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class FieldOfViewVisualizer : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void OnDrawGizmos()
    {
        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();

        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        if (enemyAI == null) return;

        Gizmos.color = Color.yellow;

        //Draw the range of sight as a wire sphere
        Gizmos.DrawWireSphere(enemyAI.transform.position, enemyAI.sightRange);

        //Calculate the edges of the field of view
        Vector3 forward = enemyAI.eyePosition.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -enemyAI.fieldOfViewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, enemyAI.fieldOfViewAngle / 2, 0) * forward;

        //Number of segments to use for the FOV visualization
        int segments = 20;


        //Draw a filled arc for the FOV
        for(int i=0; i < segments; i++)
        {
            float angle1 = -enemyAI.fieldOfViewAngle / 2 + (enemyAI.fieldOfViewAngle / segments) * i;
            float angle2 = -enemyAI.fieldOfViewAngle / 2 + (enemyAI.fieldOfViewAngle / segments) * (i + 1);

            Vector3 point1 = Quaternion.Euler(0, angle1, 0) * forward * enemyAI.sightRange;
            Vector3 point2 = Quaternion.Euler(0, angle2, 0) * forward * enemyAI.sightRange;

            Gizmos.DrawLine(enemyAI.eyePosition.position, enemyAI.eyePosition.position + point1);
            Gizmos.DrawLine(enemyAI.eyePosition.position + point1, enemyAI.eyePosition.position + point2);
            Gizmos.DrawLine(enemyAI.eyePosition.position + point2, enemyAI.eyePosition.position);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(enemyAI.eyePosition.position, enemyAI.eyePosition.position + rightBoundary * enemyAI.sightRange);
        Gizmos.DrawLine(enemyAI.eyePosition.position, enemyAI.eyePosition.position + leftBoundary * enemyAI.sightRange);
    }
}
