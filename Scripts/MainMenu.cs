using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public   TMPro.TextMeshProUGUI Ranking;
  
 
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance exits!");
            Destroy(this);
        }
    }
    public void showRanking()
    {
        ClientSend.Ranking();
    }
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ClientSend.checkGameinProgressReceived();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
