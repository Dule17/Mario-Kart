using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerSend
    {

        static void SendTCPData(int tcpClient,Packet packet)
        {
            packet.WriteLength();
            Server.clients[tcpClient].tcp.SendData(packet);
        }
        static void SendUDPData(int tcpClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[tcpClient].udp.SendData(packet);
        }
        static void SendTCPDataToAlll(Packet packet)
        {
            packet.WriteLength();
            for(int i=0;i<Server.clients.Count;i++) Server.clients[i].tcp.SendData(packet);
        }
        static void SendUDPDataToAllExeptOne(int id,Packet packet)
        {
            packet.WriteLength();
            for (int i = 0; i < Server.clients.Count; i++)if(Server.clients[i].id!=id) Server.clients[i].udp.SendData(packet);
        }
        static void SendTCPDataToAllExeptOne(int id,Packet packet)
        {
            packet.WriteLength();
            for (int i = 0; i < Server.clients.Count; i++) if (Server.clients[i].id != id) Server.clients[i].tcp.SendData(packet);
        }
        static void SendUDPDataToAlll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 0; i < Server.clients.Count; i++) Server.clients[i].udp.SendData(packet);
        }
        public static void Welcome(int tcpClient, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(tcpClient);
                SendTCPData(tcpClient, packet);
            }
        }

        public static void destroyNPC(int id)
        {
            using (Packet packet = new Packet((int)ServerPackets.destroyNPC))
            {
                packet.Write(id);

                SendTCPDataToAllExeptOne(id, packet);
            }
        }

        public static void placeItem(int tcpClient, int itemID)
        {
            using (Packet packet = new Packet((int)ServerPackets.placeItem))
            {
                packet.Write(tcpClient);
                packet.Write(itemID);
                SendTCPDataToAlll(packet);
            }
        }

        public static void startGame()
        {
            using (Packet packet = new Packet((int)ServerPackets.startGame))
            {
                packet.Write("Start");

                SendTCPDataToAlll(packet);
            }
        }
        public static void LoginandRegister(int id,string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.LoginandRegister))
            {
                packet.Write(msg);

                SendTCPData(id, packet);
            }
        }

        public static void Ranking(int id,string s)
        {
            using (Packet packet = new Packet((int)ServerPackets.ranking))
            {
                packet.Write(s);

                SendTCPData(id, packet);
            }
        }

        public static void UDPTest(int udpClient)
        {
            using (Packet packet = new Packet((int)ServerPackets.udpTest))
            {
                packet.Write("Test");
               
                SendTCPData(udpClient, packet);
            }
        }

        public static void checkGameinProgress(int id)
        {
            using (Packet packet = new Packet((int)ServerPackets.checkGameinProgress))
            {
                if (Server.gameInProgress == false) packet.Write("false");
                else packet.Write("true");
                    SendTCPData(id, packet);
            }
        }

        public static void publicChat(int id,string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.publicChat))
            {
                packet.Write(id);
                packet.Write(msg);
                SendUDPDataToAllExeptOne(id,packet);
            }
        }
        public static void privateChat(int id, string msg, string username)
        {
            using (Packet packet = new Packet((int)ServerPackets.privateChat))
            {
                packet.Write(id);
                packet.Write(msg);
                for (int i = 0; i < Server.clients.Count; i++)
                    {
                        if (Server.clients[i].Username != null && Server.clients[i].Username == username)
                        {
                            SendUDPData(Server.clients[i].id, packet);
                            return;
                        }
                    }

                
            }
        }
        public static void playerMovement(int fromClient, Vector3 v,Quaternion q, Vector3 velocity)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerMovement))
            {
                packet.Write(fromClient);
                packet.Write(v);
                packet.Write(q);
                packet.Write(velocity);
                SendUDPDataToAllExeptOne(fromClient, packet);
            }
        }

        public static void SpawnPlayer(int id,int idV, Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(idV);
                packet.Write(player.username);
                packet.Write(player.position);
                packet.Write(player.rotation);

                SendTCPData(id, packet);
            }
        }
    }
}
