using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace TheraEngine.Networking
{
    public unsafe delegate void DelDataRecieved(TPacketHeader* data, int length);
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

        /// <summary>
        /// Retrieves all packets that have arrived and need to be processed.
        /// </summary>
        public void RecievePackets() => _comm.ReceiveSomePackets(out int retrievedCount, -1, PacketRecieved);
        
        private unsafe void PacketRecieved(Packet packet)
        {
            Engine.PrintLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            
            Engine.PrintLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);

            byte[] buffer = packet.Buffer;
            EPacketType type = (EPacketType)(buffer[0] >> 6);

            //GCHandle pinnedArray = GCHandle.Alloc(packet.Buffer, GCHandleType.Pinned);
            //TPacketHeader* data = (TPacketHeader*)pinnedArray.AddrOfPinnedObject();

            //switch (data->PacketType)
            //{
            //    default:
            //    case EPacketType.Invalid:

            //        break;
            //    case EPacketType.Input:
                    
            //        break;
            //    case EPacketType.Transform:

            //        break;
            //}

            //pinnedArray.Free();
        }
        
        public unsafe void SendPacket<T>(T data, DataLinkKind kind = DataLinkKind.IpV4) where T : unmanaged
            => SendPacket(CreatePacket(data), kind);
        
        public unsafe void SendPacket(byte[] data, DataLinkKind kind = DataLinkKind.IpV4)
            => _comm.SendPacket(new Packet(data, DateTime.Now, kind));
        
        public static unsafe byte[] CreatePacket<T>(T data) where T : unmanaged
        {
            byte[] dataArr = new byte[sizeof(T)];
            void* addr = &data;
            Marshal.Copy((IntPtr)addr, dataArr, 0, dataArr.Length);
            return dataArr;
        }

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
