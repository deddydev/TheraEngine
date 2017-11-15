using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TheraEngine.Networking
{
    public static class Network
    {
        public static bool IsConnected => NetworkInterface.GetIsNetworkAvailable();
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
    }
}
