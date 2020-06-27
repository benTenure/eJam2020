using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianController : MonoBehaviour
{
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

        if (Input.GetKeyUp(KeyCode.Q))
        {
            PickUpPedestrian();
        }
    }

    public void PickUpPedestrian()
    {
        _timePickedUp = Time.time;
    }

    public void DropPedestrian()
    {
        _timePickedUp = 0.0f;
    }
}
