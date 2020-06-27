using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropOffController : MonoBehaviour
{
    // public Transform dropOffLocation;
    public GameObject dropOffZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Creating drop off location...");
            var player = other.GetComponentInParent<RunController>();
            player.DropOffPedestrians(dropOffZone);
        }
    }
}
