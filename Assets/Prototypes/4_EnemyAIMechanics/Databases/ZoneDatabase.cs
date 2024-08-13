using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZoneDatabase", menuName = "Scriptable Objects/ZoneDatabase", order=1)]
public class ZoneDatabase : ScriptableObject
{
    public List<string> availableZones;
}
