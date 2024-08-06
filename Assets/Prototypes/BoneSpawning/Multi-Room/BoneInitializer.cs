using UnityEngine;
using System.Collections.Generic;
using System;

public class BoneInitializer : KSMonoBehaviour
{
    public event Action<List<Transform>> OnBonesInitialized;
    public List<Transform> boneList = new List<Transform>();

    private void Start()
    {
        if(verbose) Debug.Log("BoneInitalizer.Awake is called");
    }

    public void InitializeBones()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bone"))
            {
                boneList.Add(child);
                child.gameObject.SetActive(false); //Initially disable the bone objects
                if (verbose) Debug.Log($"{child.gameObject.name} added to boneList and deactivated");
            }
        }
        OnBonesInitialized?.Invoke(boneList);
        if (verbose) Debug.Log("BoneInitializer.OnBonesInitialized invoked");
    }

    public Delegate[] GetOnBonesInitializedInvocationList()
    {
        if (OnBonesInitialized != null)
            return OnBonesInitialized.GetInvocationList();
        else
            return null;
    }

}
