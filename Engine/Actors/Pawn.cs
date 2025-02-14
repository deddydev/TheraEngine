﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.ComponentModel;
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
        ICameraComponent CurrentCameraComponent { get; set; }
        LocalFileRef<IUserInterfacePawn> HUD { get; set; }

        void Possess(ELocalPlayerIndex possessor);
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

        private ICameraComponent _currentCameraComponent;
        private LocalFileRef<IUserInterfacePawn> _hud = null;

        [Browsable(false)]
        public ELocalPlayerIndex? ForcePossession { get; private set; }
        [Browsable(false)]
        public Queue<ELocalPlayerIndex> PossessionQueue { get; private set; } = new Queue<ELocalPlayerIndex>();
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
        
        /// <summary>
        /// The viewport of the local player controller that is controlling this pawn.
        /// </summary>
        [Browsable(false)]
        public Viewport Viewport => LocalPlayerController?.Viewport;

        /// <summary>
        /// Dictates the component controlling the view of this pawn's controller.
        /// </summary>
        [Browsable(false)]
        public ICameraComponent CurrentCameraComponent
        {
            get => _currentCameraComponent;
            set
            {
                if (_currentCameraComponent == value)
                    return;
                
                _currentCameraComponent = value;

                LocalPlayerController controller = LocalPlayerController;
                if (controller != null)
                    controller.ViewportCamera = _currentCameraComponent?.Camera;
            }
        }

        /// <summary>
        /// The HUD for this pawn.
        /// </summary>
        [TSerialize]
        [Category("Pawn")]
        public LocalFileRef<IUserInterfacePawn> HUD
        {
            get => _hud;
            set
            {
                if (_hud != null)
                {
                    if (_hud.IsLoaded)
                        HudUnloaded(_hud.File);
                    _hud.Loaded -= HudLoaded;
                    _hud.Unloaded -= HudUnloaded;
                }
                _hud = value;
                if (_hud != null)
                {
                    _hud.Loaded += HudLoaded;
                    _hud.Unloaded += HudUnloaded;
                }
            }
        }

        private void HudUnloaded(IUserInterfacePawn obj)
        {
            if (IsSpawned  && obj is IActor actor && actor.OwningWorld == OwningWorld)
                actor.Despawned();
            obj.OwningPawn = null;
        }
        private void HudLoaded(IUserInterfacePawn obj)
        {
            obj.OwningPawn = this;
            if (IsSpawned && OwningWorld != null && obj is IActor actor)
                actor.Spawned(OwningWorld);
        }

        protected override void OnSpawnedPreComponentSpawn()
        {
            if (HUD?.File is IActor actor)
                actor.Spawned(OwningWorld);
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            OwningWorld.Settings.EnableOriginRebasingChanged += Settings_EnableOriginRebasingChanged;
            Settings_EnableOriginRebasingChanged(OwningWorld.Settings);

            var mode = OwningWorld.GameMode;
            if (mode != null)
            {
                if (ForcePossession != null)
                    mode.ForcePossession(this, ForcePossession.Value);
                mode.QueuePossession(this, PossessionQueue);

                ForcePossession = null;
                PossessionQueue.Clear();
            }
            //OwningWorld.CurrentGameModePreChanged += OwningWorld_CurrentGameModePreChanged;
        }

        //private void OwningWorld_CurrentGameModePreChanged(World world, BaseGameMode previous, BaseGameMode next)
        //{
        //    PossessionQueue = previous.CollectPossessionQueueFor(this);
        //}

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

            //OwningWorld.CurrentGameModePreChanged -= OwningWorld_CurrentGameModePreChanged;
        }
        /// <summary>
        /// The given local player will eventually control this pawn after other players in the queue have possessed the pawn first.
        /// </summary>
        /// <param name="possessor"></param>
        public void QueuePossession(ELocalPlayerIndex possessor)
        {
            var mode = OwningWorld?.GameMode;
            if (mode != null)
                mode.QueuePossession(this, possessor);
            else
                PossessionQueue.Enqueue(possessor);
        }
        /// <summary>
        /// The given local player will have immediate control over this pawn.
        /// </summary>
        /// <param name="possessor"></param>
        public void Possess(ELocalPlayerIndex possessor)
        {
            var mode = OwningWorld?.GameMode;
            if (mode != null)
                mode.ForcePossession(this, possessor);
            else
                ForcePossession = possessor;
        }
        /// <summary>
        /// Called when a new controller starts controlling this pawn.
        /// </summary>
        /// <param name="possessor"></param>
        public virtual void OnPossessed(PawnController possessor)
        {
            if (possessor is null)
                OnUnPossessed();

            Controller = possessor;

            if (Controller is LocalPlayerController lpc && lpc.Viewport != null)
                RootComponent?.OnGotAudioListener();
        }
        /// <summary>
        /// Called when a controller loses control of this pawn.
        /// Controller is set to null at the end of the method.
        /// </summary>
        public virtual void OnUnPossessed()
        {
            if (Controller is null)
                return;

            if (Controller is LocalPlayerController lpc && lpc.Viewport != null)
                RootComponent?.OnLostAudioListener();

            Controller = null;
        }

        public virtual void RegisterInput(InputInterface input) { }
        
        public void QueueWorldRebase(ISceneComponent comp)
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
