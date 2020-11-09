using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField]
    private Follow ChildOffset;
    [SerializeField]
    private Vector3 StartLocalPosition;
    private Vector3 _transformOffset;
    
    private void Awake()
    {
        StartLocalPosition = transform.localPosition;
    }
    public Vector3 transformOffset
    {
        get
        {
            if (ChildOffset) _transformOffset += ChildOffset.transformOffset;
            _transformOffset = transform.localPosition - StartLocalPosition;
            transform.localPosition = StartLocalPosition;
            return _transformOffset;
        }
    }
}
