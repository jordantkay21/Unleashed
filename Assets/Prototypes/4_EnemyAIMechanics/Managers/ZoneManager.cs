using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager instance;

    [System.Serializable]
    public class Zone
    {
        public string zoneName;
        public List<Transform> patrolPoints = new List<Transform>();
    }

    public List<Zone> zones = new List<Zone>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeZones();
    }

    private void InitializeZones()
    {
        Debug.Log("Initializing Zones...");

        //Find all the zones (parent objects) in the scene
        foreach (Transform zoneTransform in transform)
        {
            Debug.Log("Found Zone: " + zoneTransform.name);

            if (zoneTransform.childCount == 0)
            {
                Debug.LogWarning($"Zone {zoneTransform.name} has no child patrol points.");
                continue;
            }

            Zone newZone = new Zone
            {
                zoneName = zoneTransform.name
            };

            //Find all patrol points under this zone (children)
            foreach (Transform patrolPoint in zoneTransform)
            {
                if (patrolPoint == null)
                {
                    Debug.LogWarning($"Null patrol point found in {zoneTransform.name}");
                    continue;
                }

                newZone.patrolPoints.Add(patrolPoint);
                Debug.Log($"Added Patrol Point: {patrolPoint.name}");
            }

            //Add this zone to the list
            zones.Add(newZone);
        }

        Debug.Log("Zone Initialization Complete. Total zones initialized: " + zones.Count);
    }

    public List<Transform> GetPatrolPointsForZone(string zoneName)
    {
        foreach (Zone zone in zones)
        {
            if (zone.zoneName == zoneName)
                return zone.patrolPoints;
        }
        
        return null;
    }
}
