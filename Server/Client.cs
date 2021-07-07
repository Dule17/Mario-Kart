using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace GameServer
{
    class Client
    {
        public string Username;
        public static int dataBufferSize = 4096;
        public TCP tcp;
        public UDP udp;
        public int id;
        public int database_ID = -1;
        public Player player;
        public Client(int idC)
        {
            id = idC;
            tcp = new TCP(idC);
            udp = new UDP(idC);
        }
        public class TCP
        {
            public TcpClient Socket;
            private NetworkStream stream;
            Packet receivePacketData;
            private byte[] receiveBuffer;
            int id;
            public TCP(int _id)
            {
                id = _id;
            }
            public void Connect(TcpClient socket)
            {
                Socket = socket;
                receivePacketData = new Packet();
                Socket.ReceiveBufferSize = dataBufferSize;
                Socket.SendBufferSize = dataBufferSize;
                stream = socket.GetStream();
                receiveBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id,"Welcome to the server.");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:" + e.Message);
                }
            }
            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Server.clients[id].Disconnect();
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
                    Server.clients[id].Disconnect();
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
                            Server.packetHanders[packetId](id, packet);
                        }
                    });
                }

                packetLength = 0;

                if (receivePacketData.UnreadLength() >= 4)
                {
                    packetLength = receivePacketData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
            public void Disconnect()
            {
                Socket.Close();
                stream = null;
                receiveBuffer = null;
                receivePacketData = null;
                Socket = null;
            }
        }


        public class UDP
        {

            public IPEndPoint endPoint;
            public int id;
            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint ipEndPoint)
            {
                endPoint = ipEndPoint;
                ServerSend.UDPTest(id);
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] data = packetData.ReadBytes(packetLength);


                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetId = packet.ReadInt();
                        Server.packetHanders[packetId](id, packet);
                    }
                });
            }
            public void Disconnect()
            {
                endPoint = null;
            }


        }

        public void SendIntoGame(int idV,string PlayerName)
        {
            int[] pos = new int[4];
            pos[0] = 152;
            pos[1] = 149;
            pos[2] = 146;
            pos[3] = 143;
            player = new Player(id, PlayerName, new System.Numerics.Vector3(-10, 14, pos[id]));
            player.selectedCharater = idV;
            foreach (Client client in Server.clients)
            {
                if (client.id != id&&client.player!=null) ServerSend.SpawnPlayer(id,client.player.selectedCharater, client.player);
            }


            foreach (Client client in Server.clients)
            {
                if(client.player != null)ServerSend.SpawnPlayer(client.id, player.selectedCharater, player);
            }
        }

        public void Disconnect()
        {
            Console.WriteLine("" + tcp.Socket.Client.RemoteEndPoint + " has disconeted");
           if(player!=null) ServerSend.destroyNPC(id);
            player = null;
            Server.playersQuit[id]++;
            Server.checkQuit();
            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}

