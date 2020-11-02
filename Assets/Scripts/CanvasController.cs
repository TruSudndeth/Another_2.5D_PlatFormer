using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Coins_Text;
    private int coins = 0;
    // Start is called before the first frame update
    void Awake()
    {
        UpdateCanvasAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCanvasAll()
    {
        Coins_Text.text = "Coin: " + coins;
    }

    public void UpdateCoin(int _Coins)
    {
        if (_Coins > coins + 1) Debug.Log("Coin Inc too High");
        coins = _Coins;
        UpdateCanvasAll();
    }
}
