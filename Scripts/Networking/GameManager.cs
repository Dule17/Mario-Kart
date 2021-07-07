using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] localPlayerPrefab;
    public GameObject[] PlayerPrefab;
    public static bool startGame;
    public void Start()
    {
        ClientSend.SpawnPlayerReceived();
        startGame = false;
    }
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

    public void SpawnPlayers(int id,int idV, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab[idV], position,localPlayerPrefab[idV].gameObject.transform.rotation);
        }
        else
        {
            player = Instantiate(PlayerPrefab[idV], position, PlayerPrefab[idV].gameObject.transform.rotation);
        }
        player.GetComponent<PlayerManager>().id = id;
        player.GetComponent<PlayerManager>().username = username;
        GameController.players.Add(player);
        Debug.Log("Spawn");
    }
}
