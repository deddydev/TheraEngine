using Ayx.BitIO;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using TheraEngine.Core.Extensions;

namespace TheraEngine.Networking
{
    public abstract class NetworkConnection : IDisposable
    {
        public static bool AnyConnectionsAvailable => NetworkInterface.GetIsNetworkAvailable();

        public abstract bool IsServer { get; }

        public static IPAddress GetLocalIPAddressV4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip;
            throw new Exception("Local v4 IP address not found.");
        }
        public static IPAddress GetLocalIPAddressV6()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    return ip;
            throw new Exception("Local v6 IP address not found.");
        }

        PacketCommunicator _comm;

        public ReadOnlyCollection<LivePacketDevice> GetDeviceList() => LivePacketDevice.AllLocalMachine;

        #region Initialization
        protected delegate void DelReadBits(Packet packet, BitReader reader);
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
        /// <summary>
        /// http://www.winpcap.org/docs/docs_40_2/html/group__language.html
        /// </summary>
        /// <param name="filter"></param>
        public void ConnectAuto(string filter = "ip and udp")
        {
            ReadOnlyCollection<LivePacketDevice> allDevices = GetDeviceList();

            if (allDevices.Count == 0)
            {
                Engine.PrintLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }
            
            for (int i = 0; i < allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                string name = (i + 1) + ". " + device.Name;
                if (device.Description != null)
                    Engine.PrintLine(name + " (" + device.Description + ")");
                else
                    Engine.PrintLine(name + " (No description available)");
            }

            int deviceIndex = 1;
            //do
            //{
            //    Console.WriteLine("Enter the interface number (1-" + allDevices.Count + "):");
            //    string deviceIndexString = Console.ReadLine();
            //    if (!int.TryParse(deviceIndexString, out deviceIndex) ||
            //        deviceIndex < 1 || deviceIndex > allDevices.Count)
            //    {
            //        deviceIndex = 0;
            //    }
            //} while (deviceIndex == 0);
            
            Connect(allDevices[deviceIndex - 1], filter);
        }
        /// <summary>
        /// http://www.winpcap.org/docs/docs_40_2/html/group__language.html
        /// </summary>
        /// <param name="device"></param>
        /// <param name="filter"></param>
        public void Connect(LivePacketDevice device, string filter = "ip and udp")
        {
            //65536 guarantees that the whole packet will be captured on all the link layers
            _comm = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
            _comm.NonBlocking = true;

            Engine.PrintLine("Established connection to " + device.Description);

            using (BerkeleyPacketFilter pFilter = _comm.CreateFilter(filter))
                _comm.SetFilter(pFilter);
        }
        #endregion

        #region Recieving
        /// <summary>
        /// Retrieves all packets that have arrived and need to be processed.
        /// </summary>
        public void RecievePackets() => _comm.ReceiveSomePackets(out int retrievedCount, -1, PacketRecieved);
        private void PacketRecieved(Packet packet)
        {
            Engine.PrintLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            IpV4Datagram ip = packet.IpV4.IpV4;
            UdpDatagram udp = ip.Udp;
            
            Engine.PrintLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);

            byte[] buffer = packet.Buffer;

            //GCHandle pinnedArray = GCHandle.Alloc(packet.Buffer, GCHandleType.Pinned);
            //byte* data = (byte*)pinnedArray.AddrOfPinnedObject();

            BitReader reader = new BitReader(buffer);

            int packetType = reader.ReadByte();
            if (MessageTypeFuncs.IndexInArrayRange(packetType))
                MessageTypeFuncs[packetType](packet, reader);
            
            //pinnedArray.Free();
        }
        public void ReadDataPacket(Packet packet, BitReader reader)
        {

        }
        public void ReadInputPacket(Packet packet, BitReader reader)
        {

        }
        public abstract void ReadConnectionPacket(Packet packet, BitReader reader);
        public void ReadStatePacket(Packet packet, BitReader reader)
        {
            
        }
        #endregion

        #region Sending
        public unsafe void SendPacket<T>(T data, DataLinkKind kind = DataLinkKind.IpV4) where T : unmanaged
            => SendPacket(data.ToByteArray(), kind);
        public unsafe void SendPacket(byte[] data, DataLinkKind kind = DataLinkKind.IpV4)
            => _comm.SendPacket(new Packet(data, DateTime.Now, kind));
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _comm.Dispose();
                    _comm = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetworkConnection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
