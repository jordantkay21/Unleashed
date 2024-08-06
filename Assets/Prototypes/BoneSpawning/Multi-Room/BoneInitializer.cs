using UnityEngine;
using System.Collections.Generic;
using System;

public class BoneInitializer : KSMonoBehaviour
{
    public event Action<List<Transform>> OnBonesInitialized;
    public List<Transform> boneList = new List<Transform>();

    private void Start()
    {
        Debug.Log("BoneInitalizer.Awake is called");
        boneList = InitializeBones();
        OnBonesInitialized?.Invoke(boneList);
        Debug.Log("BoneInitializer.OnBonesInitialized invoked");
    }

    private List<Transform> InitializeBones()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bone"))
            {
                boneList.Add(child);
                child.gameObject.SetActive(false); //Initially disable the bone objects
                Debug.Log($"{child.gameObject.name} added to boneList and deactivated");
            }
        }

            return boneList;
    }

    public Delegate[] GetOnBonesInitializedInvocationList()
    {
        if (OnBonesInitialized != null)
            return OnBonesInitialized.GetInvocationList();
        else
            return null;
    }

}
