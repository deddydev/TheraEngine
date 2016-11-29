using CustomEngine.Input;

namespace CustomEngine.Players
{
    public class PlayerInfo
    {
        public PlayerController _controller;
        private string _userName;

        //This player's index on the server.
        private int _index;

        public int Index { get { return _index; } }
        public string UserName { get { return _userName; } }
    }
}
