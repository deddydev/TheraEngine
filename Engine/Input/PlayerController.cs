using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class PlayerController : PawnController
    {
        PlayerInfo _playerInfo;
        Viewport _viewport;

        public Camera CurrentCamera { get; set; }
        public int Number { get; set; }
    }
}
