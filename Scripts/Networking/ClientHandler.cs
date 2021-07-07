using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();
        Client.instance.myId = id;
        Debug.Log("Message from server:" + msg);
       // ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        ClientSend.LoginandRegister();
       // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public static void UDPTest(Packet packet)
    {
        string msg = packet.ReadString();

        Debug.Log("Message from server:" + msg);
        ClientSend.UDPTestReceived();

    }

    public static void publicChat(Packet packet)
    {
        int id = packet.ReadInt();
        string msg = packet.ReadString();
        ChatHandler.instance.displayMessage(id, msg,"ALL");
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        int idV = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector();
        Quaternion rotation = packet.ReadQuaternion();
        GameManager.instance.SpawnPlayers(id,idV, username, position, rotation);
    }

    public static void playerMovement(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector();
        Quaternion rotation = packet.ReadQuaternion();
        Vector3 velocity = packet.ReadVector();
        GameObject[] g = GameObject.FindGameObjectsWithTag("NPC");
        for(int i=0;i<g.Length;i++)
        {
            if(g[i].GetComponent<PlayerManager>().id==id)
            {
                g[i].transform.position =Vector3.Lerp(g[i].transform.position, position,0.2f);
                g[i].transform.rotation =Quaternion.Lerp(g[i].transform.rotation, rotation,0.2f);
                g[i].GetComponent<Rigidbody>().velocity = Vector3.Lerp(g[i].GetComponent<Rigidbody>().velocity, velocity, 0.2f);
            }
        }
    }

    public static void placeItem(Packet packet)
    {
        int id = packet.ReadInt();
        int itemID = packet.ReadInt();
        GameItemHandle gih= GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameItemHandle>();
        GameObject[] g;
        if (id != Client.instance.myId) g = GameObject.FindGameObjectsWithTag("NPC");
        else g = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0;i<g.Length;i++)if(g[i].GetComponent<PlayerManager>().id==id)
            {
                GameObject item=gih.items[itemID].prefab;
                g[i].GetComponent<CheckPointCounter>().CanPickup = true;
                if (item.name == "Banana")
                {
                    for (int j = 0; j < g[i].transform.childCount; j++)
                    {
                        Transform child = g[i].transform.GetChild(j);
                        if(child.tag=="ItemBack") item.transform.position = child.transform.position;
                    }
                    Instantiate(item);
                }
                else if (item.name == "Bullet")
                {
                    
                    for (int j = 0; j < g[i].transform.childCount; j++)
                    {
                        GameObject bullet;
                        Transform child = g[i].transform.GetChild(j);
                        if (child.tag == "ItemFront")
                        {
                            bullet = Instantiate(item, child.transform.position, g[i].transform.rotation);
                            bullet.transform.Rotate(90, 0, 0);
                            bullet.GetComponent<Rigidbody>().AddForce(g[i].transform.forward * 1000f);
                            Destroy(bullet, 10f);
                            return;
                        }
                    }
                   

                }
                break;
            }
    }

    public static void destroyNPC(Packet packet)
    {
        int id = packet.ReadInt();
        GameObject[] g = GameObject.FindGameObjectsWithTag("NPC");

        for (int i = 0; i < GameController.players.Count; i++)
            if (GameController.players[i].GetComponent<PlayerManager>().id == id)
            {
                GameController.players.RemoveAt(i);
                break;
            }

        for (int i = 0; i < g.Length; i++)
        {
            if (g[i].GetComponent<PlayerManager>().id == id)
            {
                DestroyObject(g[i]);
                return;
            }
        }
    }

    public static void privateChat(Packet packet)
    {
        int id = packet.ReadInt();
        string msg=packet.ReadString();
        ChatHandler.instance.displayMessage(id, msg, "PRIVATE");
    }

    public static void startGame(Packet packet)
    {
        Debug.Log("Ready");
        GameManager.startGame = true;
    }

    public static void checkGameinProgress(Packet packet)
    {
        string msg = packet.ReadString();
        if (msg == "false") SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else GameObject.FindGameObjectsWithTag("error")[0].GetComponent<TMPro.TextMeshProUGUI>().text = "Game in progress";
    }

    public static void LoginandRegister(Packet packet)
    {
        string msg = packet.ReadString();
        if (msg == "Sucesful") SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
        {
            UI_Manager.instance.txtMessage.text = "Wrong username or password";
            Client.instance.Disconnect();
            Client.instance.SetTCPandUDP();
        }
    }

    public static void ranking(Packet packet)
    {
        string msg = packet.ReadString();
        MainMenu.instance.Ranking.text = msg;
    }
}
