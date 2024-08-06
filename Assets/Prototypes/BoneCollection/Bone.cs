using UnityEngine;
using System;

/// <summary>
/// Represents a collectable bone within the game. It manages its own collection logic and events.
/// </summary>
public class Bone : KSMonoBehaviour
{
    /// <summary>
    /// Event triggered when the bone is collected
    /// </summary>
    public event Action<GameObject> OnCollected;

    /// <summary>
    /// Unity's method called when another collider enters the trigger collider attached to this bone
    /// </summary>
    /// <param name="other">The other collider that triggered this event</param>
    private void OnTriggerEnter(Collider other)
    {
        //Check if the collider is tagged as "Player" to determine if the bone should be collected 
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    /// <summary>
    /// Collects this bone. Disables the game object and triggers any subscribed events
    /// </summary>
    public void Collect()
    {
        //Debug.Log($"Bone collected event invoked: {gameObject.name}");
        OnCollected?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieves the list of delegates currently subscribed to the OnCollected event
    /// </summary>
    /// <returns>An array of Delgates representing the subscribers to the OnCollected event</returns>
    public Delegate[] GetOnCollectedInvocationList()
    {
        if (OnCollected != null)
            return OnCollected.GetInvocationList();
        else
            return null;
    }
}