using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Tests;
using TheraEngine.Worlds.Actors;

namespace TheraEditor
{
    public class EditorGameMode : GameMode<FlyingCameraPawn, LocalPlayerController>
    {
        protected override void HandleLocalPlayerJoined(LocalPlayerController item)
        {
            if (item.LocalPlayerIndex != PlayerIndex.One)
                return;

            Viewport v = DockableRenderForm.ActiveRenderForm?.RenderPanel?.GetViewport(0);
            if (v != null)
                v.Owner = item;
            
            //RenderPanel p = RenderPanel.GamePanel;
            //if (p != null)
            //{
            //    Viewport v = p.GetViewport(0) ?? p.AddViewport();
            //    if (v != null)
            //        v.Owner = item;
            //}

            //FlyingCameraPawn pawn = _pawnClass.CreateNew();
            //pawn.Hud = new EditorHud(item.Viewport.Region.Bounds);
            //if (item.ControlledPawn == null)
            //    item.ControlledPawn = pawn;
            //else
            //    item.EnqueuePosession(pawn);
            //Engine.World.SpawnActor(pawn);

        }
        protected override void HandleLocalPlayerLeft(LocalPlayerController item)
        {
            base.HandleLocalPlayerLeft(item);
        }

        public override void BeginGameplay()
        {
            Engine.World.SpawnActor(new TestCharacter(), new Vec3(-5.0f, 50.0f, -5.0f));
            base.BeginGameplay();
        }
    }
}
