using System.Collections.Generic;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds.Actors;
using System.Collections.Concurrent;
using System.ComponentModel;
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
                ViewportCamera = camera;
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
                ViewportCamera = camera;
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

        [Category("Local Player Controller")]
        public InputInterface Input => _input;

        [Category("Local Player Controller")]
        public LocalPlayerIndex LocalPlayerIndex => _index;

        [Category("Local Player Controller")]
        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                //if (_viewport != null && _viewport.OwningPanel.GlobalHud != null)
                //    _input.WantsInputsRegistered -= _viewport.OwningPanel.GlobalHud.RegisterInput;

                _viewport = value;

                UpdateViewport(SetViewportHUD, SetViewportCamera);

                //if (_viewport.OwningPanel.GlobalHud != null)
                //    _input.WantsInputsRegistered += _viewport.OwningPanel.GlobalHud.RegisterInput;

            }
        }

        [Category("Local Player Controller")]
        public Camera ViewportCamera
        {
            get => _viewport?.Camera;
            set
            {
                if (_viewport != null)
                    _viewport.Camera = value;
            }
        }

        [Category("Pawn Controller")]
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
                    if (_controlledPawn.HUD != null && _controlledPawn != _controlledPawn.HUD)
                        _input.WantsInputsRegistered -= _controlledPawn.HUD.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                Engine.PrintLine("Assigned new controlled pawn to Player " + _index + ": " + _controlledPawn.GetType().GetFriendlyName());

                if (_controlledPawn != null)
                {
                    UpdateViewport(SetViewportHUD, SetViewportCamera);

                    _input.WantsInputsRegistered += _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD != null && _controlledPawn != _controlledPawn.HUD)
                        _input.WantsInputsRegistered += _controlledPawn.HUD.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
        }

        private bool _setViewportHUD = true, _setViewportCamera = true;

        [Category("Local Player Controller")]
        public bool SetViewportHUD
        {
            get => _setViewportHUD;
            set
            {
                _setViewportHUD = value;
                UpdateViewport(_setViewportHUD, _setViewportCamera);
            }
        }
        [Category("Local Player Controller")]
        public bool SetViewportCamera
        {
            get => _setViewportCamera;
            set
            {
                _setViewportCamera = value;
                UpdateViewport(_setViewportHUD, _setViewportCamera);
            }
        }

        /// <summary>
        /// Updates the viewport with the HUD and/or camera from the controlled pawn.
        /// </summary>
        public void UpdateViewport(bool setHUD, bool setCamera)
        {
            if (_viewport == null || _controlledPawn == null)
                return;

            if (setHUD)
                _viewport.HUD = _controlledPawn.HUD;

            if (setCamera)
                _viewport.Camera = _controlledPawn.CurrentCameraComponent?.Camera;
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
