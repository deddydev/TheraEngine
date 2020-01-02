using Ayx.BitIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
                    Engine.PrintLine($"Received connection request from {endPoint.ToString()}.");
                    NetworkClient client = FindConnectedClient(endPoint);
                    if (client is null)
                    {
                        client = new NetworkClient(endPoint);
                        int index = AddClient(client);
                        TPacketConnectionAccepted response = new TPacketConnectionAccepted();
                        response.Header.Header.PacketType = EPacketType.Connection;
                        response.Header.ConnectionMessage = EConnectionMessage.Accepted;
                        response.ServerIndex = (byte)index;
                        Engine.PrintLine($"Accepted request. Server index = {index.ToString()}.");
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
        public override void UpdatePacketQueue(float delta)
        {
            foreach (NetworkClient c in ConnectedClients.Values)
                c.UpdatePacketQueue(delta);
        }
    }
    public class NetworkClient
    {
        public NetworkClient(IPEndPoint endPoint)
        {
            EndPoint = endPoint;

            IPAddress localAddr = NetworkConnection.GetLocalIPAddressV4();
            _outputConnection = new UdpClient();
            _outputConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _outputConnection.Client.Bind(new IPEndPoint(localAddr, NetworkConnection.ServerPort));
            _outputConnection.Connect(EndPoint);
        }

        private UdpClient _outputConnection;
        protected LinkedList<(byte[], float, Func<bool>)> _queuedPackets
            = new LinkedList<(byte[], float, Func<bool>)>();

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
        public bool IsSending { get; private set; } = false;

        public void UpdatePacketQueue(float delta)
        {
            var node = _queuedPackets.First;
            while (node != null)
            {
                var pack = node.Value;

                pack.Item2 -= delta;
                bool stopWhen = false;
                if (pack.Item3 != null)
                    stopWhen = pack.Item3();
                if (pack.Item2 <= 0.0f || stopWhen)
                {
                    var temp = node.Next;
                    _queuedPackets.Remove(node);
                    node = temp;
                }
                else
                    node = node.Next;
            }
        }
        public void SendPackets()
        {
            if (IsSending)
                return;
            Task.Run(() =>
            {
                IsSending = true;
                TimeSpan span = TimeSpan.FromSeconds(1.0 / 10.0);
                Stopwatch w = Stopwatch.StartNew();
                while (_outputConnection != null)
                {
                    var node = _queuedPackets.First;
                    while (node != null)
                    {
                        var pack = node.Value;
                        _outputConnection.Send(pack.Item1, pack.Item1.Length);
                        node = node.Next;
                    }
                    while (w.Elapsed < span) ;
                    w.Restart();
                }
                IsSending = false;
            });
        }

    }
}
