//using NetworkCommsDotNet;
//using NetworkCommsDotNet.Connections;
//using NetworkCommsDotNet.Connections.UDP;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TheraEngine.Networking
{
    public unsafe delegate void DelDataRecieved(NetworkPacketObject* data, int length);
    public class NetworkConnection
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
        
        public void Start(string ip = "192.168.0.1", int port = 10000)
        {
            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            int deviceIndex = 0;
            do
            {
                Console.WriteLine("Enter the interface number (1-" + allDevices.Count + "):");
                string deviceIndexString = Console.ReadLine();
                if (!int.TryParse(deviceIndexString, out deviceIndex) ||
                    deviceIndex < 1 || deviceIndex > allDevices.Count)
                {
                    deviceIndex = 0;
                }
            } while (deviceIndex == 0);

            // Take the selected adapter
            PacketDevice selectedDevice = allDevices[deviceIndex - 1];

            // Open the device
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                Engine.PrintLine("Listening on " + selectedDevice.Description + "...");

                // Compile the filter
                using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp"))
                {
                    // Set the filter
                    communicator.SetFilter(filter);
                }

                // Retrieve the packets
                Packet packet;
                do
                {
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            // Timeout elapsed
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:
                            PacketRecieved(packet);
                            break;
                        default:
                            throw new InvalidOperationException("The result " + result + " should never be reached here");
                    }
                } while (true);
            }
        }

        private unsafe void PacketRecieved(Packet packet)
        {
            Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            
            Console.WriteLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);

            GCHandle pinnedArray = GCHandle.Alloc(packet.Buffer, GCHandleType.Pinned);
            NetworkPacketObject* data = (NetworkPacketObject*)pinnedArray.AddrOfPinnedObject();



            pinnedArray.Free();
        }
        
        public event DelDataRecieved DataRecieved;
        public unsafe void SendData(NetworkPacketObject* data, int length)
        {

        }

        //public void ConnectionEstablishShutdown(Connection connection)
        //{

        //}
    }
}
