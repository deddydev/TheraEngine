using Ayx.BitIO;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Networking
{
    public class Client : NetworkConnection
    {
        public override bool IsServer => false;

        public int ServerIndex { get; set; }

        private bool _connectionResponseRecieved = false;
        private bool _connectionAccepted = false;

        /// <summary>
        /// Attempts to connect to the server.
        /// </summary>
        /// <param name="timeout">How many seconds to try connecting before giving up.</param>
        /// <param name="packetsPerSecond">How many packets to send each second.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        public bool RequestConnection(double timeout = 5.0, int packetsPerSecond = 10)
        {
            _connectionResponseRecieved = false;
            _connectionAccepted = false;
            
            int millisecondsPerPacket = 1000 / packetsPerSecond;

            TPacketConnection conn = new TPacketConnection();
            conn.Header.PacketType = EPacketType.Connection;
            conn.ConnectionMessage = EConnectionMessage.Request;

            TimeSpan timeoutSpan = TimeSpan.FromSeconds(timeout);
            Stopwatch watch = Stopwatch.StartNew();
            while (!_connectionResponseRecieved && watch.Elapsed < timeoutSpan)
            {
                SendPacket(conn);
                Thread.Sleep(millisecondsPerPacket);
            }
            watch.Stop();
            return _connectionAccepted;
        }
        public Task<bool> RequestConnectionAsync(double timeout = 5.0, int packetsPerSecond = 10)
            => Task.Run(() => RequestConnection(timeout, packetsPerSecond));
        public void CancelConnectionRequest()
        {
            _connectionResponseRecieved = true;
        }

        public override void ReadConnectionPacket(Packet packet, BitReader reader)
        {
            IpV4Datagram ip = packet.IpV4.IpV4;
            UdpDatagram udp = ip.Udp;

            EConnectionMessage message = (EConnectionMessage)reader.ReadByte();
            switch (message)
            {
                case EConnectionMessage.Request:
                    //Not a server->client message
                    break;
                case EConnectionMessage.Denied:
                    //Not enough spots on server.
                    //ConnectionDenied?.Invoke();
                    _connectionAccepted = false;
                    _connectionResponseRecieved = true;
                    break;
                case EConnectionMessage.Accepted:
                    //The server successfully added this client to the list.
                    ServerIndex = reader.ReadByte();
                    //ConnectionAccepted?.Invoke();
                    _connectionAccepted = true;
                    _connectionResponseRecieved = true;
                    break;
                case EConnectionMessage.LocalPlayerCountChanged:
                    
                    //This is the index of the first local player in the server player list.
                    //All other local players are directly after.
                    int serverLocalPlayerStartIndex = reader.ReadByte();
                    
                    break;
            }
        }
    }
}
