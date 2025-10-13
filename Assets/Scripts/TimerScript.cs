using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float timeSeconds;
    private int timeMinutes;
    private int timeHours;
    public GameObject Timer;
    public bool timerOn = true;
    
    private string timerTemp;
    void Start()
    {
        timeMinutes = 0;
        timeHours = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {

            timeSeconds += Time.deltaTime;

            if (timeSeconds >= 60) { timeMinutes += 1; timeSeconds = 0; }
            if (timeMinutes >= 60) { timeHours += 1; timeMinutes = 0; }
            Timer.GetComponent<TMP_Text>().text = string.Format("{0}h {1:00}m {2:00.00}s", timeHours, timeMinutes, System.Math.Round(timeSeconds, 2));
        }
    }
}
