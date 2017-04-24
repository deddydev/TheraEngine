using CustomEngine.Input;

namespace CustomEngine.Players
{
    public class PlayerInfo
    {
        public PlayerInfo(PlayerController controller)
        {
            _controller = controller;
        }

        public PlayerController _controller;
        private string _userName;

        //This player's index on the server.
        private int _index = -1;

        public int Index { get { return _index; } }
        public string UserName { get { return _userName; } }
    }
}
