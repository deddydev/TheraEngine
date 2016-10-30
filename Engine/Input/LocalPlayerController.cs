using System;
using CustomEngine.Input.Gamepads;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        private Viewport _viewport;

        public GamepadManager Gamepad { get { return _input as GamepadManager; } }
        public int LocalPlayerIndex { get { return _viewport.Index; } }
        public Viewport Viewport
        {
            get { return _viewport; }
            internal set { _viewport = value; }
        }
        public Camera CurrentCamera
        {
            get { return _viewport.Camera; }
            set { _viewport.Camera = value; }
        }
        internal void SetInputLibrary()
        {
            if (Gamepad == null)
                return;
            int controllerIndex = Gamepad.ControllerIndex;
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK:
                    _input = new TKGamepadManager(controllerIndex);
                    break;
                case InputLibrary.XInput:
                    _input = new DXGamepadManager(controllerIndex);
                    break;
            }
        }
        public LocalPlayerController(GamepadManager gamepad)
        {
            _input = gamepad;
            Engine.ActivePlayers.Add(this);
        }
        ~LocalPlayerController()
        {
            if (Engine.ActivePlayers.Contains(this))
                Engine.ActivePlayers.Remove(this);
        }
    }
}
