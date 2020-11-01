using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private Transform PointA;
    [SerializeField]
    private Transform PointB;
    [SerializeField]
    private float PlatTravelTime_sec;
    [SerializeField]
    private float PlatWaitForSec;
    private Vector3 TargetPosition;
    private bool _PointA = true;
    private bool YouHaveToWait = false;
    private float Speed;
    private IEnumerator _PlatformWaitForTime;

    void Start()
    {
        Speed = Vector3.Distance(PointA.transform.position, PointB.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (_PointA)
        {
            if(TargetPosition != PointA.transform.position)TargetPosition = PointA.transform.position;
        }
        if (!_PointA)
        {
            if(TargetPosition != PointB.transform.position)TargetPosition = PointB.transform.position;
        }

        if (transform.position != TargetPosition)
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, (Time.deltaTime * Speed) / PlatTravelTime_sec);
        else if (!YouHaveToWait)
        {
            YouHaveToWait = true;
            _PlatformWaitForTime = PlatformWaitForTime(PlatWaitForSec);
            StartCoroutine(_PlatformWaitForTime);
        }
    }

    private IEnumerator PlatformWaitForTime(float _Time)
    {
        yield return new WaitForSeconds(_Time);
        YouHaveToWait = false;
        _PointA = !_PointA;
        StopCoroutine(_PlatformWaitForTime);
    }
}
