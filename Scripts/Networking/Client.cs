using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEditor;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string ip;
    public int port = 26564;
    public int myId = 0;
    public string myUsername="";
    public TCP tcp;
    public UDP udp;
    public int selectedCharater;
    bool isConnected = false;
    delegate void PacketHandler(Packet packet);
    static Dictionary<int, PacketHandler> packetHandlers;
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
    public void OnApllicationQuit()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        if(isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
        }
    }

   public void SetTCPandUDP()
    {
        //AssetDatabase.Refresh();
        tcp = new TCP();
        udp = new UDP();
    }

    public void connectToServer()
    {
        InitializeClientData();
        isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        public NetworkStream stream;
        public Packet receivePacketData;
        public byte[] receiveBuffer;
        public byte[] sendDataBuffer;

        public void Disconnect()
        {
            instance.Disconnect();
            receiveBuffer = null;
            stream = null;
            socket = null;
            receivePacketData = null;
        }
        public void Connect()
        {
            socket = new TcpClient();
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            if (!socket.Connected)
            {
                return;
            }
            stream = socket.GetStream();
            receivePacketData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int byteLength = stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivePacketData.Reset(HandelData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
                Disconnect();
            }
        }

        private bool HandelData(byte[] data)
        {
            int packetLength = 0;
            receivePacketData.SetBytes(data);
            if (receivePacketData.UnreadLength() >= 4)
            {
                packetLength = receivePacketData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivePacketData.UnreadLength())
            {
                byte[] packetBytes = receivePacketData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);
                    }
                });


                packetLength = 0;

                if (receivePacketData.UnreadLength() >= 4)
                {
                    packetLength = receivePacketData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Disconnect()
        {
            instance.Disconnect();
            endPoint = null;
            socket = null;
        }
        public void Connect(int localport)
        {
            socket = new UdpClient(localport);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallBack, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] data = socket.EndReceive(ar, ref endPoint);
                socket.BeginReceive(ReceiveCallBack, null);
                if (data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet);
                }
            });
        }
    }

    void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome,ClientHandler.Welcome },
            { (int)ServerPackets.udpTest,ClientHandler.UDPTest },
            { (int)ServerPackets.publicChat,ClientHandler.publicChat },
            { (int)ServerPackets.spawnPlayer,ClientHandler.SpawnPlayer },
            { (int)ServerPackets.playerMovement,ClientHandler.playerMovement},
            { (int)ServerPackets.placeItem,ClientHandler.placeItem},
             { (int)ServerPackets.destroyNPC,ClientHandler.destroyNPC},
            { (int)ServerPackets.privateChat,ClientHandler.privateChat},
             { (int)ServerPackets.startGame,ClientHandler.startGame},
             { (int)ServerPackets.checkGameinProgress,ClientHandler.checkGameinProgress},
               { (int)ServerPackets.LoginandRegister,ClientHandler.LoginandRegister},
                { (int)ServerPackets.ranking,ClientHandler.ranking}
        };
    }
}
