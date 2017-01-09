using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Rendering.Cameras;
using System.Linq;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors
{
    public enum PlayerIndex
    {
        One     = 0,
        Two     = 1,
        Three   = 2,
        Four    = 3
    }
    public class Pawn : Actor
    {
        private PawnController _controller;

        public PawnController Controller { get { return _controller; } }
        public LocalPlayerController LocalPlayerController { get { return _controller as LocalPlayerController; } }
        private CameraComponent _currentCameraComponent;

        public CameraComponent CurrentCameraComponent
        {
            get { return _currentCameraComponent; }
            set
            {
                _currentCameraComponent = value;
                LocalPlayerController controller = LocalPlayerController;
                if (controller != null)
                    controller.CurrentCamera = value.Camera;
            }
        }

        public Pawn() : base() { }
        public Pawn(PlayerIndex possessor) : base() { QueuePossession(possessor); }
        public Pawn(SceneComponent root, params LogicComponent[] logicComponents)
        : base (root, logicComponents) { }
        public Pawn(PlayerIndex possessor, SceneComponent root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { QueuePossession(possessor); }

        public void QueuePossession(PlayerIndex possessor)
        {
            Engine.QueuePossession(this, possessor);
        }

        internal virtual void OnPossessed(PawnController c)
        {
            if (c == null)
                OnUnPossessed();

            _controller = c;

            LocalPlayerController controller = LocalPlayerController;
            if (controller != null && _currentCameraComponent != null)
                controller.CurrentCamera = _currentCameraComponent.Camera;
        }
        internal virtual void OnUnPossessed()
        {
            if (_controller == null)
                return;

            _controller = null;
        }
        public void RequestRegisterInput()
        {
            
        }
        public virtual void RegisterInput(InputInterface input) { }
        internal override void Tick(float delta)
        {
            if (Engine.World == null)
                return;

            BoundingBox bounds = Engine.World.Settings.OriginRebaseBounds;
            Vec3 point = RootComponent.WorldMatrix.GetPoint();
            if (!bounds.Contains(point))
                Engine.World.RebaseOrigin(point);
        }
        protected Viewport GetViewport()
        {
            LocalPlayerController player = LocalPlayerController;
            if (player == null)
                return null;
            return player.Viewport;
        }
    }
}
