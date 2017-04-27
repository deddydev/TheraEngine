using CustomEngine.Input;

namespace CustomEngine.Players
{
    public class PlayerInfo
    {
        public PlayerInfo(PlayerController controller)
        {
            _controller = controller;
        }

        private string _userName;
        public PlayerController _controller;

        //This player's index on the server.
        private int _index = -1;

        public int Index => _index;
        public string UserName => _userName;
    }
}
