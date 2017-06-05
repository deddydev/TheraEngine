using System;
using System.Collections.Generic;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds.Actors;
using CustomEngine.GameModes;
using CustomEngine.Players;
using System.Collections.Concurrent;

namespace CustomEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        public static Dictionary<int, ConcurrentQueue<Camera>> CameraPossessionQueue = new Dictionary<int, ConcurrentQueue<Camera>>();

        public LocalPlayerController(Queue<IPawn> possessionQueue = null) : base()
        {
            int index = Engine.ActivePlayers.Count;
            _index = (PlayerIndex)index;
            Engine.ActivePlayers.Add(this);

            _input = new InputInterface(index);
            //_input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(index))
            {
                Camera camera;
                while (!CameraPossessionQueue[index].TryDequeue(out camera)) ;
                CurrentCamera = camera;
            }
        }
        public LocalPlayerController() : base()
        {
            int index = Engine.ActivePlayers.Count;
            _index = (PlayerIndex)index;
            Engine.ActivePlayers.Add(this);

            _input = new InputInterface(index);
            //_input.WantsInputsRegistered += RegisterInput;

            _possessionQueue = new Queue<IPawn>();
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();

            if (CameraPossessionQueue.ContainsKey(index))
            {
                Camera camera;
                while (!CameraPossessionQueue[index].TryDequeue(out camera)) ;
                CurrentCamera = camera;
            }
        }
        ~LocalPlayerController()
        {
            int index = (int)_index;
            if (index >= 0 && index < Engine.ActivePlayers.Count)
                Engine.ActivePlayers.RemoveAt(index);
        }

        private Viewport _viewport;
        private PlayerIndex _index;
        protected InputInterface _input;

        public InputInterface Input => _input;
        public PlayerIndex LocalPlayerIndex => (PlayerIndex)_viewport.Index;
        public Viewport Viewport
        {
            get => _viewport;
            set => _viewport = value;
        }
        public Camera CurrentCamera
        {
            get => _viewport.Camera;
            set => _viewport.Camera = value;
        }

        public override IPawn ControlledPawn
        {
            get => base.ControlledPawn;
            set
            {
                if (_controlledPawn != null)
                {
                    _controlledPawn.OnUnPossessed();
                    _input.TryUnregisterInput();
                    _input.WantsInputsRegistered -= _controlledPawn.RegisterInput;
                }

                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();

                if (_controlledPawn != null)
                {
                    _input.WantsInputsRegistered += _controlledPawn.RegisterInput;
                    _controlledPawn.OnPossessed(this);
                    _input.TryRegisterInput();
                }
            }
        }

        //protected virtual void RegisterInput(InputInterface input)
        //{
        //    input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause);
        //    input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause);
        //}
        //private void OnTogglePause()
        //{
        //    Engine.TogglePause(LocalPlayerIndex);
        //}
    }
}
