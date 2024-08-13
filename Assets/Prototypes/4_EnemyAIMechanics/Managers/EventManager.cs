using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public event Action<AiAgent.State> OnChangeAIState;
    public event Action OnPlayerSpotted;
    public event Action OnPlayerLost;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

    }
    public void ChangeAIState(AiAgent.State newState)
    {
        OnChangeAIState?.Invoke(newState);
    }

    public void PlayerSpotted()
    {
        OnPlayerSpotted?.Invoke();
    }

    public void PlayerOutOfSight()
    {
        OnPlayerLost?.Invoke();
    }
}
