using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroAvatar_Offset : MonoBehaviour
{
    [SerializeField]
    private Follow ModelOffset;
    private void FixedUpdate()
    {
        Vector3 ModelOffsetInc = ScaleModelOffset(ModelOffset.transformOffset);
        transform.position += ModelOffsetInc;
    }

    private Vector3 ScaleModelOffset(Vector3 _Scaled)
    {
        _Scaled.x *= transform.localScale.x;
        _Scaled.y *= transform.localScale.y;
        _Scaled.z *= transform.localScale.z;
        return _Scaled;
    }
}
