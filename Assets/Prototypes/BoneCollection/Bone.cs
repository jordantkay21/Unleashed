using UnityEngine;
using System;

public class Bone : MonoBehaviour
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
        OnCollected?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}