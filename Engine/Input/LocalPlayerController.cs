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
        public static Dictionary<int, ConcurrentQueue<ICamera>> CameraPossessionQueue = new Dictionary<int, ConcurrentQueue<ICamera>>();

        public LocalPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null) : base()
        {
            LocalPlayerIndex = index;
            int i = (int)index;

            _input = new InputInterface(i);
            _input.InputRegistration += RegisterInput;

            _possessionQueue = possessionQueue ?? new Queue<IPawn>();
            if (_possessionQueue.Count != 0 && ControlledPawn is null)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(i))
            {
                ICamera camera;
                while (!CameraPossessionQueue[i].TryDequeue(out camera)) ;
                ViewportCamera = camera;
            }
        }
        public LocalPlayerController(ELocalPlayerIndex index) : base()
        {
            LocalPlayerIndex = index;
            int i = (int)index;

            _input = new InputInterface(i);
            _input.InputRegistration += RegisterInput;

            _possessionQueue = new Queue<IPawn>();

            if (CameraPossessionQueue.ContainsKey(i))
            {
                ICamera camera;
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

                UpdateViewport(InheritControlledPawnHUD, InheritControlledPawnCamera);

                //if (_viewport.OwningPanel.GlobalHud != null)
                //    _input.WantsInputsRegistered += _viewport.OwningPanel.GlobalHud.RegisterInput;

            }
        }

        [Category("Local Player Controller")]
        public ICamera ViewportCamera
        {
            get => _viewport?.AttachedCamera;
            set
            {
                if (_viewport != null)
                    _viewport.AttachedCamera = value;
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
                    _input.TryUnregisterInput();
                    _input.InputRegistration -= _controlledPawn.RegisterInput;

                    if (_controlledPawn.HUD != null && _controlledPawn != _controlledPawn.HUD)
                        _input.InputRegistration -= _controlledPawn.HUD.File.RegisterInput;

                    _controlledPawn.OnUnPossessed();

                    Engine.Out($"Player {((int)LocalPlayerIndex + 1).ToString()} released control of {_controlledPawn}");
                }

                _controlledPawn = value;

                if (_controlledPawn is null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                //Engine.PrintLine("Assigned new controlled pawn to Player " + _index + ": " + (_controlledPawn is null ? "null" : _controlledPawn.GetType().GetFriendlyName()));

                UpdateViewport(InheritControlledPawnHUD, InheritControlledPawnCamera);

                if (_controlledPawn != null)
                {
                    _input.InputRegistration += _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD != null && _controlledPawn != _controlledPawn.HUD)
                        _input.InputRegistration += _controlledPawn.HUD.File.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();

                    Engine.Out($"Player {((int)LocalPlayerIndex + 1).ToString()} gained control of {_controlledPawn}");
                }
            }
        }

        private bool _setViewportHUD = true, _setViewportCamera = true;

        /// <summary>
        /// Determines if this local player controller should update
        /// its viewport with the possessed pawn's HUD.
        /// </summary>
        [Category("Local Player Controller")]
        public bool InheritControlledPawnHUD
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
        /// its viewport with the possessed pawn's camera.
        /// </summary>
        [Category("Local Player Controller")]
        public bool InheritControlledPawnCamera
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
        private void UpdateViewport(bool setHUD, bool setCamera)
        {
            if (_viewport is null)
                return;

            if (setHUD)
                _viewport.AttachedHUD = _controlledPawn?.HUD?.File;

            if (setCamera)
                _viewport.AttachedCamera = _controlledPawn?.CurrentCameraComponent?.Camera;
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
            _input.InputRegistration -= RegisterInput;
        }
        public void UnlinkControlledPawn()
        {
            _possessionQueue.Clear();
            ControlledPawn = null;
        }
    }
}
