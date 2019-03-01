using System;
using System.ComponentModel;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Actors
{
    public enum ELocalPlayerIndex
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
        ServerPlayerController ServerPlayerController { get; }
        AIController AIController { get; }
        LocalPlayerController LocalPlayerController { get; }
        CameraComponent CurrentCameraComponent { get; set; }
        LocalFileRef<IUserInterface> HUD { get; set; }

        void ForcePossessionBy(ELocalPlayerIndex possessor);
        void QueuePossession(ELocalPlayerIndex possessor);
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
    [TFileExt("pawn")]
    [TFileDef("Pawn Actor")]
    public class Pawn<T> : Actor<T>, IPawn where T : OriginRebasableComponent
    {
        public Pawn()
            : this(false) { }
        public Pawn(bool deferInitialization)
            : base(deferInitialization) { }
        public Pawn(bool deferInitialization, ELocalPlayerIndex possessor)
            : base(deferInitialization) { QueuePossession(possessor); }
        public Pawn(T root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
        public Pawn(ELocalPlayerIndex possessor, T root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { QueuePossession(possessor); }

        private CameraComponent _currentCameraComponent;
        private LocalFileRef<IUserInterface> _hud = null;

        /// <summary>
        /// The interface that is managing and providing input to this pawn.
        /// </summary>
        [Browsable(false)]
        public PawnController Controller { get; private set; }
        /// <summary>
        /// Casts the controller to a server player controller.
        /// </summary>
        [Browsable(false)]
        public ServerPlayerController ServerPlayerController => Controller as ServerPlayerController;
        /// <summary>
        /// Casts the controller to an AI controller.
        /// </summary>
        [Browsable(false)]
        public AIController AIController => Controller as AIController;
        /// <summary>
        /// Casts the controller to a local player controller.
        /// </summary>
        [Browsable(false)]
        public LocalPlayerController LocalPlayerController => Controller as LocalPlayerController;
        /// <summary>
        /// Casts the controller to a generic player controller.
        /// </summary>
        [Browsable(false)]
        public PlayerController PlayerController => Controller as PlayerController;
        
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

        /// <summary>
        /// The HUD for this pawn.
        /// </summary>
        [TSerialize]
        [Category("Pawn")]
        public LocalFileRef<IUserInterface> HUD
        {
            get => _hud;
            set
            {
                if (_hud != null)
                {
                    if (_hud.IsLoaded)
                        HudUnloaded(_hud.File);
                    _hud.UnregisterLoadEvent(HudLoaded);
                    _hud.UnregisterUnloadEvent(HudUnloaded);
                }
                _hud = value;
                if (_hud != null)
                {
                    _hud.RegisterLoadEvent(HudLoaded);
                    _hud.RegisterUnloadEvent(HudUnloaded);
                }
            }
        }

        private void HudUnloaded(IUserInterface obj)
        {
            if (IsSpawned  && obj is BaseActor actor && actor.OwningWorld == OwningWorld)
                actor.Despawned();
            obj.OwningPawn = null;
        }
        private void HudLoaded(IUserInterface obj)
        {
            obj.OwningPawn = this;
            if (IsSpawned && OwningWorld != null && obj is BaseActor actor)
                actor.Spawned(OwningWorld);
        }

        protected override void OnSpawnedPreComponentSpawn()
        {
            if (HUD?.File is BaseActor actor)
                actor.Spawned(OwningWorld);
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            OwningWorld.Settings.EnableOriginRebasingChanged += Settings_EnableOriginRebasingChanged;
            Settings_EnableOriginRebasingChanged(OwningWorld.Settings);
        }
        private void Settings_EnableOriginRebasingChanged(WorldSettings settings)
        {
            if (settings.EnableOriginRebasing)
                RootComponent.WorldTransformChanged += QueueWorldRebase;
            else
                RootComponent.WorldTransformChanged -= QueueWorldRebase;
        }
        protected override void OnDespawned()
        {
            if (OwningWorld.Settings.EnableOriginRebasing)
                RootComponent.WorldTransformChanged -= QueueWorldRebase;

            if (HUD?.File is BaseActor actor)
                actor.Despawned();
        }

        public void QueuePossession(ELocalPlayerIndex possessor)
            => Engine.QueuePossession(this, possessor);
        public void ForcePossessionBy(ELocalPlayerIndex possessor)
            => Engine.ForcePossession(this, possessor);
        
        public virtual void OnPossessed(PawnController possessor)
        {
            if (possessor == null)
                OnUnPossessed();

            Controller = possessor;
        }
        public virtual void OnUnPossessed()
        {
            if (Controller == null)
                return;

            Controller = null;
        }

        public virtual void RegisterInput(InputInterface input) { }
        
        public void QueueWorldRebase(SceneComponent comp)
        {
            if (!IsSpawned || OwningWorld.IsRebasingOrigin)
                return;

            float rebaseRadius = OwningWorld.Settings.OriginRebaseRadius;
            Vec3 point = RootComponent.WorldMatrix.Translation;
            if (!Collision.SphereContainsPoint(Vec3.Zero, rebaseRadius, point))
                Engine.QueueRebaseOrigin(OwningWorld, point);
        }
        public bool IsInWorldBounds()
        {
            if (!IsSpawned)
                return true;

            return IsInBounds(OwningWorld.Settings.Bounds);
        }
        public bool IsInBounds(BoundingBox bounds)
            => bounds.Contains(RootComponent.WorldMatrix.Translation);
    }
}
