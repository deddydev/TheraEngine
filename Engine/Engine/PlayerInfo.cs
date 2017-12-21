namespace TheraEngine.Players
{
    public class PlayerInfo
    {
        private string _userName;

        //This player's index on the server.
        private int _serverIndex = -1;

        public int ServerIndex => _serverIndex;
        public string UserName => _userName;
    }
}
