using Ayx.BitIO;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace TheraEngine.Networking
{
    public class Client : NetworkConnection
    {
        public override bool IsServer => false;
        public override int LocalPort => ClientPort;
        public override int RemotePort => ServerPort;

        public int ServerIndex { get; set; }
        public IPEndPoint ServerEndPoint { get; set; }
        
        private bool _connectionResponseRecieved = false;
        private bool _connectionAccepted = false;

        private NetworkClient _connection;

        /// <summary>
        /// Attempts to connect to the server.
        /// </summary>
        /// <param name="timeout">How many seconds to try connecting before giving up.</param>
        /// <param name="packetsPerSecond">How many packets to send each second.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        public bool RequestConnection(float timeout = 5.0f, int packetsPerSecond = 10)
        {
            _connectionResponseRecieved = false;
            _connectionAccepted = false;
            
            int millisecondsPerPacket = 1000 / packetsPerSecond;

            TPacketConnection conn = new TPacketConnection();
            conn.Header.PacketType = EPacketType.Connection;
            conn.ConnectionMessage = EConnectionMessage.Request;
            SendPacket(conn, timeout, () => _connectionResponseRecieved);

            TimeSpan timeoutSpan = TimeSpan.FromSeconds(timeout);
            Stopwatch watch = Stopwatch.StartNew();
            BlockWhile(() => !_connectionResponseRecieved && watch.Elapsed < timeoutSpan);

            if (!_connectionResponseRecieved)
                Engine.Out("Server connection request timed out.");
            else
                Engine.Out("Recieved a response from the server.");

            return _connectionAccepted;
        }

        private void BlockWhile(Func<bool> func) { while (func()) ; }

        public Task<bool> RequestConnectionAsync(float timeout = 5.0f, int packetsPerSecond = 10)
            => Task.Run(() => RequestConnection(timeout, packetsPerSecond));
        public void CancelConnectionRequest()
        {
            _connectionResponseRecieved = true;
        }
        public override void ReadConnectionPacket(IPEndPoint endPoint, BitReader reader)
        {
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
                    Engine.Out("Server denied connection request.");
                    break;
                case EConnectionMessage.Accepted:
                    if (_connectionResponseRecieved &&
                        _connectionAccepted && 
                        ServerEndPoint.ToString() == endPoint.ToString())
                    {
                        Engine.Out("Server previously accepted connection request. Ignoring new request.");
                    }
                    else
                    {
                        //The server successfully added this client to the list.
                        ServerIndex = reader.ReadByte();
                        ServerEndPoint = endPoint;
                        //ConnectionAccepted?.Invoke();
                        _connectionAccepted = true;
                        _connectionResponseRecieved = true;
                        Engine.Out("Server accepted connection request.");
                    }
                    break;
                case EConnectionMessage.LocalPlayerCountChanged:
                    
                    //This is the index of the first local player in the server player list.
                    //All other local players are directly after.
                    int serverLocalPlayerStartIndex = reader.ReadByte();
                    
                    break;
            }
        }

        public override void UpdatePacketQueue(float delta)
        {
            _connection?.UpdatePacketQueue(delta);
        }
    }
}
