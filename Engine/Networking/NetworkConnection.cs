using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace TheraEngine.Networking
{
    public unsafe delegate void DelDataRecieved(NetworkPacketObject* data, int length);
    public class NetworkConnection : IDisposable
    {
        public static bool AnyConnectionsAvailable => NetworkInterface.GetIsNetworkAvailable();
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
        public void Start()
        {
            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Engine.PrintLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }
            
            for (int i = 0; i < allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    Engine.PrintLine(" (" + device.Description + ")");
                else
                    Engine.PrintLine(" (No description available)");
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
            
            PacketDevice selectedDevice = allDevices[deviceIndex - 1];

            _comm =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000);                                  // read timeout
            {
                Engine.PrintLine("Listening on " + selectedDevice.Description + "...");

                //A high level filtering expression.
                //See http://www.winpcap.org/docs/docs_40_2/html/group__language.html
                using (BerkeleyPacketFilter filter = _comm.CreateFilter("ip and udp"))
                {
                    _comm.SetFilter(filter);
                }
            }
        }

        public void RecievePackets()
        {
            do
            {
                PacketCommunicatorReceiveResult result = _comm.ReceivePacket(out Packet packet);
                switch (result)
                {
                    case PacketCommunicatorReceiveResult.Timeout:
                        continue;
                    case PacketCommunicatorReceiveResult.Ok:
                        PacketRecieved(packet);
                        break;
                    default:
                        throw new InvalidOperationException("The result " + result + " should never be reached here");
                }
            } while (true);
        }

        private unsafe void PacketRecieved(Packet packet)
        {
            Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            
            Engine.PrintLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);

            GCHandle pinnedArray = GCHandle.Alloc(packet.Buffer, GCHandleType.Pinned);
            NetworkPacketObject* data = (NetworkPacketObject*)pinnedArray.AddrOfPinnedObject();



            pinnedArray.Free();
        }
        
        public event DelDataRecieved DataRecieved;
        public unsafe void SendData(NetworkPacketObject* data, int length)
        {
            byte[] dataArr = new byte[length];
            Marshal.Copy(new IntPtr(data), 0, dataArr, length);
            Packet p = new Packet()
            _comm.SendPacket()
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

        //public void ConnectionEstablishShutdown(Connection connection)
        //{

        //}
    }
}
