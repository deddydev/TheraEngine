using TheraEngine.Input;
using TheraEngine.Input.Devices;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Components.Scene;
using TheraEngine.Components;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Worlds;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Actors
{
    public enum LocalPlayerIndex
    {
        One     = 0,
        Two     = 1,
        Three   = 2,
        Four    = 3
    }
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    public interface IPawn : IActor
    {
        PawnController Controller { get; }
        PlayerController ServerPlayerController { get; }
        AIController AIController { get; }
        LocalPlayerController LocalPlayerController { get; }
        CameraComponent CurrentCameraComponent { get; set; }
        IUserInterface HUD { get; set; }

        void QueuePossession(LocalPlayerIndex possessor);
        void OnUnPossessed();
        void OnPossessed(PawnController possessor);
        void RegisterInput(InputInterface input);
    }
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    public class Pawn : Pawn<OriginRebasableComponent>
    {
        public Pawn() : base() { }
        public Pawn(OriginRebasableComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    /// <summary>
    /// A pawn is an actor that can be controlled by either a player or AI.
    /// </summary>
    [FileExt("pawn")]
    [FileDef("Pawn Actor")]
    public class Pawn<T> : Actor<T>, IPawn where T : OriginRebasableComponent
    {
        private PawnController _controller;
        private CameraComponent _currentCameraComponent;
        private IUserInterface _hud = null;
        
        [Category("Pawn")]
        public PawnController Controller => _controller;
        [Browsable(false)]
        public PlayerController ServerPlayerController => Controller as PlayerController;
        [Browsable(false)]
        public AIController AIController => Controller as AIController;
        [Browsable(false)]
        public LocalPlayerController LocalPlayerController => Controller as LocalPlayerController;

        [Browsable(false)]
        public Viewport Viewport => LocalPlayerController?.Viewport;

        /// <summary>
        /// Dictates the component controlling the view of this pawn's controller.
        /// </summary>
        [Browsable(false)]
        public CameraComponent CurrentCameraComponent
        {
            get => _currentCameraComponent;
            set
            {
                _currentCameraComponent = value;
                LocalPlayerController controller = LocalPlayerController;
                if (controller != null)
                    controller.ViewportCamera = _currentCameraComponent.CameraRef.File;
            }
        }

        [Category("Pawn")]
        public IUserInterface HUD
        {
            get => _hud;
            set
            {
                if (_hud != null && _hud.OwningPawn == this)
                {
                    _hud.OwningPawn = null;
                }
                _hud = value;
                if (_hud != null)
                {
                    _hud.OwningPawn = this;
                }
            }
        }

        public override void OnSpawnedPreComponentSpawn()
        {
            HUD?.Spawned(OwningWorld);
        }

        public override void OnSpawnedPostComponentSpawn()
        {
            if (OwningWorld.Settings.EnableOriginRebasing)
                RootComponent.WorldTransformChanged += QueueWorldRebase;
        }

        public override void OnDespawned()
        {
            if (OwningWorld.Settings.EnableOriginRebasing)
                RootComponent.WorldTransformChanged -= QueueWorldRebase;
            HUD?.Despawned();
        }

        public Pawn() : this(false) { }
        public Pawn(bool deferInitialization) : base(deferInitialization) { }
        public Pawn(bool deferInitialization, LocalPlayerIndex possessor) : base(deferInitialization) { QueuePossession(possessor); }
        public Pawn(T root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { }
        public Pawn(LocalPlayerIndex possessor, T root, params LogicComponent[] logicComponents)
        : base(root, logicComponents) { QueuePossession(possessor); }

        public void QueuePossession(LocalPlayerIndex possessor)
            => Engine.QueuePossession(this, possessor);
        
        public virtual void OnPossessed(PawnController possessor)
        {
            if (possessor == null)
                OnUnPossessed();

            _controller = possessor;
        }
        public virtual void OnUnPossessed()
        {
            if (_controller == null)
                return;

            _controller = null;
        }

        public virtual void RegisterInput(InputInterface input) { }
        
        public void QueueWorldRebase()
        {
            if (OwningWorld == null)
                return;

            BoundingBox bounds = OwningWorld.Settings.OriginRebaseBounds;
            Vec3 point = RootComponent.WorldMatrix.Translation;
            if (!bounds.Contains(point))
                Engine.QueueRebaseOrigin(OwningWorld, point);
        }
        public bool IsInWorldBounds()
        {
            if (OwningWorld == null)
                return true;

            return IsInBounds(OwningWorld.Settings.Bounds);
        }
        public bool IsInBounds(BoundingBox bounds)
            => bounds.Contains(RootComponent.WorldMatrix.Translation);
    }
}
