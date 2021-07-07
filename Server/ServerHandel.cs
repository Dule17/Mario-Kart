using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandel
    {
        public static void WelcomeReceived(int fromClient,Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();
            string password = packet.ReadString();
            Server.clients[id].Username = username;
            Console.WriteLine("player username:" + username + " Password:"+password);
            Console.WriteLine("" + Server.clients[id].tcp.Socket.Client.RemoteEndPoint + " is now a player:" + fromClient);
            if(id!=fromClient)
            {
                Console.WriteLine("player username:"+username+" wrong id");
            }
            
        }

        public static void UDPTestReceived(int fromClient, Packet packet)
        {
            string msg = packet.ReadString();
            Console.WriteLine("from player:" + fromClient + "  " + msg);
        }

        public static void publicChatReceived(int fromClient, Packet packet)
        {
            string msg = packet.ReadString();
            ServerSend.publicChat(fromClient,msg);
        }
        public static void privateChatReceived(int fromClient, Packet packet)
        {
            string msg = packet.ReadString();
            string username = packet.ReadString();
            ServerSend.privateChat(fromClient, msg,username);
        }
        public static void spawnPlayerReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            int idv = packet.ReadInt();
            string username = packet.ReadString();
           if(Server.gameInProgress==false) Server.clients[fromClient].SendIntoGame(idv,username);
        }

        public static void endGameReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            int place = packet.ReadInt();
            float fastestLap = packet.ReadFloat();
            float fastestGame = packet.ReadFloat();
            int wins = 0;
            int loses = 0;
            if (place == 1) wins = 1;
            else loses = 1;
            Server.server_Database.UPDATE(Server.clients[fromClient].database_ID, wins, loses, fastestLap, fastestGame);
            Server.playersQuit[fromClient]++;
            Server.checkQuit();
            /*Server.gameInProgress = false;
            Server.playersReadyToPLay = 0;*/
        }

        public static void rankingReceived(int fromClient, Packet packet)
        {
            string s = Server.server_Database.RANKING();
            ServerSend.Ranking(fromClient,s);
        }

        public static void startGameReceived(int fromClient, Packet packet)
        {
            Console.WriteLine(Server.clients[fromClient].tcp.Socket.Client.RemoteEndPoint + " is now ready to play");
            if (Server.playersQuit[fromClient] > 0) return;
            Server.playersReadyToPLay++;
            if (Server.playersReadyToPLay == Server.Max_Players)
            {
                Server.gameInProgress = true;
                ServerSend.startGame();
            }
        }

        public static void LoginandRegisterReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();
            string password = packet.ReadString();
            Server.clients[id].Username = username;
            Console.WriteLine("player username:" + username + " Password:" + password);
            Console.WriteLine("" + Server.clients[id].tcp.Socket.Client.RemoteEndPoint + " is now a player:" + fromClient);
            if (id != fromClient)
            {
                Console.WriteLine("player username:" + username + " wrong id");
            }
            string type = packet.ReadString();
            if(type=="Register")
            {
                Server.server_Database.INSERT(username, password);
            }
           int ind= Server.server_Database.Login(username, password);
           string msg;
            if (ind == -1) msg = "Failed";
            else
            {
                msg = "Sucesful";
                if (Server.gameInProgress == false) Server.playersQuit[fromClient] = 0;
            }
            Server.clients[fromClient].database_ID = ind;
            ServerSend.LoginandRegister(id, msg);
        }

        public static void checkGameinProgressReceived(int fromClient, Packet packet)
        {
             ServerSend.checkGameinProgress(fromClient);
        }

        public static void destroyNPCReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            Server.playersQuit[id]++;
            Server.checkQuit();
            Server.clients[id].player = null;
            ServerSend.destroyNPC(id);
        }

        public static void placeItemReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            int itemID = packet.ReadInt();
            ServerSend.placeItem(fromClient,itemID);
        }

        public static void playerMovementReceived(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 v = packet.ReadVector();
            Quaternion q = packet.ReadQuaternion();
            Vector3 velocity = packet.ReadVector();
            Server.clients[id].player.position = v;
            Server.clients[id].player.rotation = q;
            // Console.WriteLine("" + v.X + "," + v.Y + "," + v.Z);
            ServerSend.playerMovement(id,v,q,velocity);
        }
    }
}
