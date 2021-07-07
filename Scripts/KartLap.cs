using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
struct PlayerTime
{
   public int minutes;
   public float seconds;
}
public class KartLap : MonoBehaviour
{
    TextMeshProUGUI LapCounter;
    TextMeshProUGUI TimeCounter;
    TextMeshProUGUI stats;
    GameController gameController;
    PlayerTime[] time;
    float syncFix;
    public GameObject EndGame;
    bool over;
    public static int LapCount;
    public static int CheckpointsCount;
    void Start()
    {
        LapCount = 3;
        CheckpointsCount = 14;
        time = new PlayerTime[LapCount + 1];

        LapCounter = GameObject.FindGameObjectsWithTag("LapCounter")[0].GetComponent<TMPro.TextMeshProUGUI>();
        TimeCounter= GameObject.FindGameObjectsWithTag("TimeCounter")[0].GetComponent<TMPro.TextMeshProUGUI>();
        EndGame = GameObject.FindGameObjectsWithTag("EndGame")[0];
        stats = GameObject.FindGameObjectsWithTag("Stats")[0].GetComponent<TMPro.TextMeshProUGUI>();
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        GameObject.FindGameObjectsWithTag("EndGame")[0].active = false;
        over = false;

    }


    public void Check(int index)
    {
        if (over == true) return;
        if (this.GetComponent<CheckPointCounter>().checkpoint < index)
        {

            this.GetComponent<CheckPointCounter>().checkpoint++;
        }
        else if (this.GetComponent<CheckPointCounter>().checkpoint == CheckpointsCount && index == 1)
        {
            float seconds = 0;
            for (int i = 1; i <= this.GetComponent<CheckPointCounter>().lap; i++)
            {
                 seconds += time[i].seconds + time[i].minutes * 60f;
            }
            time[this.GetComponent<CheckPointCounter>().lap].minutes = time[0].minutes - (int)(seconds / 60f);
            seconds = time[0].seconds - seconds % 60;
            if (seconds < 0)
            {
                time[this.GetComponent<CheckPointCounter>().lap].minutes--;
                time[this.GetComponent<CheckPointCounter>().lap].seconds = 60 + seconds;
            }
            else time[this.GetComponent<CheckPointCounter>().lap].seconds = seconds;
            if (this.GetComponent<CheckPointCounter>().lap == LapCount)
            {
                EndGame.active = true;
                over = true;
                float gameTime = time[0].minutes * 60 + time[0].seconds;
                float minLap = time[1].minutes * 60 + time[1].seconds;
                for(int i=2;i<time.Length;i++)if(time[i].minutes*60+time[i].seconds<minLap)minLap= minLap = time[i].minutes * 60 + time[i].seconds;
                ClientSend.endGameReceived(this.GetComponent<CheckPointCounter>().rank,minLap,gameTime);
            }
            else
            {
                this.GetComponent<CheckPointCounter>().lap++;
                this.GetComponent<CheckPointCounter>().checkpoint = 1;
                gameController.ResetItemBoxes();
            }
        }
    }


    public void DisplayRank()
    {
        if (this.GetComponent<CheckPointCounter>().rank == 1) GameObject.FindGameObjectsWithTag("Rank")[0].GetComponent<TMPro.TextMeshProUGUI>().text = "1st";
        else if(this.GetComponent<CheckPointCounter>().rank ==2) GameObject.FindGameObjectsWithTag("Rank")[0].GetComponent<TMPro.TextMeshProUGUI>().text = "2nd";
        else GameObject.FindGameObjectsWithTag("Rank")[0].GetComponent<TMPro.TextMeshProUGUI>().text = ""+ this.GetComponent<CheckPointCounter>().rank +"th";
    }
    int fix = 0;
    // Update is called once per frame
    void Update()
    {
        DisplayRank();
        if (over == false)
        {
            if (GameController.StartGame)
            {
                if (fix == 0)
                {
                    fix = 1;
                    syncFix = Time.time;
                }
                LapCounter.text = "Lap:" + this.GetComponent<CheckPointCounter>().lap + "/" + LapCount;
                time[0].minutes = (int)((Time.time - syncFix) / 60f);
                time[0].seconds = (Time.time - syncFix) % 60f;
                TimeCounter.text = "Time:" + time[0].minutes.ToString("00") + ":" + time[0].seconds.ToString("00.00");
            }
            else TimeCounter.text = "Time:" + time[0].minutes.ToString("00") + ":" + time[0].seconds.ToString("00.00");
        }
        else
        {
            string StatString="";
            StatString+="Your Time:"+ time[0].minutes.ToString("00") + ":" + time[0].seconds.ToString("00.00")+"\n";
            for (int i = 1; i <= LapCount; i++)
                StatString += "Lap:" + i + " Lap Time:" + time[i].minutes.ToString("00") + ":" + time[i].seconds.ToString("00.00") + "\n";
            stats.text = StatString;
        }
    }
}
