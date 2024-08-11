using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text alertText;
    public TMP_Text stateText;
    public TMP_Text timerText;

    private float timeOutOfSight;
    private bool isPlayerOutOfSight;

    private void Awake()
    {
        if (instance == null) 
            instance = this;
    }

    private void Update()
    {
        if (isPlayerOutOfSight)
        {
            timeOutOfSight += Time.deltaTime;
            timerText.text = $"Time out of sight: {timeOutOfSight:F2} seconds"; //insures the number should be formatted as a floating-point number ('F') with exactly 2 digits after the decimal point ('2')
        }
    }
    private void OnEnable()
    {
        EventManager.instance.OnChangeAIState += UpdateAIStateText;
        EventManager.instance.OnPlayerSpotted += HandlePlayerSpotted;
        EventManager.instance.OnPlayerLost += HandlePlayerLost;
    }

    private void HandlePlayerSpotted()
    {
        isPlayerOutOfSight = false;
        DisplayAlert("Player Spotted!");
        timeOutOfSight = 0;
    }

    private void HandlePlayerLost()
    {
        isPlayerOutOfSight = true;
        DisplayAlert("Player is out of Sight!");
    }

    public void DisplayAlert(string message)
    {
        alertText.text = message;
    }

    private void UpdateAIStateText(EnemyAI.State state)
    {
        stateText.text = $"AI State: {state}";
    }

    public void SetPatrolState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Patrol);
    }
    /*
    public void SetChaseState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Chase);
    }

    public void SetAttackState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Attack);
    }
    */
    private void OnDisable()
    {
        EventManager.instance.OnChangeAIState -= UpdateAIStateText;
        EventManager.instance.OnPlayerSpotted -= HandlePlayerSpotted;
        EventManager.instance.OnPlayerLost -= HandlePlayerLost;
    }
}
