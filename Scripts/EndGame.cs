using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndGame : MonoBehaviour
{
    public void MainMenu()
    {
        ClientSend.destroyNPCReceived();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
