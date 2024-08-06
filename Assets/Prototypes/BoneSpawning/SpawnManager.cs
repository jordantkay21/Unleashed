using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : KSMonoBehaviour
{
    public int spawnAmount;

    private static event Action OnAllBonesCollected;

    public BoneInitializer[] boneInitializersArray;
    public List<BoneInitializer> boneInitializers;

    public BoneInitializer currentInitializer;

    public List<Transform> bonePool;
    public List<GameObject> activeBones = new List<GameObject>();

    private void OnEnable()
    {
        OnAllBonesCollected += SpawnBones;
        OnAllBonesCollected += SelectRandomInitializer;
    }

    [Obsolete]
    private void Start()
    {
        
        if(verbose) Debug.Log("SpawnManager.Start() called");
        boneInitializersArray = FindObjectsOfType<BoneInitializer>();
        boneInitializers = new List<BoneInitializer>(boneInitializersArray);
        InitilizeStartingArea();  
    }

    #region Bone Initilizers Management
    /// <summary>
    /// Initializes the starting area by finding and activating the initial boneInitializer
    /// </summary>
    private void InitilizeStartingArea()
    {
        BoneInitializer foundInitializer = null;

        for (int i = 0; i < boneInitializers.Count; i++)
        {
            if (boneInitializers[i].CompareTag("StartingArea"))
            {
                int currentIndex = i;
                foundInitializer = boneInitializers[i];
                
                foundInitializer.OnBonesInitialized += AddBonesToPool;
                
                if (verbose) DebugBoneInitializedSubscribers(currentInitializer);

                foundInitializer.InitializeBones();
                boneInitializers.RemoveAt(currentIndex);
            }
        }

    }

    /// <summary>
    /// Randomly selects a BoneInitilizer to activate after all bones are collected
    /// </summary>
    private void SelectRandomInitializer()
    {
        if (boneInitializers.Count > 0)
        {
            int currentIndex = UnityEngine.Random.Range(0, boneInitializers.Count);

            currentInitializer = boneInitializers[currentIndex];
            currentInitializer.OnBonesInitialized += AddBonesToPool;
            if (verbose) DebugBoneInitializedSubscribers(currentInitializer);
            if (verbose) Debug.Log("AddBonesToPool should be subscribed");

            currentInitializer.InitializeBones();
            boneInitializers.RemoveAt(currentIndex); //prevents area from being selected again
        }
        else
        {
            //GameOver Logic
            Debug.Log("GameOver");
            return;
        }
    }
    #endregion

    #region Bone Pool Management
    /// <summary>
    /// Adds bones to the pool and prepares for spawning
    /// </summary>
    /// <param name="bonesList">List of bones from the BoneInitilizers</param>
    private void AddBonesToPool(List<Transform> bonesList)
    {
       if(verbose) Debug.Log("AddBonesToPool is called");
        bonePool.AddRange(bonesList);
        ShuffleBonePool();
        SpawnBones();
    }

    /// <summary>
    /// Shuffles the bone pool to randomize spawn locations
    /// </summary>
    private void ShuffleBonePool()
    {
        for (int i = 0; i < bonePool.Count; i++)
        {
            Transform temp = bonePool[i]; //temporarily holds the value of the current element at index "i" to facilitate the swap
            int randomIndex = UnityEngine.Random.Range(i, bonePool.Count); //generates a random index within the range of the current index
            bonePool[i] = bonePool[randomIndex]; //assigns the element at the randomly chosen "randomIndex" to the current index "i"
            bonePool[randomIndex] = temp; //elements at the current index "i" and the randomly chosen index "randomIndex" are swapped
        }
    }
    #endregion

    /// <summary>
    /// Spawns bones from the bone pool
    /// </summary>
    private void SpawnBones()
    {
        if(bonePool.Count > 0)
        {
            spawnAmount = Mathf.Min(spawnAmount, bonePool.Count);
            for (int i=0; i < spawnAmount; i++)
            {
                GameObject bone = bonePool[i].gameObject;
                activeBones.Add(bone);
                HandleBoneComponent(bone);
                bone.SetActive(true);
            }
            bonePool.RemoveRange(0, spawnAmount);
        }
        else
        {
            Debug.Log("NO BONES TO SPAWN - GameOver possible");
        }
    }

    private void HandleBoneComponent(GameObject bone)
    {
        Bone boneComponent = bone.GetComponent<Bone>();

        if (boneComponent != null)
        {
            boneComponent.OnCollected += HandledBoneCollected;
            if (verbose) DebugBoneCollectedSubscribers(boneComponent);
        }
        else
        {
            Debug.Log("${bone.name} is missing the Bone component");
        }
    }

    private void HandledBoneCollected(GameObject bone)
    {
        activeBones.Remove(bone.gameObject);
        if (activeBones.Count == 0)
            OnAllBonesCollected?.Invoke();
    }

    private void OnDisable()
    {
        OnAllBonesCollected -= SpawnBones;
        OnAllBonesCollected -= SelectRandomInitializer;
    }

    #region Debugging Helpers
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
    #endregion
}
