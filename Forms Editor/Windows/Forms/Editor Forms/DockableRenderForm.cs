using System;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types.Pawns;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableRenderForm : DockContent, IEditorControl
    {
        public DockableRenderForm(LocalPlayerIndex playerIndex, int formIndex)
        {
            FormIndex = formIndex;
            PlayerIndex = playerIndex;
            InitializeComponent();
            RenderPanel.AllowDrop = true;
        }
        
        public int FormIndex { get; private set; }
        public LocalPlayerIndex PlayerIndex { get; private set; } = LocalPlayerIndex.One;
        public FlyingCameraPawn EditorPawn { get; private set; }

        LocalPlayerIndex IEditorControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => EditorPawn;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm == this)
            {
                Engine.SetGamePanel(null, false);
                Editor.SetActiveEditorControl(null);
            }
        }
        protected override void OnShown(EventArgs e)
        {
            EditorPawn = new FlyingCameraPawn(PlayerIndex)
            {
                HUD = new EditorHud(RenderPanel.ClientSize)
            };
            Engine.World.SpawnActor(EditorPawn);

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
            Engine.SetGamePanel(RenderPanel, false);
            Editor.SetActiveEditorControl(this);
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

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + FormIndex;
        }

        BaseFileWrapper _lastDraggedNode = null;
        FileObject _dragInstance = null;
        private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            BaseWrapper[] dragNodes = Editor.Instance.ContentTree.DraggedNodes;
            if (dragNodes.Length != 1)
                return;
            BaseFileWrapper wrapper = dragNodes[0] as BaseFileWrapper;
            if (wrapper == null)
                return;
            if (_lastDraggedNode != wrapper)
            {
                _lastDraggedNode = wrapper;
                _dragInstance = null;
            }
            FileObject instance = _dragInstance ?? (_dragInstance = wrapper.GetNewInstance());
            if (instance is IActor actor)
            {
                Engine.World.SpawnActor(actor);
                EditorHud hud = EditorPawn.HUD as EditorHud;
                hud.HighlightedComponent = actor.RootComponent;
                hud.MouseDown();
            }
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            EditorHud hud = EditorPawn.HUD as EditorHud;
            if (hud.DragComponent != null)
            {
                Engine.World.DespawnActor(hud.DragComponent.OwningActor);
                hud.DragComponent = null;
                hud.MouseUp();
            }
        }

        private void RenderPanel_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        {
            _dragInstance = null;
            _lastDraggedNode = null;
        }
    }
}
