using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public event Action<EnemyAI.State> OnChangeAIState;
    public event Action OnPlayerSpotted;
    public event Action OnPlayerLost;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }
    public void ChangeAIState(EnemyAI.State newState)
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
