using System;
using CustomEngine.Input.Devices;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        private Viewport _viewport;
        private int _index;
        protected InputInterface _input;

        public InputInterface Input { get { return _input; } }
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
        public LocalPlayerController()
        {
            _index = Engine.ActivePlayers.Count;
            _input = new InputInterface(_index);
            Engine.ActivePlayers.Add(this);
        }
        ~LocalPlayerController()
        {
            if (Engine.ActivePlayers.Contains(this))
                Engine.ActivePlayers.Remove(this);
        }
    }
}
