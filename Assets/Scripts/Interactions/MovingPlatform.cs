using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player_Controller PlayerController = other.GetComponent<Player_Controller>();
            if(PlayerController != null)
            {
                PlayerController.MovingPlatform = transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            Player_Controller PlayerController = other.GetComponent<Player_Controller>();
            if (PlayerController != null)
            {
                PlayerController.MovingPlatform = null;
                PlayerController.Jumped = false;
            }
        }
    }
}
