using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class PlayerController : PawnController
    {
        PlayerInfo _playerInfo;
        Viewport _viewport;
        internal int _number;

        public PlayerController()
        {
            Engine.ActivePlayers.Add(this);
            Engine.RemakePlayerNumbers();
        }
        ~PlayerController()
        {
            if (Engine.ActivePlayers.Contains(this))
            {
                Engine.ActivePlayers.Remove(this);
                Engine.RemakePlayerNumbers();
            }
        }

        public Camera CurrentCamera { get { return _viewport.Camera; } set { _viewport.Camera = value; } }
        public int Number { get { return _number; } }
    }
}
