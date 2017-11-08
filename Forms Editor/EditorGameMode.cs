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

            DockableRenderForm form = DockableRenderForm.ActiveRenderForm;
            if (form != null)
            {
                item.ControlledPawn = form.EditorPawn;
                form.RenderPanel.GetViewport(0)?.RegisterController(item);
            }
        }

        protected override void HandleLocalPlayerLeft(LocalPlayerController item)
        {
            base.HandleLocalPlayerLeft(item);
        }

        public override void BeginGameplay()
        {
            //Engine.World.SpawnActor(new TestCharacter(), new Vec3(-5.0f, 50.0f, -5.0f));
            base.BeginGameplay();
        }
    }
}
