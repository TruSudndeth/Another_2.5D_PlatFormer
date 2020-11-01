using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PointAPointB : MonoBehaviour
{
    [SerializeField]
    private Transform GameObject;
    [SerializeField]
    private PointAPointB PointA;
    [SerializeField]
    private PointAPointB PointB;
    [SerializeField]
    public bool _PointA = false;
    [SerializeField]
    public bool _PointB = false;

    private void Awake()
    {
        if (Application.isPlaying) enabled = false;
    }
    void Update()
    {
        if(_PointB && _PointA)
        {
            PointA._PointB = false;
            PointA._PointA = false;
            PointB._PointB = false;
            PointB._PointA = false;
        }
        if (_PointA)
        {
            PointA._PointB = false;
            PointB._PointB = false;
            GameObject.transform.position = PointA.transform.position;
        }
        if (_PointB)
        {
            PointB._PointA = false;
            PointA._PointA = false;
            GameObject.transform.position = PointB.transform.position;
        }
    }
}
