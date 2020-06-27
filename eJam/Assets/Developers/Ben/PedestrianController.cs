using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PedestrianController : MonoBehaviour
{
    public GameObject selectionRing;
    
    private float _timePickedUp = 0f;
    private bool _isPickedUp = false;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        print($"Time: {_timePickedUp}");
    }

    public void PickUpPedestrian()
    {
        if (!_isPickedUp)
        {
            _timePickedUp = Time.time;
            selectionRing.SetActive(false);
            _isPickedUp = true;
        }
        else
        {
            selectionRing.SetActive(true);
            _isPickedUp = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("We found the player");
            PickUpPedestrian();

            RunController PlayerRef = other.GetComponent<RunController>();
            if (PlayerRef)
            {
                PlayerRef.GrabPedestrian(this);
            }
            else
            {
                Debug.Log("PedestrianController.cs: Hey I can't find the player!");
            }
        }
    }
}
