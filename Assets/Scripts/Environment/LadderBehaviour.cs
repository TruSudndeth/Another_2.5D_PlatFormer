using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderBehaviour : MonoBehaviour
{
    [SerializeField]
    private float LadderLedgeGrabTilt;
    [SerializeField]
    private bool GrabRight = true;
    private Vector3 CurrentRotation;

    void Start()
    {
        // 90 is grab left tilt z -0.1f
        if(GrabRight)
        {
            CurrentRotation = new Vector3(0, 270, 0.1f);
        }
        // 270 is grab right tilt z +0.1f
        else
        {
            CurrentRotation = new Vector3(0, 90, -0.1f);
        }
        transform.rotation = Quaternion.Euler(CurrentRotation);
    }
}
