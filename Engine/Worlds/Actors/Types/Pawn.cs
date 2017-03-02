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
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    public interface IPawn
    {
        PawnController Controller { get; }
        LocalPlayerController LocalPlayerController { get; }
        CameraComponent CurrentCameraComponent { get; set; }
        void QueuePossession(PlayerIndex possessor);
        void OnUnPossessed();
        void OnPossessed(PawnController possessor);
        void RegisterInput(InputInterface input);
    }
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    public class Pawn : Pawn<SceneComponent>
    {
        public Pawn() : base() { }
        public Pawn(SceneComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pawn<T> : Actor<T>, IPawn where T : SceneComponent
    {
        private PawnController _controller;
        private CameraComponent _currentCameraComponent;

        public PawnController Controller => _controller;
        public LocalPlayerController LocalPlayerController => _controller as LocalPlayerController;
        public CameraComponent CurrentCameraComponent
        {
            get => _currentCameraComponent;
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
        public Pawn(T root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { }
        public Pawn(PlayerIndex possessor, T root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { QueuePossession(possessor); }

        public void QueuePossession(PlayerIndex possessor)
        {
            Engine.QueuePossession(this, possessor);
        }

        public virtual void OnPossessed(PawnController possessor)
        {
            if (possessor == null)
                OnUnPossessed();

            _controller = possessor;

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
