using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    private bool LeftLedge = true;
    private Vector3 _LedgeOffset;

    public Vector3 ReverseLedgeGrab(int _DirectionTraveling)
    {
        if (_DirectionTraveling > 0) LeftLedge = true;
        if (_DirectionTraveling < 0) LeftLedge = false;

        _LedgeOffset = transform.position;
        _LedgeOffset.y += transform.localScale.y / 2;

        if (!LeftLedge)// Right Side Ledge Grab
        {
            _LedgeOffset.z += transform.localScale.z / 2;
        }

        if (LeftLedge) // Left Side Ledge Grab
        {
            _LedgeOffset.z -= transform.localScale.z / 2;
        }
        _LedgeOffset.x = 0;

        return _LedgeOffset;

    }
}
