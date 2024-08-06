using UnityEngine;
using System;

public class Bone : KSMonoBehaviour
{
    public event Action<GameObject> OnCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        //Debug.Log($"Bone collected event invoked: {gameObject.name}");
        OnCollected?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    public Delegate[] GetOnCollectedInvocationList()
    {
        if (OnCollected != null)
            return OnCollected.GetInvocationList();
        else
            return null;
    }
}