using TheraEngine.Players;
using TheraEngine.Actors;
using System.Collections.Generic;

namespace TheraEngine.Input
{
    public class PlayerController : PawnController
    {
        public int ServerPlayerIndex => PlayerInfo != null ? PlayerInfo.ServerIndex : -1;

        public PlayerInfo PlayerInfo { get; set; }

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
