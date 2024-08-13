using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Cinemachine;

public class UIManager : MonoBehaviour
{
    public List<AIButtonMapping> aiButtonMappings; 
    public TMP_Text infoPanel; //The UI Text element to display AI information

    public CinemachineVirtualCameraBase overlookCamera; //Reference to the overlook camera
    public Button switchCameraButton; // Ui button to switch between cameras
    public Toggle sightToggle; //Toggle button to show/hide sight visuals

    private List<AiAgent> aiAgents = new List<AiAgent>(); // List of all AI Agents in the scene
    private AiAgent selectedAI;
    private AiAgent previousSelectedAI;
    private FieldOfViewVisualizer selectedFOV;

    private bool isUsingOverlookCamera = false; //Track which camera is active

    [System.Obsolete]
    private void Start()
    {
        //Assign each button's listener to its corresponding AI agent
        foreach (var mapping in aiButtonMappings)
        {
            AiAgent agent = mapping.aiAgent; //Cache the agent reference for the button
            mapping.aiButton.onClick.AddListener(() => OnAIButtonClicked(agent));
        }

        ////Find all AI Agents in the scene and populate the dropdown
        //aiAgents.AddRange(FindObjectsOfType<AiAgent>());

        ////populate the dropdown with AI names
        //for (int i = 0; i < aiButtons.Count; i++)
        //{
        //    //Cache the index for the button's delegate
        //    int index = i; 
        //    aiButtons[i].onClick.AddListener(() => OnAIButtonClicked(index));
        //}
        //List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        //foreach (var ai in aiAgents)
        //    options.Add(new TMP_Dropdown.OptionData(ai.name));

        //Add listener to the switch camera button
        switchCameraButton.onClick.AddListener(SwitchCamera);

        //Add listener to the sight toggle button
        sightToggle.onValueChanged.AddListener(OnSightToggleChanged);

        //Set initial selection
        if (aiAgents.Count > 0)
            OnAIButtonClicked(aiButtonMappings[0].aiAgent);

        UpdateSwitchCameraButtonText();
    }

    private void Update()
    {
        //Update the information each frame
        DisplayAIInformation();
    }
    private void OnAIButtonClicked(AiAgent agent)
    {
        //Set the selected AI based on the dropdown selection
        selectedAI = agent;
        selectedFOV = selectedAI.GetComponent<FieldOfViewVisualizer>();

        if (selectedFOV != null)
        {
            sightToggle.isOn = selectedFOV.debugMode;
        }

        //Update the selection indicator
        if (previousSelectedAI != null && previousSelectedAI != selectedAI)
            previousSelectedAI.SetSelected(false, isUsingOverlookCamera); //Disable the indicator on the previous AI

        selectedAI.SetSelected(true, isUsingOverlookCamera); //Enable the indicator on the newly selected AI if overlook camera is active

        //Remember the current selected AI as the previous one for the next selection
        previousSelectedAI = selectedAI;

        //Only switch the camera if we are not using the overlook camera
        if(!isUsingOverlookCamera)
            ActivateAICamera(selectedAI);
    }
    private void SwitchCamera()
    {
        if (isUsingOverlookCamera)
        {
            //Switch to the selected AI's camera
            ActivateAICamera(selectedAI);

            //Update the toggle state
            isUsingOverlookCamera = false; //Make sure this flag is set to false when switching to the AI camera

            //Update button Text
            UpdateSwitchCameraButtonText();

            //Update the selection indicator state (disable)
            if (selectedAI != null)
                selectedAI.SetSelected(true, isUsingOverlookCamera);
            if (previousSelectedAI != null && previousSelectedAI != selectedAI)
                previousSelectedAI.SetSelected(false, isUsingOverlookCamera);
        }
        else
        {
            //Deactivate the selected AI's camera
            foreach (var agent in aiAgents)
            {
                var camera = agent.GetComponentInChildren<CinemachineVirtualCameraBase>();
                if (camera != null)
                    camera.Priority = 0;
            }

            //Switch to the overlook Camera
            overlookCamera.Priority = 10;

            //Update the toggle state
            isUsingOverlookCamera = true;

            //Update button Text
            UpdateSwitchCameraButtonText();

            //Update the selection indicator state (enable)
            if (selectedAI != null)
                selectedAI.SetSelected(true, isUsingOverlookCamera);
            if (previousSelectedAI != null && previousSelectedAI != selectedAI)
                previousSelectedAI.SetSelected(true, isUsingOverlookCamera);
        }
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
        foreach (var mapping in aiButtonMappings)
        {
            var camera = mapping.aiAgent.GetComponentInChildren<CinemachineVirtualCameraBase>();
            if (camera != null)
                camera.Priority = 0;
            else
                Debug.Log($"Camera on agents are null");
        }

        //Activate the selected AI's camera
        var selectedCamera = selectedAI.GetComponentInChildren<CinemachineVirtualCameraBase>();
        if (selectedCamera != null)
            selectedCamera.Priority = 10;

        overlookCamera.Priority = 0;
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
