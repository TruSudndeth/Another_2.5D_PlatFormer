using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Collectable_Type : MonoBehaviour
{
    [SerializeField]
    private Collect_Type CollectableType;

    public delegate void _CollectableType_(Collect_Type Type);
    public static event _CollectableType_ Collected;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Collected?.Invoke(CollectableType);
            Destroy(gameObject);
        }
    }
}
