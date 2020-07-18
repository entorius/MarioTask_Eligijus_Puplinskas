using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCoinsScript : MonoBehaviour
{
    public static int coinsValue = 0;
    Text coins;
    // Start is called before the first frame update
    void Start()
    {
        coins = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int coins = FindObjectOfType<ScoreManager>().GetCoins();
        int numbersCount = 1;
        string zerosString = "";
        while (coins >= 10)
        {
            numbersCount++;
            coins /= 10;
        }
        for (int i = 0; i < 2 - numbersCount; i++)
        {
            zerosString += "0";
        }
        this.coins.text = "X" + zerosString + FindObjectOfType<ScoreManager>().GetCoins();
    }
}
