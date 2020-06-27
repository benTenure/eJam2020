using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsticleSpawner : MonoBehaviour
{
    public ObsticleSpawnerData data;

    private void Awake()
    {
        if (Random.Range(0, 100) < data.spawnChance)
        {
            int r = Random.Range(0, data.obsticles.Count);
            GameObject newObj = Instantiate(data.obsticles[r], transform);
            newObj.transform.localPosition = Vector3.zero;
            if (data.randomRotation)
            {
                r = Random.Range(0, 360);
                newObj.transform.eulerAngles = new Vector3(0, r, 0);
            }
        }
    }

}
