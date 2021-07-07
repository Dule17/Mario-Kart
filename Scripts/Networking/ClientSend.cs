using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }
    static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets
    public static void  WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(UI_Manager.instance.UsernameInput.text);
            Client.instance.myUsername = UI_Manager.instance.UsernameInput.text;
            packet.Write(UI_Manager.instance.passwordInput.text);
            SendTCPData(packet);
        }
    }
    public static void LoginandRegister()
    {
        using (Packet packet = new Packet((int)ClientPackets.LoginandRegisterReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(UI_Manager.instance.UsernameInput.text);
            Client.instance.myUsername = UI_Manager.instance.UsernameInput.text;
            packet.Write(UI_Manager.instance.passwordInput.text);
            packet.Write(UI_Manager.instance.ButtonType);
            SendTCPData(packet);
        }
    }

    public static void Ranking()
    {
        using (Packet packet = new Packet((int)ClientPackets.rankingReceived))
        {
            packet.Write("Ranking");
            SendTCPData(packet);
        }
    }
    public static void UDPTestReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            packet.Write("Received UDP packet");
            SendUDPData(packet);
        }
    }
    public static void publicChatReceived(string msg)
    {
        using (Packet packet = new Packet((int)ClientPackets.publicChatReceived))
        {
            packet.Write(msg);
            SendUDPData(packet);
        }
    }
    public static void checkGameinProgressReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.checkGameinProgressReceived))
        {
            SendTCPData(packet);
        }
    }
    public static void privateChatReceived(string msg, string chatType)
    {
        using (Packet packet = new Packet((int)ClientPackets.privateChatReceived))
        {
            packet.Write(msg);
            packet.Write(chatType);
            SendUDPData(packet);
        }
    }
    public static void SpawnPlayerReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.spawnPlayerReceived))
        {
            packet.Write(Client.instance.myId);
            //packet.Write(PlayerPrefs.GetInt("Selected_index"));
            packet.Write(Client.instance.selectedCharater);
            packet.Write(Client.instance.myUsername);

            SendTCPData(packet);
        }
    }

    public static void playerMovementReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovementReceived))
        {
            packet.Write(Client.instance.myId);
            GameObject g = GameObject.FindGameObjectsWithTag("Player")[0];
            packet.Write(g.gameObject.transform.position);
            packet.Write(g.gameObject.transform.rotation);
            packet.Write(g.gameObject.GetComponent<Rigidbody>().velocity);
            SendUDPData(packet);
        }
    }

    public static void placeItemReceived(int itemID)
    {
        using (Packet packet = new Packet((int)ClientPackets.placeItemReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(itemID);
            SendTCPData(packet);
        }
    }
    public static void destroyNPCReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.destroyNPCReceived))
        {
            packet.Write(Client.instance.myId);
            SendTCPData(packet);
        }
    }

    public static void endGameReceived(int place,float minLap,float gameTime)
    {
        using (Packet packet = new Packet((int)ClientPackets.endGameReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(place);
            packet.Write(minLap);
            packet.Write(gameTime);
            SendTCPData(packet);
        }
    }


    public static void startGameReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.startGameReceived))
        {
            packet.Write("Ready");
            SendTCPData(packet);
        }
    }
    #endregion
}
