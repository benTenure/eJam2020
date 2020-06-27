using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObsticleSpawnerData", order = 1)]
public class ObsticleSpawnerData : ScriptableObject
{
    public List<GameObject> obsticles;
    [Range(0, 100)]
    public float spawnChance;
    public bool randomRotation;
}
