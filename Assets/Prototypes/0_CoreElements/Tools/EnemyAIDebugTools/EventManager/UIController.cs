using UnityEngine;

public class UIController : MonoBehaviour
{
    public void SetPatrolState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Patrol);
    }

    public void SetChaseState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Chase);
    }

    public void SetAttackState()
    {
        EventManager.instance.ChangeAIState(EnemyAI.State.Attack);
    }
}
