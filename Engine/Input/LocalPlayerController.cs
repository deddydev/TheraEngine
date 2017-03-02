﻿using System;
using System.Collections.Generic;
using CustomEngine.Input.Devices;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Input
{
    public class LocalPlayerController : PlayerController
    {
        private Viewport _viewport;
        private PlayerIndex _index;
        protected InputInterface _input;

        public InputInterface Input => _input;
        public PlayerIndex LocalPlayerIndex => (PlayerIndex)_viewport.Index;
        public Viewport Viewport
        {
            get => _viewport;
            internal set => _viewport = value;
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
        public LocalPlayerController(Queue<IPawn> possessionQueue = null) : base()
        {
            int index = Engine.ActivePlayers.Count;
            _index = (PlayerIndex)index;
            _input = new InputInterface(index);
            Engine.ActivePlayers.Add(this);
            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();
        }
        public LocalPlayerController()
        {
            int index = Engine.ActivePlayers.Count;
            _index = (PlayerIndex)index;
            _input = new InputInterface(index);
            Engine.ActivePlayers.Add(this);
            _possessionQueue = new Queue<IPawn>();
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();
        }
        ~LocalPlayerController()
        {
            if (Engine.ActivePlayers.Contains(this))
                Engine.ActivePlayers.Remove(this);
        }
    }
}
