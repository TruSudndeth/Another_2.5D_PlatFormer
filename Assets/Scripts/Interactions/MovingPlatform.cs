using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            }
        }
    }
}
