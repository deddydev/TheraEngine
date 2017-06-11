﻿using TheraEngine.Input.Devices;
using TheraEngine.Players;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds.Actors;
using System.Collections.Generic;

namespace TheraEngine.Input
{
    public class PlayerController : PawnController
    {
        PlayerInfo _playerInfo;

        public int ServerPlayerIndex => PlayerInfo != null ? PlayerInfo.Index : -1;

        public PlayerInfo PlayerInfo
        {
            get => _playerInfo;
            set => _playerInfo = value;
        }

        public PlayerController(Queue<IPawn> possessionQueue) : base(possessionQueue)
        {
            PlayerInfo = new PlayerInfo();
        }
        public PlayerController() : base()
        {
            PlayerInfo = new PlayerInfo();
        }
        ~PlayerController()
        {

        }
    }
}
