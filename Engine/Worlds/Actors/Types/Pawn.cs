using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Rendering.Cameras;
using System.Linq;

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
        private PhysicsState _physicsState;
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
        public Pawn(PlayerIndex possessor) : base() { Engine.QueuePossession(this, possessor); }
        public Pawn(SceneComponent root, params LogicComponent[] logicComponents)
        : base (root, logicComponents) { }
        public Pawn(PlayerIndex possessor, SceneComponent root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { Engine.QueuePossession(this, possessor); }

        public virtual void OnPossessed(PawnController c)
        {
            if (c == null)
                OnUnPossessed();

            _controller = c;

            LocalPlayerController controller = LocalPlayerController;
            if (controller != null && _currentCameraComponent != null)
                controller.CurrentCamera = _currentCameraComponent.Camera;
        }
        public virtual void OnUnPossessed()
        {
            if (_controller == null)
                return;

            _controller = null;
        }
        public virtual void RegisterInput(InputInterface input) { }
        internal override void Tick(float delta)
        {
            if (Engine.World == null)
                return;

            Box bounds = Engine.World.Settings.OriginRebaseBounds;
            if (!bounds.Contains(RootComponent.LocalTransform.Translation))
                Engine.World.RebaseOrigin(RootComponent.LocalTransform.Translation);
        }
    }
}
