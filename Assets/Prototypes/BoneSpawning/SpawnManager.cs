using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : MonoBehaviour
{
    public List<Transform> spawnPoints;

    private void Start()
    {
        InitializeSpawnPoints();
        SpawnBones();
    }

    private void InitializeSpawnPoints()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("Bone");
        spawnPoints = new List<Transform>();

        foreach (GameObject obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
            obj.SetActive(false);
        }
    }

    private void SpawnBones()
    {
        List<Transform> selectedPoints = new List<Transform>(spawnPoints); ////Copying the SpawnPoints List

        for (int i = 0; i < selectedPoints.Count; i++)
        {
            Transform temp = selectedPoints[i]; //temporarily holds the value of the current element at index "i" to facilitate the swap
            int randomIndex = UnityEngine.Random.Range(i, selectedPoints.Count); //generates a random index within the range of the current index
            selectedPoints[i] = selectedPoints[randomIndex]; //assigns the element at the randomly chosen "randomIndex" to the current index "i"
            selectedPoints[randomIndex] = temp; //elements at the current index "i" and the randomly chosen index "randomIndex" are swapped
        }
    
        for (int i=0; i<10; i++)
        {
            selectedPoints[i].gameObject.SetActive(true);
        }
    }
}
