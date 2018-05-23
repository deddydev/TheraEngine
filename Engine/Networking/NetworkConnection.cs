//using NetworkCommsDotNet;
//using NetworkCommsDotNet.Connections;
//using NetworkCommsDotNet.Connections.UDP;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TheraEngine.Networking
{
    public delegate void DelDataRecieved(NetworkPacketObject data);
    public class NetworkConnection
    {
        public static bool AnyConnectionsAvailable => NetworkInterface.GetIsNetworkAvailable();
        public static IPAddress GetLocalIPAddressV4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            throw new Exception("Local v4 IP Address Not Found!");
        }
        public static IPAddress GetLocalIPAddressV6()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    return ip;
            throw new Exception("Local v6 IP Address Not Found!");
        }

        public event DelDataRecieved DataRecieved;

        //private Connection _connection;

        public void Start(string ip = "192.168.0.1", int port = 10000)
        {
            //ConnectionInfo connInfo = new ConnectionInfo(ip, port);
            //_connection = UDPConnection.GetConnection(connInfo, UDPOptions.None, false);
            //_connection.AppendShutdownHandler(ConnectionEstablishShutdown);

            //SendReceiveOptions customSendReceiveOptions = new SendReceiveOptions<>();

            //_connection.AppendIncomingPacketHandler<NetworkPacketObject>("ServerUpdate", RecievedData, sro);
        }
        //private void RecievedData(PacketHeader packetHeader, Connection connection, NetworkPacketObject data)
        //{
        //    DataRecieved?.Invoke(data);
        //}
        public void SendData(NetworkPacketObject data)
        {

        }
        //public void ConnectionEstablishShutdown(Connection connection)
        //{

        //}
    }
}
