using CustomEngine.Input.Devices;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class PlayerController : PawnController
    {
        PlayerInfo _playerInfo;

        public int ServerPlayerIndex { get { return _playerInfo != null ? _playerInfo.Index : -1; } }

        public PlayerController()
        {

        }
        ~PlayerController()
        {

        }
    }
}
