using UnityEngine;
using System;

public class Bone : MonoBehaviour
{
    public event Action<GameObject> OnCollected;

    private void OnEnable()
    {
        //Debug.Log("Bone_OnCollected subscribing to OnCollected");
        OnCollected += Bone_OnCollected;
    }

    private void Bone_OnCollected(GameObject obj)
    {
        Debug.Log($"Collected item's name is: {obj.gameObject.name}");
        //Debug.Log($"{obj.name} was collected, Bone.Bone_OnCollected was executed");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        Debug.Log($"Bone collected event invoked: {gameObject.name}");
        OnCollected?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        OnCollected -= Bone_OnCollected;
    }

    public Delegate[] GetOnCollectedInvocationList()
    {
        if (OnCollected != null)
            return OnCollected.GetInvocationList();
        else
            return null;
    }
}