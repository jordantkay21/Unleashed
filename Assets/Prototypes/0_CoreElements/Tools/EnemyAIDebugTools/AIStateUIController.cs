using UnityEngine;
using TMPro;
using System;

public class AIStateUIController : MonoBehaviour
{
    public TMP_Text stateText;

    private void OnEnable()
    {
        EventManager.instance.OnChangeAIState += UpdateAIStateText;
    }

    private void OnDisable()
    {
        EventManager.instance.OnChangeAIState -= UpdateAIStateText;
    }

    private void UpdateAIStateText(EnemyAI.State state)
    {
        stateText.text = $"AI State: {state}";
    }
}
