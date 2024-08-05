using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : MonoBehaviour
{
    public int spawnAmount;

    private static event Action OnAllBonesCollected;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> bonePool;
    private List<GameObject> activeBones = new List<GameObject>();

    private void OnEnable()
    {
        OnAllBonesCollected += SpawnBones;
    }
    private void Start()
    {
        InitializeSpawnPoints();
        CreateBonePool(spawnPoints);
        SpawnBones();
    }

    private void CreateBonePool(List<Transform> spawnPoints)
    {
        bonePool = new List<Transform>(spawnPoints); ////Copying the SpawnPoints List

        for (int i = 0; i < bonePool.Count; i++)
        {
            Transform temp = bonePool[i]; //temporarily holds the value of the current element at index "i" to facilitate the swap
            int randomIndex = UnityEngine.Random.Range(i, bonePool.Count); //generates a random index within the range of the current index
            bonePool[i] = bonePool[randomIndex]; //assigns the element at the randomly chosen "randomIndex" to the current index "i"
            bonePool[randomIndex] = temp; //elements at the current index "i" and the randomly chosen index "randomIndex" are swapped
        }
    }

    private void InitializeSpawnPoints()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("Bone");

        foreach (GameObject obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
            obj.SetActive(false);
        }
    }

    private void SpawnBones()
    {
        if (bonePool.Count < spawnAmount)
            spawnAmount = bonePool.Count;

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject bone = bonePool[i].gameObject;
            activeBones.Add(bone);
            bonePool.Remove(bone.transform);
            bone.GetComponent<Bone>().OnCollected += HandledBoneCollected;
            bonePool[i].gameObject.SetActive(true);
        }
    }

    private void HandledBoneCollected(GameObject bone)
    {
        activeBones.Remove(bone);

        if (activeBones.Count == 0)
            OnAllBonesCollected?.Invoke();
    }

    private void OnDisable()
    {
        OnAllBonesCollected -= SpawnBones;
    }
}
