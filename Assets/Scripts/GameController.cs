using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CanvasController _CanvasController;
    private int Coins = 0;
    private int Health = 3;
    private int Lifes = 3;

    void Awake()
    {
        Collectable_Type.Collected += Collectables;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Collectables(Collect_Type _CollectType)
    {
        switch (_CollectType)
        {
            case Collect_Type.Coin:
                Coins++;
                _CanvasController.UpdateCoin(Coins);
                break;
            case Collect_Type.Health:
                Health++;
                break;
            case Collect_Type.Lives:
                Lifes++;
                break;
        }
    }

    private void OnDestroy()
    {
        Collectable_Type.Collected -= Collectables;
    }
}
