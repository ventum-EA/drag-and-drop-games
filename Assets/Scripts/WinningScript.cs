using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WinningScript : MonoBehaviour
{
    public TimerScript timerScript;
    public DropPlaceScript DropPlaceScript;
    public bool end = false;
    public GameObject winPanel;
    public GameObject winTime;
    public GameObject winPoints;
    public GameObject[] stars;
    public int starCount=0;

    // Update is called once per frame
    private void Awake()
    {
        winPanel.SetActive(false);
        for (int i = 0; i < starCount; i++)
        {
            stars[i].GetComponent<UnityEngine.UI.Image>().color = new Color(0.345f, 0.345f, 0.345f);
        }

    }
    void Update()
    {
        if (timerScript != null && DropPlaceScript!=null)
        {
            if (DropPlaceScript.carCount < 1)
            {
                end = true;
            }  
        }


        if (end)
        {
            timerScript.timerOn = false;
            winPanel.SetActive(true);
            float winSeconds = timerScript.timeSeconds;
            float winMinutes=0;
            float winHours=0;
            do {
                if (winSeconds >= 60) { winMinutes += 1; winSeconds = 0; }
                if (winMinutes >= 60) { winHours += 1; winMinutes = 0; }
            }while(winSeconds>=60 || winMinutes>=60);
            winTime.GetComponent<TMP_Text>().text = string.Format("{0}h {1:00}m {2:00.00}s", winHours, winMinutes, System.Math.Round(winSeconds, 2));
            winPoints.GetComponent<TMP_Text>().text ="Points: "+ DropPlaceScript.points.ToString()+"/12";
            if (DropPlaceScript.points == 12)
            {
                starCount += 1;
            }
            if(winSeconds <= 240)
            {
                starCount += 1;
            }
            if(DropPlaceScript.points==12 && winSeconds <= 240)
            {
                starCount += 1;
            }
            for (int i = 0; i < starCount; i++)
            {
                stars[i].GetComponent<UnityEngine.UI.Image>().color  = new Color(1f,1f,1f);
            }
          
        }
    }
}
