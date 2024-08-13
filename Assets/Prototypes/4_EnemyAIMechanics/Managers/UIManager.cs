using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Cinemachine;

public class UIManager : MonoBehaviour
{
    public TMP_Dropdown aiDropdown; //The dropdown menu for selecting AI Agents
    public TMP_Text infoPanel; //The UI Text element to display AI information

    public CinemachineVirtualCameraBase overlookCamera; //Reference to the overlook camera
    public Button switchCameraButton; // Ui button to switch between cameras
    public Toggle sightToggle; //Toggle button to show/hide sight visuals

    private List<AiAgent> aiAgents = new List<AiAgent>(); // List of all AI Agents in the scene
    private AiAgent selectedAI;
    private FieldOfViewVisualizer selectedFOV;

    private bool isUsingOverlookCamera = false; //Track which camera is active

    [System.Obsolete]
    private void Start()
    {
        //Find all AI Agents in the scene and populate the dropdown
        aiAgents.AddRange(FindObjectsOfType<AiAgent>());

        //populate the dropdown with AI names
        aiDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var ai in aiAgents)
            options.Add(new TMP_Dropdown.OptionData(ai.name));

        aiDropdown.AddOptions(options);

        //Add listener to handle dropdown selection
        aiDropdown.onValueChanged.AddListener(OnAISelected);

        //Add listener to the switch camera button
        switchCameraButton.onClick.AddListener(SwitchCamera);

        //Add listener to the sight toggle button
        sightToggle.onValueChanged.AddListener(OnSightToggleChanged);

        //Set initial selection
        if (aiAgents.Count > 0)
            OnAISelected(0);

        UpdateSwitchCameraButtonText();
    }


    private void Update()
    {
        //Update the information each frame
        DisplayAIInformation();
    }
    private void OnAISelected(int index)
    {
        //Set the selected AI based on the dropdown selection
        selectedAI = aiAgents[index];
        selectedFOV = selectedAI.GetComponent<FieldOfViewVisualizer>();

        //Activate the selected AI's camera
        ActivateAICamera(selectedAI);

        if (selectedFOV != null)
        {
            sightToggle.isOn = selectedFOV.debugMode;
        }
    }
    private void SwitchCamera()
    {
        if (isUsingOverlookCamera)
        {
            //Switch to the selected AI's camera
            ActivateAICamera(selectedAI);

            //Update the toggle state
            isUsingOverlookCamera = false; //Make sure this flag is set to false when switching to the AI camera

        }
        else
        {
            //Switch to the overlook Camera
            overlookCamera.Priority = 10;

            //Deactivate the selected AI's camera
            foreach (var agent in aiAgents)
            {
                var camera = agent.GetComponentInChildren<CinemachineVirtualCameraBase>();
                if (camera != null)
                    camera.Priority = 0;
            }

            //Update the toggle state
            isUsingOverlookCamera = true;
        }

        //Update button Text
        UpdateSwitchCameraButtonText();
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
    private void ActivateAICamera(AiAgent selectedAI)
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
    private void OnSightToggleChanged(bool isOn)
    {
        if (selectedFOV != null)
            selectedFOV.debugMode = isOn;

        
    }
    private void UpdateSwitchCameraButtonText()
    {
        if (isUsingOverlookCamera)
            switchCameraButton.GetComponentInChildren<TMP_Text>().text = "AI Camera";
        else
            switchCameraButton.GetComponentInChildren<TMP_Text>().text = "Overlook Camera";

    }
}
