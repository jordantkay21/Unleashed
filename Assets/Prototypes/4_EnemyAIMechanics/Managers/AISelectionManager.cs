using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Cinemachine;

public class AISelectionManager : MonoBehaviour
{
    public TMP_Dropdown aiDropdown; //The dropdown menu for selecting AI Agents
    public TMP_Text infoPanel; //The UI Text element to display AI information

    private List<EnemyAI> aiAgents = new List<EnemyAI>(); // List of all AI Agents in the scene
    private EnemyAI selectedAI;

    [System.Obsolete]
    private void Start()
    {
        //Find all AI Agents in the scene and populate the dropdown
        aiAgents.AddRange(FindObjectsOfType<EnemyAI>());

        //populate the dropdown with AI names
        aiDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var ai in aiAgents)
            options.Add(new TMP_Dropdown.OptionData(ai.name));

        aiDropdown.AddOptions(options);

        //Add listener to handle dropdown selection
        aiDropdown.onValueChanged.AddListener(OnAISelected);

        //Set initial selection
        if (aiAgents.Count > 0)
            OnAISelected(0);
    }

    private void OnAISelected(int index)
    {
        //Set the selected AI based on the dropdown selection
        selectedAI = aiAgents[index];

        //Activate the selected AI's camera
        ActivateAICamera(selectedAI);

        //Display AI Information
        DisplayAIInformation();
    }


    private void DisplayAIInformation()
    {
        if (selectedAI != null)
        {
            //Display relevent information about the selected AI
            infoPanel.text = $"AI Name: {selectedAI.name}\n" +
                         $"Current State: {selectedAI.currentState}\n" +
                         $"Position: {selectedAI.transform.position}\n" +
                         $"Sight Range: {selectedAI.sightRange}\n" +
                         $"Field of View: {selectedAI.fieldOfViewAngle}\n" +
                         $"Player In Sight: {selectedAI.playerInSight}";
        }
    }
    private void ActivateAICamera(EnemyAI selectedAI)
    {
        //Deactivate all AI cameras first
        foreach (var agent in aiAgents)
        {
            var camera = agent.GetComponentInChildren<CinemachineVirtualCameraBase>();
            if (camera != null)
                camera.Priority = 0;
            else
                Debug.Log($"Camera on agents are null");
        }

        //Activate the selected AI's camera
        var selectedCamera = selectedAI.GetComponentInChildren<CinemachineVirtualCameraBase>();
        if (selectedCamera != null)
            selectedCamera.Priority = 10;
    }
}
