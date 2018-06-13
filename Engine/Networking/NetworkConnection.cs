using Ayx.BitIO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheraEngine.Core.Extensions;

namespace TheraEngine.Networking
{
    public abstract class NetworkConnection
    {
        public const int ServerPort = 9000;
        public const int ClientPort = 7000;

        public static bool AnyConnectionsAvailable => NetworkInterface.GetIsNetworkAvailable();
        
        public abstract bool IsServer { get; }
        public abstract int LocalPort { get; }
        public abstract int RemotePort { get; }

        public static IPAddress GetLocalIPAddressV4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            throw new Exception("Local v4 IP address not found.");
        }
        public static IPAddress GetLocalIPAddressV6()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    return ip;
            throw new Exception("Local v6 IP address not found.");
        }
        
        protected UdpClient _udpConnection;
        protected LinkedList<(byte[], float, Func<bool>)> _queuedPackets 
            = new LinkedList<(byte[], float, Func<bool>)>();

        #region Initialization
        protected delegate void DelReadBits(IPEndPoint endPoint, BitReader reader);
        protected DelReadBits[] MessageTypeFuncs;
        public NetworkConnection()
        {
            MessageTypeFuncs = new DelReadBits[]
            {
                ReadDataPacket,
                ReadInputPacket,
                ReadConnectionPacket,
                ReadStatePacket,
            };
        }
        public void InitializeConnection()
        {
            _udpConnection = new UdpClient();
            _udpConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpConnection.Client.Bind(new IPEndPoint(IPAddress.Any, LocalPort));
            _udpConnection.Connect("localhost", RemotePort);
        }
        #endregion

        #region Recieving
        /// <summary>
        /// Retrieves all packets that have arrived and need to be processed.
        /// </summary>
        public async void RecievePackets()
        {
            try
            {
                var result = await _udpConnection.ReceiveAsync();
                byte[] buffer = result.Buffer;

                BitReader reader = new BitReader(buffer);

                int packetType = reader.ReadByte();
                if (MessageTypeFuncs.IndexInArrayRange(packetType))
                    MessageTypeFuncs[packetType](result.RemoteEndPoint, reader);
            }
            catch (SocketException ex)
            {
                Engine.PrintLine(ex.ToString());
            }
        }
        public void SendPackets(float delta)
        {
            var node = _queuedPackets.First;
            while (node != null)
            {
                var pack = node.Value;
                _udpConnection.Send(pack.Item1, pack.Item1.Length);
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
        private void PacketRecieved(Task<UdpReceiveResult> result)
        {
            byte[] buffer = result.Result.Buffer;

            BitReader reader = new BitReader(buffer);

            int packetType = reader.ReadByte();
            if (MessageTypeFuncs.IndexInArrayRange(packetType))
                MessageTypeFuncs[packetType](result.Result.RemoteEndPoint, reader);
        }
        public void ReadDataPacket(IPEndPoint endPoint, BitReader reader)
        {

        }
        public void ReadInputPacket(IPEndPoint endPoint, BitReader reader)
        {

        }
        public abstract void ReadConnectionPacket(IPEndPoint endPoint, BitReader reader);
        public void ReadStatePacket(IPEndPoint endPoint, BitReader reader)
        {
            
        }
        #endregion

        #region Sending
        public unsafe void SendPacket<T>(T data, float timeout = 2.0f, Func<bool> stopWhen = null) where T : unmanaged
            => SendPacket(data.ToByteArray(), timeout, stopWhen);
        public unsafe void SendPacket(byte[] data, float timeout = 2.0f, Func<bool> stopWhen = null)
        {
            _queuedPackets.AddLast((data, timeout, stopWhen));
        }
        #endregion
    }
}
