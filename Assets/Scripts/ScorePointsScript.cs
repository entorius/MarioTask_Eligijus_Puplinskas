using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePointsScript : MonoBehaviour
{
    public static int scoreValue = 0;
    Text score;
    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int score = FindObjectOfType<ScoreManager>().GetScore();
        int zerosCount = 0;
        string zerosString = "";
        while(score >= 1)   //Counts how many zeros to add to begining of the score
        {
            zerosCount++;
            score /= 10;
        }
        for(int i = 0; i < 6 - zerosCount; i++) //adds zeros to string
        {
            zerosString += "0";
        }
        this.score.text = zerosString + FindObjectOfType<ScoreManager>().GetScore();     //creates final score with zeros and score
    }
}
