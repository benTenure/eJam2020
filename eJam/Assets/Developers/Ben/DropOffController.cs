using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropOffController : MonoBehaviour
{
    public Transform dropOffLocation;
    
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
        var player = other.GetComponent<RunController>();

        var randomX = Random.Range();
        var randomZ = Random.Range();
        var randomDropOffVector = new Vector3();
    }
}
