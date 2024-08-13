using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AIButtonMapping
{
    public Button aiButton; //Reference to the button in the UI
    public AiAgent aiAgent; //Reference to the specific AI agent
}
