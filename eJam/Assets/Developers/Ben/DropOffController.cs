using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropOffController : MonoBehaviour
{
    // public Transform dropOffLocation;
    public GameObject dropOffZone;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Creating drop off location...");
            var player = other.GetComponentInParent<RunController>();
            GetDropOffLocation(player);
        }
    }

    private void GetDropOffLocation(RunController player)
    {
        var worldSpaceVector = dropOffZone.transform.TransformPoint(transform.position);

        var colliderScaleX = dropOffZone.transform.localScale.x / 2;
        var colliderScaleZ = dropOffZone.transform.localScale.z / 2;
        
        var randomX = Random.Range(worldSpaceVector.x - colliderScaleX, worldSpaceVector.x + colliderScaleX);
        var randomZ = Random.Range(worldSpaceVector.z - colliderScaleZ, worldSpaceVector.z + colliderScaleZ);
        var randomDropOffVector = new Vector3(randomX, 0f, randomZ);
        
        print(randomDropOffVector);
        player.DropOffPedestrians(randomDropOffVector);
    }
}
