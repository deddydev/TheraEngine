using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        public static Dictionary<int, ConcurrentQueue<Camera>> CameraPossessionQueue = new Dictionary<int, ConcurrentQueue<Camera>>();

        public LocalPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null) : base()
        {
            LocalPlayerIndex = index;
            int i = (int)index;

            _input = new InputInterface(i);
            _input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = possessionQueue ?? new Queue<IPawn>();
            if (_possessionQueue.Count != 0 && ControlledPawn == null)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(i))
            {
                Camera camera;
                while (!CameraPossessionQueue[i].TryDequeue(out camera)) ;
                ViewportCamera = camera;
            }
        }
        public LocalPlayerController(ELocalPlayerIndex index) : base()
        {
            LocalPlayerIndex = index;
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

        private Viewport _viewport;
        protected InputInterface _input;

        [Category("Local Player Controller")]
        public InputInterface Input => _input;

        [Category("Local Player Controller")]
        public ELocalPlayerIndex LocalPlayerIndex { get; }

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
        public ICamera ViewportCamera
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
                        _input.WantsInputsRegistered -= _controlledPawn.HUD.File.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                //Engine.PrintLine("Assigned new controlled pawn to Player " + _index + ": " + (_controlledPawn == null ? "null" : _controlledPawn.GetType().GetFriendlyName()));

                if (_controlledPawn != null)
                {
                    UpdateViewport(SetViewportHUD, SetViewportCamera);

                    _input.WantsInputsRegistered += _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD != null && _controlledPawn != _controlledPawn.HUD)
                        _input.WantsInputsRegistered += _controlledPawn.HUD.File.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
        }

        private bool _setViewportHUD = true, _setViewportCamera = true;

        /// <summary>
        /// Determines if this local player controller should update
        /// its assigned viewport with the possessed pawn's HUD.
        /// </summary>
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
        /// <summary>
        /// Determines if this local player controller should update
        /// its assigned viewport with the possessed pawn's camera.
        /// </summary>
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
                _viewport.HUD = _controlledPawn.HUD?.File;

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
        public void UnlinkControlledPawn()
        {
            _possessionQueue.Clear();
            ControlledPawn = null;
        }
    }
}
