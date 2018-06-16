using Ayx.BitIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public const long LocalHostIp = 0x7F000001;

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
        //public void InitializeLocalConnection()
        //{
        //    _connection = new UdpClient();
        //    _connection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //    _connection.Client.Bind(new IPEndPoint(IPAddress.Any, LocalPort));
        //    _connection.Connect("localhost", RemotePort);

        //    ReceivePackets();
        //    SendPackets();
        //}
        public virtual void InitializeConnection(int localPort, IPEndPoint endPoint)
        {
            IPAddress localAddr = GetLocalIPAddressV4();

            _connection = new UdpClient();
            _connection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _connection.Client.Bind(new IPEndPoint(localAddr, localPort));

            if (endPoint != null)
                _connection.Connect(endPoint);

            ReceivePackets();
            //SendPackets();
        }
        protected UdpClient _connection;
        private bool _isReceiving = false;
        protected void ReceivePackets()
        {
            if (_isReceiving)
                return;
            Task.Run(async () =>
            {
                _isReceiving = true;
                while (_connection != null)
                {
                    var packet = await _connection.ReceiveAsync();
                    InterpretPacket(packet.RemoteEndPoint, packet.Buffer);
                }
                _isReceiving = false;
            });
        }
        
        //private ConcurrentDictionary<IPEndPoint, List<byte[]>> _packetCacheRecieving 
        //    = new ConcurrentDictionary<IPEndPoint, List<byte[]>>();
        //private ConcurrentDictionary<IPEndPoint, List<byte[]>> _packetCacheReading
        //    = new ConcurrentDictionary<IPEndPoint, List<byte[]>>();
        private void InterpretPacket(IPEndPoint remoteEndPoint, byte[] buffer)
        {
            BitReader reader = new BitReader(buffer);

            int packetType = reader.ReadByte();
            if (MessageTypeFuncs.IndexInArrayRange(packetType))
                MessageTypeFuncs[packetType](remoteEndPoint, reader);

            //_packetCacheRecieving.AddOrUpdate(
            //    remoteEndPoint,
            //    m => { return new List<byte[]>() { buffer }; },
            //    (r, x) => { x.Add(buffer); return x; });
        }
        #endregion

        #region Recieving
        /// <summary>
        /// Retrieves all packets that have arrived and need to be processed.
        /// </summary>
        public void RecievePackets()
        {
            //_packetCacheReading.Clear();
            //THelpers.Swap(ref _packetCacheRecieving, ref _packetCacheReading);
            //foreach (var packet in _packetCacheReading)
            //{
            //    foreach (var buffer in packet.Value)
            //    {
            //        BitReader reader = new BitReader(buffer);

            //        int packetType = reader.ReadByte();
            //        if (MessageTypeFuncs.IndexInArrayRange(packetType))
            //            MessageTypeFuncs[packetType](packet.Key, reader);
            //    }
            //}
        }
        public abstract void UpdatePacketQueue(float delta);
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
        public void SendPacket<T>(T data, float timeout = 2.0f, Func<bool> stopWhen = null) where T : unmanaged
            => SendPacket(data.ToByteArray(), timeout, stopWhen);
        public void SendPacket(byte[] data, float timeout = 2.0f, Func<bool> stopWhen = null)
        {
            //_queuedPackets.AddLast((data, timeout, stopWhen));
        }
        #endregion
    }
}
