using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WorldGenData", order = 1)]
public class WorldGenData : ScriptableObject
{
    [Header("Asset Settings")]
    public List<GameObject> TopSidewalks;
    public List<GameObject> BottomSidewalks;
    public List<GameObject> Buildings;
    public List<GameObject> Roads;

    public List<GameObject> SafeZones;

    [Header("Generation Settings")]

    [Range(4, 10)]
    public int lengthGenerated;

    [Range(10, 40)]
    public int minimumDistanceBetweenSafeZones;

    [Range(0, 20)]
    public int safeZoneDistanceVariance;



    [Header("Scroll Settings")]

    public float scrollSpeed;
}
