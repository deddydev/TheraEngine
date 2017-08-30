using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableRenderForm : DockContent
    {
        public DockableRenderForm(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            InitializeComponent();
            RenderPanel.AllowDrop = true;
        }

        public PlayerIndex PlayerIndex { get; private set; }
        public FlyingCameraPawn EditorPawn { get; private set; }

        protected override void OnShown(EventArgs e)
        {
            EditorPawn = new FlyingCameraPawn(PlayerIndex) { Hud = new EditorHud(RenderPanel.ClientSize) };
            Engine.World.SpawnActor(EditorPawn);
            Viewport v = RenderPanel.AddViewport();
            v.Camera = EditorPawn.CurrentCameraComponent.Camera;
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            if (Engine.ActivePlayers.Count == 0)
                return;
            LocalPlayerController c = Engine.ActivePlayers[0];
            Viewport v = RenderPanel?.GetViewport(0);
            if (v != null)
                v.Owner = c;
            c.ControlledPawn = EditorPawn;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }
        protected override void OnResizeBegin(EventArgs e)
        {
            RenderPanel.BeginResize();
            base.OnResizeBegin(e);
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            RenderPanel.EndResize();
            base.OnResizeEnd(e);
        }

        private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            BaseFileWrapper file = Editor.Instance.ContentTree.DragNode as BaseFileWrapper;
            if (file == null)
                return;

            FileObject instance = file.FileObject;
            if (instance is IActor actor)
            {
                if (!actor.IsSpawned)
                {
                    Engine.World.SpawnActor(actor);
                    EditorHud hud = EditorPawn.Hud as EditorHud;
                    hud.HighlightedComponent = actor.RootComponent;
                    hud.MouseDown();
                }
            }
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            EditorHud hud = EditorPawn.Hud as EditorHud;
            if (hud.DragComponent != null)
            {
                Engine.World.DespawnActor(hud.DragComponent.OwningActor);
                hud.DragComponent = null;
                hud.MouseUp();
            }
        }
    }
}
