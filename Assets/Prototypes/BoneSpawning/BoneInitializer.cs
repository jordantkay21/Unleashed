using UnityEngine;
using System.Collections.Generic;
using System;


/// <summary>
/// Manages the initialization and activation of bone objects within a specific game area.
/// </summary>
public class BoneInitializer : KSMonoBehaviour
{
    /// <summary>
    /// Event that is triggered once bones are initialized
    /// </summary>
    public event Action<List<Transform>> OnBonesInitialized;

    /// <summary>
    /// List of bone transforms managed by this initializer
    /// </summary>
    public List<Transform> boneList = new List<Transform>();

    /// <summary>
    /// Initializes bone objects by deactivating them and adding them to a list for management
    /// </summary>
    public void InitializeBones()
    {
        //Iterate through all child objects to find bones
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bone"))
            {
                boneList.Add(child);
                child.gameObject.SetActive(false); //Initially disable the bone objects
                if (verbose) Debug.Log($"{child.gameObject.name} added to boneList and deactivated");
            }
        }

        //Invoke the event to notify subscribers that bones are initialized
        OnBonesInitialized?.Invoke(boneList);
        if (verbose) Debug.Log("BoneInitializer.OnBonesInitialized invoked");
    }

    /// <summary>
    /// Retrieves the invocation list of the OnBonesInitialized event
    /// </summary>
    /// <returns>An array of Delegate objects subscribed to the OnBonesInitialized event.</returns>
    public Delegate[] GetOnBonesInitializedInvocationList()
    {
        if (OnBonesInitialized != null)
            return OnBonesInitialized.GetInvocationList();
        else
            return null;
    }

}
