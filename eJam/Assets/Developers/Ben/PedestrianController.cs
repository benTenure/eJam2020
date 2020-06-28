using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PedestrianController : MonoBehaviour
{
    public GameObject selectionRing;
    public Animator anim;
    
    private float _timePickedUp = 0f;
    private bool _isPickedUp = false;
    private static readonly int PickUp = Animator.StringToHash("PickUp");

    private void PickUpPedestrian()
    {
        if (!_isPickedUp)
        {
            _timePickedUp = Time.time;
            selectionRing.SetActive(false);
            _isPickedUp = true;
            anim.SetTrigger(PickUp);
        }
        else
        {
            selectionRing.SetActive(true);
            _isPickedUp = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isPickedUp)
        {
            PickUpPedestrian();

            var playerRef = other.transform.parent.GetComponent<RunController>();
            
            if (playerRef)
            {
                playerRef.GrabPedestrian(this);
            }
            else
            {
                Debug.Log("PedestrianController.cs: Hey I can't find the player!");
            }
        }
    }

    public void BeenDroppedOff(Transform lookDir)
    {
        anim.SetTrigger("SafeZone");
        if (lookDir)
        {
            transform.LookAt(lookDir);
        }
        else
        {
            transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, transform.rotation.w);
        }
    }
}
