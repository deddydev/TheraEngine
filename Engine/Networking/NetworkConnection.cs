using Ayx.BitIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheraEngine.Core;
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
        public void InitializeLocalConnection()
        {
            _udpConnection = new UdpClient();
            _udpConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpConnection.Client.Bind(new IPEndPoint(IPAddress.Any, LocalPort));
            _udpConnection.Connect("localhost", RemotePort);

            ReceivePackets();
            SendPackets();
        }
        public void InitializeConnection(int localPort, IPEndPoint targetPoint)
        {
            IPAddress localAddr = GetLocalIPAddressV4();

            _udpConnection = new UdpClient();
            _udpConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpConnection.Client.Bind(new IPEndPoint(localAddr, localPort));

            if (targetPoint != null)
                _udpConnection.Connect(targetPoint);

            ReceivePackets();
            SendPackets();
        }
        private bool _isReceiving = false;
        private bool _isSending = false;
        private void ReceivePackets()
        {
            if (_isReceiving)
                return;
            Task.Run(async () =>
            {
                _isReceiving = true;
                while (_udpConnection != null)
                {
                    var packet = await _udpConnection.ReceiveAsync();
                    InterpretPacket(packet.RemoteEndPoint, packet.Buffer);
                }
                _isReceiving = false;
            });
        }
        private void SendPackets()
        {
            if (_isSending)
                return;
            Task.Run(() =>
            {
                _isSending = true;
                TimeSpan span = TimeSpan.FromSeconds(1.0 / 10.0);
                Stopwatch w = Stopwatch.StartNew();
                while (_udpConnection != null)
                {
                    var node = _queuedPackets.First;
                    while (node != null)
                    {
                        var pack = node.Value;
                        _udpConnection.Send(pack.Item1, pack.Item1.Length);
                        node = node.Next;
                    }
                    while (w.Elapsed < span) ;
                    w.Restart();
                }
                _isSending = false;
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
