using Ayx.BitIO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace TheraEngine.Networking
{
    public class Server : NetworkConnection
    {
        public override bool IsServer => true;
        public override int LocalPort => ServerPort;
        public override int RemotePort => ClientPort;

        public int MaxConnectedClients { get; set; } = 1;
        public bool CanConnectNewClient() => ConnectedClients.Count < MaxConnectedClients;
        public ConcurrentDictionary<string, NetworkClient> ConnectedClients { get; } = new ConcurrentDictionary<string, NetworkClient>();

        public NetworkClient FindConnectedClient(IPEndPoint endPoint)
        {
            string addr = endPoint.ToString();
            if (ConnectedClients.ContainsKey(addr))
                return ConnectedClients[addr];
            return null;
        }
        public int AddClient(NetworkClient client)
        {
            string addr = client.EndPoint.ToString();
            if (ConnectedClients.ContainsKey(addr))
                ConnectedClients[addr] = client;
            else
                ConnectedClients.TryAdd(addr, client);
            return ConnectedClients.Count - 1;
        }
        
        public override void ReadConnectionPacket(IPEndPoint endPoint, BitReader reader)
        {
            EConnectionMessage message = (EConnectionMessage)reader.ReadByte();
            switch (message)
            {
                case EConnectionMessage.Request:
                    Engine.PrintLine("Recieved connection request.");
                    NetworkClient client = FindConnectedClient(endPoint);
                    if (client == null)
                    {
                        client = new NetworkClient(endPoint);
                        int index = AddClient(client);
                        TPacketConnectionAccepted response = new TPacketConnectionAccepted();
                        response.Header.Header.PacketType = EPacketType.Connection;
                        response.Header.ConnectionMessage = EConnectionMessage.Accepted;
                        response.ServerIndex = (byte)index;
                        Engine.PrintLine("Accepted request.");

                        _udpConnection.Connect(endPoint);
                        SendPacket(response, 1.0f);
                    }
                    else
                    {
                        TPacketConnection response = new TPacketConnection();
                        response.Header.PacketType = EPacketType.Connection;
                        response.ConnectionMessage = EConnectionMessage.Denied;
                        Engine.PrintLine("Denied request.");
                        SendPacket(response, 5.0f);
                    }
                    break;
                case EConnectionMessage.Denied:
                case EConnectionMessage.Accepted:
                    //Not a client->server message
                    break;
                case EConnectionMessage.LocalPlayerCountChanged:
                    int clientLocalPlayers = reader.ReadByte();
                    
                    break;
            }
        }
    }
    public class NetworkClient
    {
        public NetworkClient(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; }
        /// <summary>
        /// How many seconds can pass without recieving any packets from the client
        /// before considering it disconnected.
        /// </summary>
        public float SecondsToDisconnect { get; set; }
        /// <summary>
        /// How many seconds have passed since the last recieved packet from this client.
        /// </summary>
        public float SecondsSinceLastPacket { get; internal set; }
    }
}
