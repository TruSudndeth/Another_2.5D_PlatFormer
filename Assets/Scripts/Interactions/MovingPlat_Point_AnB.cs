using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlat_Point_AnB : MonoBehaviour
{
    [SerializeField]
    private float Speed = 1;
    [SerializeField]
    private Transform PointA;
    [SerializeField]
    private Transform PointB;
    private bool _PointA = false;

    private Vector3 CurrentPosition;
    private Vector3 GoToPosition;
    // Start is called before the first frame update
    void Start()
    {
        CurrentPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Speed > 50) Speed = 50f;
        if (_PointA)
        {
            if(GoToPosition != PointA.position)GoToPosition = PointA.position;
        }
        if (!_PointA)
        {
            if(GoToPosition != PointB.position)GoToPosition = PointB.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, GoToPosition, Time.deltaTime * Speed);
        if (Vector3.Distance(transform.position, GoToPosition) <= 0) _PointA = !_PointA;
    }
}
