using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Player_LedgeGrab : MonoBehaviour
{
    [SerializeField]
    private bool LeftLedge = true;
    [SerializeField]
    private Player_Controller PlayerController;
    private Vector3 _PlayerLedgeGrabOffset;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LatterEnds")
        {
            PlayerController.StepOverLadder(true);
        }
    }
    public Vector3 ReverseLedgeGrab(int _DirectionTraveling)
    {
        if (_DirectionTraveling > 0) LeftLedge = true;
        if (_DirectionTraveling < 0) LeftLedge = false;
        _PlayerLedgeGrabOffset = transform.position;
        _PlayerLedgeGrabOffset.y += transform.localScale.y;
        

        // invert this to grab other side of ledge
        if (!LeftLedge) // Right Side Ledge Grab
        {
            _PlayerLedgeGrabOffset.z += transform.localScale.z;
        }

        if(LeftLedge) // Left Side Ledge Grab
        {
            _PlayerLedgeGrabOffset.z -= transform.localScale.z;
        }

        
        _PlayerLedgeGrabOffset.x = 0;

        return _PlayerLedgeGrabOffset;
    }
}
