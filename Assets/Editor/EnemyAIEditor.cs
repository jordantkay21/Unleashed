using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AiAgent))]
public class EnemyAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AiAgent enemyAI = (AiAgent)target;


        //Ensure the ZoneDatabase is assigned
        if (enemyAI.zoneDatabase != null)
        {
            // Fetch teh available zones from the ScriptableObject
            List<string> zones = enemyAI.zoneDatabase.availableZones;
            int index = zones.IndexOf(enemyAI.selectedZone);

            if (index == -1) index = 0; // Default to the first zone if the selected one is not found

            //Display the dropdown  at the top of the inspector
            index = EditorGUILayout.Popup("Assigned Zone", index, zones.ToArray());
            enemyAI.selectedZone = zones[index];
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a ZoneDatabase.", MessageType.Warning);
        }

        //Draw the default inspector
        DrawDefaultInspector();

        //Apply any property changes made in the inspector
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
