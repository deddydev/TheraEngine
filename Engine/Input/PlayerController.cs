﻿using CustomEngine.Input.Devices;
using CustomEngine.Players;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds.Actors;
using System.Collections.Generic;

namespace CustomEngine.Input
{
    public class PlayerController : PawnController
    {
        PlayerInfo _playerInfo;

        public int ServerPlayerIndex { get { return _playerInfo != null ? _playerInfo.Index : -1; } }

        public PlayerController(Queue<Pawn> possessionQueue) : base(possessionQueue)
        {

        }
        public PlayerController() : base()
        {

        }
        ~PlayerController()
        {

        }
    }
}
