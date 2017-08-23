using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Worlds.Actors;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<FlyingCameraPawn, LocalPlayerController>
    {
        protected override void HandleLocalPlayerJoined(LocalPlayerController item)
        {
            if (item.LocalPlayerIndex != PlayerIndex.One)
                return;

            RenderPanel p = RenderPanel.GamePanel;
            if (p != null)
            {
                Viewport v = p.GetViewport(0);
                if (v != null)
                    v.Owner = item;
            }

            FlyingCameraPawn pawn = _pawnClass.CreateNew();
            if (item.ControlledPawn == null)
                item.ControlledPawn = pawn;
            else
                item.EnqueuePosession(pawn);
            Engine.World.SpawnActor(pawn);
        }
        protected override void HandleLocalPlayerLeft(LocalPlayerController item)
        {
            base.HandleLocalPlayerLeft(item);
        }
    }
}
