using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropOffController : MonoBehaviour
{
    // public Transform dropOffLocation;
    public GameObject dropOffZone;

    bool bDroppingOff = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !bDroppingOff)
        {
            print("Creating drop off location...");
            var player = other.GetComponentInParent<RunController>();
            bDroppingOff = player.DropOffPedestrians(dropOffZone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bDroppingOff = false;
        }
    }
}
