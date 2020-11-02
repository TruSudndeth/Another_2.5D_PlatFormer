using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Behaviour : MonoBehaviour
{
    //ToDo's
    // Shiny Gold Sparkles
    private float Speed;
    private float Rotation;

    private void Start()
    {
        Speed = Random.Range(100.0f, 200.0f);
    }
    void FixedUpdate()
    {
        Rotation += Time.fixedDeltaTime * Speed;
        transform.rotation = Quaternion.Euler(0, Rotation, 0);
    }
}
