using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Input.Devices;

namespace TheraEngine.Input
{
    public class ServerPlayerController : PlayerController
    {
        public ServerPlayerController(int serverPlayerIndex, Queue<IPawn> possessionQueue = null) : base()
        {
            _input = new InputInterface(serverPlayerIndex);
            _input.InputRegistration += RegisterInput;

            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0 && ControlledPawn is null)
                ControlledPawn = _possessionQueue.Dequeue();
        }
        public ServerPlayerController(int serverPlayerIndex) : base()
        {
            _input = new InputInterface(serverPlayerIndex);
            _input.InputRegistration += RegisterInput;

            _possessionQueue = new Queue<IPawn>();
        }

        protected InputInterface _input;

        [Category("Server Player Controller")]
        public InputInterface Input => _input;
        
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

                    _input.InputRegistration -= _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD?.IsLoaded ?? false && _controlledPawn != _controlledPawn.HUD.File)
                        _input.InputRegistration -= _controlledPawn.HUD.File.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn is null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                //Engine.PrintLine("Assigned new controlled pawn to Player " + _serverPlayerIndex + ": " + (_controlledPawn is null ? "null" : _controlledPawn.GetType().GetFriendlyName()));

                if (_controlledPawn != null)
                {
                    _input.InputRegistration += _controlledPawn.RegisterInput;
                    if (_controlledPawn.HUD?.IsLoaded ?? false && _controlledPawn != _controlledPawn.HUD.File)
                        _input.InputRegistration += _controlledPawn.HUD.File.RegisterInput;

                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
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
            _input.InputRegistration -= RegisterInput;
        }
        internal void UnlinkControlledPawn()
        {
            _possessionQueue.Clear();
            ControlledPawn = null;
        }
    }
}
