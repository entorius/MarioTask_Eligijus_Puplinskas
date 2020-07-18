using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeCountScript : MonoBehaviour
{
    public static int startTime = 0;
    Text time;
    // Start is called before the first frame update
    void Start()
    {
        time = GetComponent<Text>();
        startTime = FindObjectOfType<ScoreManager>().startTimeSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        time.text = ((int)FindObjectOfType<ScoreManager>().GetCurrentTime()).ToString();
    }
}
