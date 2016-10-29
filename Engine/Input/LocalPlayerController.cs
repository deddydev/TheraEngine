using CustomEngine.Input.Gamepads;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        private Viewport _viewport;
        private GamepadManager _gamepad;
        
        public GamepadManager Gamepad { get { return _gamepad; } }
        public int LocalPlayerIndex { get { return _viewport.Index; } }
        public Viewport Viewport
        {
            get { return _viewport; }
            internal set { _viewport = value; }
        }
        
        internal void SetInputLibrary()
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK:
                    TKGamepadAwaiter.Await(FoundGamepad);
                    break;
                case InputLibrary.XInput:
                    DXGamepadAwaiter.Await(FoundGamepad);
                    break;
            }
        }

        private void FoundGamepad(int controllerIndex)
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK:
                    _gamepad = new TKGamepadManager(controllerIndex);
                    break;
                case InputLibrary.XInput:
                    _gamepad = new DXGamepadManager(controllerIndex);
                    break;
            }
        }

        public LocalPlayerController()
        {
            Engine.ActivePlayers.Add(this);
            SetInputLibrary();
        }
        ~LocalPlayerController()
        {
            if (Engine.ActivePlayers.Contains(this))
                Engine.ActivePlayers.Remove(this);
        }

        public Camera CurrentCamera
        {
            get { return _viewport.Camera; }
            set { _viewport.Camera = value; }
        }
    }
}
