using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameController : MonoBehaviour
{
    public GameObject[] itemBoxes;
    GameObject pauseMenu;
    public static List<GameObject> players;
    int countDownTime;
    TextMeshProUGUI countDown;
    ButtonPause joystickPause;
    public static bool StartGame;
    void Start()
    {
        itemBoxes = GameObject.FindGameObjectsWithTag("ItemBoxes");
        pauseMenu = GameObject.FindGameObjectsWithTag("Pause")[0];
        pauseMenu.active = false;
        players = new List<GameObject>();
        countDownTime = 3;
        StartGame = false;
        joystickPause = FindObjectOfType<ButtonPause>();
        countDown= GameObject.FindGameObjectsWithTag("CountDown")[0].GetComponent<TMPro.TextMeshProUGUI>();
        countDown.text = "Waiting for other players";
        //StartCoroutine(CountDownTimer());
    }
    int ind = 0;
  /*  public void SetPlayers()
    {
        GameObject[] p;
        if (ind == 0)
        {
            p = GameObject.FindGameObjectsWithTag("Player");
            ind = 1;
            players.Add(p[0].GetComponent<CheckPointCounter>());
        }
         p=GameObject.FindGameObjectsWithTag("NPC");

        for (int i = 0; i < p.Length; i++) players.Add( p[i].GetComponent<CheckPointCounter>());
    }*/
    IEnumerator CountDownTimer()
    {
        while(countDownTime>0)
        {
            countDown.text =""+ countDownTime;
            yield return new WaitForSeconds(1f);
            countDownTime--;
        }
        countDown.text = "GO!";
       StartGame = true;
        yield return new WaitForSeconds(1f);
        GameObject.FindGameObjectsWithTag("CountDown")[0].SetActive(false);
    }

    public void ResetItemBoxes()
    {
        for (int i = 0; i < itemBoxes.Length; i++) itemBoxes[i].active = true;
    }
    void Respawn_Player()
    {
        if (Input.GetKey("r")) Respawn.RespawnPlayer(GameObject.FindGameObjectsWithTag("Player")[0]);
    }
    

    void Pause()
    {
        if(Input.GetAxisRaw("Cancel") != 0 || (joystickPause!=null&&joystickPause.pressed==true))
        {
            if (pauseMenu.active == false) pauseMenu.active = true;
            else pauseMenu.active = false;
        }
    }

    void Positionig()
    {
        for(int i=0;i<players.Count;i++)
            for (int j = i+1; j < players.Count; j++)
            {
            if ((players[j].GetComponent<CheckPointCounter>().lap >= players[i].GetComponent<CheckPointCounter>().lap && players[j].GetComponent<CheckPointCounter>().checkpoint > players[i].GetComponent<CheckPointCounter>().checkpoint)|| (players[j].GetComponent<CheckPointCounter>().lap > players[i].GetComponent<CheckPointCounter>().lap))
            {
                    GameObject kl;
                    kl = players[i];
                    players[i] = players[j];
                    players[j] = kl;
            }
            else if(players[j].GetComponent<CheckPointCounter>().lap == players[i].GetComponent<CheckPointCounter>().lap && players[j].GetComponent<CheckPointCounter>().checkpoint == players[i].GetComponent<CheckPointCounter>().checkpoint)
                {
                    float d1, d2;
                    int checkpoint;
                    if (players[i].GetComponent<CheckPointCounter>().checkpoint < KartLap.CheckpointsCount) checkpoint = players[i].GetComponent<CheckPointCounter>().checkpoint;
                    else checkpoint = 0;

                    d1 = Vector3.Distance(players[i].transform.position, GameObject.FindGameObjectsWithTag("CheckPoint")[checkpoint].transform.position);
                    d2 = Vector3.Distance(players[j].transform.position, GameObject.FindGameObjectsWithTag("CheckPoint")[checkpoint].transform.position);


                    if(d2<d1)
                    {
                        GameObject kl;
                        kl = players[i];
                        players[i] = players[j];
                        players[j] = kl;
                    }
                }
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<CheckPointCounter>().rank = i + 1;
          //  players[i].DisplayRank();
        }

    }
    public  int fix = 0;
    void FixedUpdate()
    {
        if(StartGame)Respawn_Player();
            Pause();
            Positionig();
        if (GameManager.startGame && fix == 0)
        {
            fix = 1;
            GameObject.FindGameObjectsWithTag("CountDown")[0].GetComponent<AudioSource>().Play();
           StartCoroutine(CountDownTimer());
           
        }
    }
}
