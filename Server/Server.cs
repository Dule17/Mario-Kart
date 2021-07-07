using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace GameServer
{
    class Server
    {
        public static List<Client> clients;
        public static int Max_Players { get; set; }
        public static int Port { get; set; }
        public   delegate void PacketHander(int fromClient, Packet packet);
        public static Dictionary<int,PacketHander> packetHanders;
        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public static int playersReadyToPLay;
        public static bool gameInProgress;
        public static int[] playersQuit;
        public static SQL_database server_Database;
        public static void Start(int maxPlayers,string ip,int port)
        {
            server_Database = new SQL_database();
            clients = new List<Client>();
            playersReadyToPLay = 0;
            gameInProgress =false;
            playersQuit = new int[maxPlayers];
            for (int i = 0; i < maxPlayers; i++) playersQuit[i] = 0;
            packetHanders = new Dictionary<int, PacketHander>()
            {
                 { (int)ClientPackets.welcomeReceived,ServerHandel.WelcomeReceived },
                 { (int)ClientPackets.udpTestReceived,ServerHandel.UDPTestReceived },
                 { (int)ClientPackets.publicChatReceived,ServerHandel.publicChatReceived }, 
                 { (int)ClientPackets.spawnPlayerReceived,ServerHandel.spawnPlayerReceived },
                 { (int)ClientPackets.playerMovementReceived,ServerHandel.playerMovementReceived },
                 { (int)ClientPackets.placeItemReceived,ServerHandel.placeItemReceived },
                 { (int)ClientPackets.destroyNPCReceived,ServerHandel.destroyNPCReceived },
                 { (int)ClientPackets.privateChatReceived,ServerHandel.privateChatReceived },
                 { (int)ClientPackets.startGameReceived,ServerHandel.startGameReceived },
                 { (int)ClientPackets.endGameReceived,ServerHandel.endGameReceived },
                 { (int)ClientPackets.checkGameinProgressReceived,ServerHandel.checkGameinProgressReceived },
                 { (int)ClientPackets.LoginandRegisterReceived,ServerHandel.LoginandRegisterReceived },
                 { (int)ClientPackets.rankingReceived,ServerHandel.rankingReceived }

            };
            Max_Players = maxPlayers;
            Port = port;
            for (int i = 0; i < Max_Players; i++) clients.Add(new Client(i));
            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine("Starting GameServer...");
            tcpListener = new TcpListener(IPAddress.Parse(ip), Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine("GameServer started on Port:"+Port);
        }

        public static void checkQuit()
        {
            int br = 0;
            for (int i = 0; i < Max_Players; i++)
                if (playersQuit[i] > 0) br++;

            if (br == Max_Players)
            {
                for (int i = 0; i < Max_Players; i++) playersQuit[i] = 0;
                playersReadyToPLay = 0;
                gameInProgress = false;
            }
        }
        private static void UDPReceiveCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(ar, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4) return;

                using (Packet packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();
                    if (clients[clientId].udp.endPoint == null)
                    {
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString()) clients[clientId].udp.HandleData(packet);


                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void SendUDPData(IPEndPoint clientEndPoint,Packet packet)
        {
            try
            {
                if(clientEndPoint!=null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(),clientEndPoint, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            for(int i=0;i<Max_Players;i++)
            {
                if(clients[i].tcp.Socket==null)
                {
                    Console.WriteLine("Client:" + client.Client.RemoteEndPoint);
                    clients[i].tcp.Connect(client);
                    return;
                }
            }
        }
    }
}
