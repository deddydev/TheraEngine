using TheraEngine.Input;

namespace TheraEngine.Players
{
    public class PlayerInfo
    {
        private string _userName;

        //This player's index on the server.
        private int _index = -1;

        public int Index => _index;
        public string UserName => _userName;
    }
}
