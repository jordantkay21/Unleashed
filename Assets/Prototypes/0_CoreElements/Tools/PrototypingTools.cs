using UnityEngine;
using System.Collections.Generic;

public class PrototypingTools : MonoBehaviour
{
    public SpawnManager spawnManager;

    public void CollectAllActiveBones()
    {
        List<GameObject> tempBones = new List<GameObject>(spawnManager.activeBones);

        foreach(GameObject boneObj in tempBones)
        {
            Bone boneComponent = boneObj.GetComponent<Bone>();
            if (boneComponent != null)
                boneComponent.Collect();
        }

        //spawnManager.activeBones.Clear();
    }
}
