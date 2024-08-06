using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : KSMonoBehaviour
{
    public int spawnAmount;

    private static event Action OnAllBonesCollected;

    //public List<Transform> spawnPoints = new List<Transform>();
    public BoneInitializer[] boneInitializers;
    public List<Transform> bonePool;
    public List<GameObject> activeBones = new List<GameObject>();

    private void OnEnable()
    {
        OnAllBonesCollected += SpawnBones;
    }

    [Obsolete]
    private void Start()
    {
        Debug.Log("SpawnManager.Start() called");
        boneInitializers = FindObjectsOfType<BoneInitializer>();
        foreach (var initializer in boneInitializers)
        {
            initializer.OnBonesInitialized += AddBonesToPool;
            DebugBoneInitializedSubscribers(initializer);
            Debug.Log("AddBonesToPool should be subscribed");
        }    
    }

    private void AddBonesToPool(List<Transform> bonesList)
    {
        Debug.Log("AddBonesToPool is called");
        bonePool.AddRange(bonesList);
        CreateBonePool();
        SpawnBones();
    }

    private void CreateBonePool()
    {
        for (int i = 0; i < bonePool.Count; i++)
        {
            Transform temp = bonePool[i]; //temporarily holds the value of the current element at index "i" to facilitate the swap
            int randomIndex = UnityEngine.Random.Range(i, bonePool.Count); //generates a random index within the range of the current index
            bonePool[i] = bonePool[randomIndex]; //assigns the element at the randomly chosen "randomIndex" to the current index "i"
            bonePool[randomIndex] = temp; //elements at the current index "i" and the randomly chosen index "randomIndex" are swapped
        }
    }

    /*
    private void InitializeSpawnPoints()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("Bone");

        foreach (GameObject obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
            obj.SetActive(false);
        }
    }
    */

    private void SpawnBones()
    {
        if (bonePool != null && bonePool.Count > 0)
        {
            if (bonePool.Count < spawnAmount)
                spawnAmount = bonePool.Count;

            Debug.Log($"bonePool Amount: {bonePool.Count}");
            Debug.Log($"spawnAmount: {spawnAmount}");

            for (int i = 0; i < spawnAmount; i++)
            {
                //Debug.Log($"index value: {i}");
                GameObject bone = bonePool[i].gameObject;

                activeBones.Add(bone);

                Bone boneComponent = bone.GetComponent<Bone>();
                if (boneComponent != null)
                {
                    boneComponent.OnCollected += HandledBoneCollected;

                    //Uncomment to debug event subscribers
                    //DebugBoneCollectedSubscribers(boneComponent);
                }
                else
                    Debug.Log($"{bone.name} is missing the boneComponent");

                bonePool[i].gameObject.SetActive(true);
            }
        }
        else
        {
            //GameOver Logic
            Debug.Log("NO BONES TO SPAWN");
            return;
        }

        bonePool.RemoveRange(0, spawnAmount);
    }

    private void HandledBoneCollected(GameObject bone)
    {
        //Debug.Log($"HandledBoneCollected() has been executed");

        activeBones.Remove(bone.gameObject);

        if (activeBones.Count == 0)
        {
            //Debug.Log("All Bones Collected");
            OnAllBonesCollected?.Invoke();
        }
    }

    private void OnDisable()
    {
        OnAllBonesCollected -= SpawnBones;
    }

    private void DebugBoneCollectedSubscribers(Bone boneComponent)
    {
        Delegate[] subscribers = boneComponent.GetOnCollectedInvocationList();

        if (subscribers != null)
        {
            foreach (var subscriber in subscribers)
                Debug.Log($"{boneComponent.gameObject.name}'s OnCollected event subscriber: {subscriber.Method.Name}");
        }
        else
            Debug.Log("No subscribers for OnCollected event");
    }

    private void DebugBoneInitializedSubscribers(BoneInitializer boneInitializer)
    {
        Delegate[] subscribers = boneInitializer.GetOnBonesInitializedInvocationList();

        if (subscribers != null)
        {
            foreach (var subscriber in subscribers)
                Debug.Log($"{boneInitializer.gameObject.name}'s event subscriber: {subscriber.Method.Name}");
        }
        else
            Debug.Log("No subscribers for BoneInitialized event");
    }
}
