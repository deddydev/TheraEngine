using System.Collections.Generic;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Actors;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace TheraEngine.Input
{
    public class ServerPlayerController : PlayerController
    {
        public static Dictionary<int, ConcurrentQueue<Camera>> CameraPossessionQueue = new Dictionary<int, ConcurrentQueue<Camera>>();
        
        public ServerPlayerController(int serverPlayerIndex, Queue<IPawn> possessionQueue = null) : base()
        {
            _input = new InputInterface(serverPlayerIndex);
            _input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0 && ControlledPawn == null)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(serverPlayerIndex))
            {
                Camera camera;
                while (!CameraPossessionQueue[serverPlayerIndex].TryDequeue(out camera)) ;
                ViewportCamera = camera;
            }
        }
        public ServerPlayerController(int serverPlayerIndex) : base()
        {
            _input = new InputInterface(serverPlayerIndex);
            _input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = new Queue<IPawn>();

            if (CameraPossessionQueue.ContainsKey(serverPlayerIndex))
            {
                Camera camera;
                while (!CameraPossessionQueue[serverPlayerIndex].TryDequeue(out camera)) ;
                ViewportCamera = camera;
            }
        }
        ~ServerPlayerController()
        {
            _input.WantsInputsRegistered -= RegisterInput;
            int index = PlayerInfo.ServerIndex;
            if (index >= 0 && index < Engine.LocalPlayers.Count)
                Engine.LocalPlayers.RemoveAt(index);
        }

        private Viewport _viewport;
        protected InputInterface _input;

        [Category("Server Player Controller")]
        public InputInterface Input => _input;
        
        [Category("Server Player Controller")]
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
                    if (_controlledPawn.HUD?.IsLoaded ?? false && _controlledPawn != _controlledPawn.HUD.File)
                        _input.WantsInputsRegistered -= _controlledPawn.HUD.File.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                //Engine.PrintLine("Assigned new controlled pawn to Player " + _serverPlayerIndex + ": " + (_controlledPawn == null ? "null" : _controlledPawn.GetType().GetFriendlyName()));

                if (_controlledPawn != null)
                {
                    UpdateViewport(SetViewportHUD, SetViewportCamera);

                    _input.WantsInputsRegistered += _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD?.IsLoaded ?? false && _controlledPawn != _controlledPawn.HUD.File)
                        _input.WantsInputsRegistered += _controlledPawn.HUD.File.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
        }

        private bool _setViewportHUD = true, _setViewportCamera = true;

        /// <summary>
        /// Determines if this local player controller should update
        /// its viewport with the currently possessed pawn's HUD.
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
        /// its viewport with the currently possessed pawn's camera.
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
            //Engine.TogglePause(ServerPlayerIndex);
        }
        public void SetPause(bool paused)
        {
            //Engine.SetPaused(paused, ServerPlayerIndex);
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
