using Ayx.BitIO;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System.Collections.Generic;

namespace TheraEngine.Networking
{
    public class Server : NetworkConnection
    {
        public override bool IsServer => true;

        public int MaxConnectedClients { get; set; } = 1;
        public bool CanConnectNewClient() => ConnectedClients.Count < MaxConnectedClients;
        public Dictionary<string, NetworkClient> ConnectedClients { get; } = new Dictionary<string, NetworkClient>();

        public NetworkClient FindConnectedClient(IpV4Address ip, ushort port)
        {
            string addr = ip + ":" + port.ToString();
            if (ConnectedClients.ContainsKey(addr))
                return ConnectedClients[addr];
            return null;
        }

        public override void ReadConnectionPacket(Packet packet, BitReader reader)
        {
            IpV4Datagram ip = packet.IpV4.IpV4;
            UdpDatagram udp = ip.Udp;

            EConnectionMessage message = (EConnectionMessage)reader.ReadByte();
            switch (message)
            {
                case EConnectionMessage.Request:
                    NetworkClient client = FindConnectedClient(ip.Source, udp.SourcePort);
                    if (client == null)
                    {

                    }
                    else
                    {

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
