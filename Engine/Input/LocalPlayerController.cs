using System.Collections.Generic;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds.Actors;
using System.Collections.Concurrent;
using System;

namespace TheraEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        public static Dictionary<int, ConcurrentQueue<Camera>> CameraPossessionQueue = new Dictionary<int, ConcurrentQueue<Camera>>();

        public LocalPlayerController(LocalPlayerIndex index, Queue<IPawn> possessionQueue = null) : base()
        {
            _index = index;
            int i = (int)index;

            _input = new InputInterface(i);
            _input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0 && ControlledPawn == null)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(i))
            {
                Camera camera;
                while (!CameraPossessionQueue[i].TryDequeue(out camera)) ;
                CurrentCamera = camera;
            }
        }
        public LocalPlayerController(LocalPlayerIndex index) : base()
        {
            _index = index;
            int i = (int)index;

            _input = new InputInterface(i);
            _input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = new Queue<IPawn>();

            if (CameraPossessionQueue.ContainsKey(i))
            {
                Camera camera;
                while (!CameraPossessionQueue[i].TryDequeue(out camera)) ;
                CurrentCamera = camera;
            }
        }
        ~LocalPlayerController()
        {
            _input.WantsInputsRegistered -= RegisterInput;
            int index = (int)_index;
            if (index >= 0 && index < Engine.ActivePlayers.Count)
                Engine.ActivePlayers.RemoveAt(index);
        }

        private Viewport _viewport;
        private LocalPlayerIndex _index;
        protected InputInterface _input;

        public InputInterface Input => _input;
        public LocalPlayerIndex LocalPlayerIndex => _index;
        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                if (_viewport != null && _viewport.OwningPanel.GlobalHud != null)
                    _input.WantsInputsRegistered -= _viewport.OwningPanel.GlobalHud.RegisterInput;
                _viewport = value;
                if (_viewport != null)
                {
                    if (_controlledPawn != null)
                        UpdateViewport();

                    if (_viewport.OwningPanel.GlobalHud != null)
                        _input.WantsInputsRegistered += _viewport.OwningPanel.GlobalHud.RegisterInput;
                }
            }
        }
        public Camera CurrentCamera
        {
            get => _viewport?.Camera;
            set
            {
                if (_viewport != null)
                    _viewport.Camera = value;
            }
        }

        public override IPawn ControlledPawn
        {
            get => base.ControlledPawn;
            set
            {
                if (_controlledPawn == value)
                    return;

                if (_controlledPawn != null)
                {
                    _controlledPawn.OnUnPossessed();
                    _input.TryUnregisterInput();

                    _input.WantsInputsRegistered -= _controlledPawn.RegisterInput;
                    if (_controlledPawn.Hud != null && _controlledPawn != _controlledPawn.Hud)
                        _input.WantsInputsRegistered -= _controlledPawn.Hud.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                if (_controlledPawn != null)
                {
                    if (_viewport != null)
                        UpdateViewport();

                    _input.WantsInputsRegistered += _controlledPawn.RegisterInput;
                    if (_controlledPawn.Hud != null && _controlledPawn != _controlledPawn.Hud)
                        _input.WantsInputsRegistered += _controlledPawn.Hud.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
        }
        private void UpdateViewport()
        {
            _viewport.PawnHUD = _controlledPawn.Hud;
            if (_controlledPawn.CurrentCameraComponent != null)
                _viewport.Camera = _controlledPawn.CurrentCameraComponent.Camera;
        }
        protected virtual void RegisterInput(InputInterface input)
        {
            //input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause);
            //input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause);
        }
        public void TogglePause()
        {
            Engine.TogglePause(LocalPlayerIndex);
        }
        public void SetPause(bool paused)
        {
            Engine.SetPaused(paused, LocalPlayerIndex);
        }
        internal void Destroy()
        {
            UnlinkControlledPawn();
            Viewport = null;
            _input.WantsInputsRegistered -= RegisterInput;
        }
        internal void UnlinkControlledPawn()
        {
            _possessionQueue.Clear();
            ControlledPawn = null;
        }
    }
}
