using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public abstract class DockableRenderableFileEditor : DockableFileEditor, IEditorRenderableControl
    {
        public virtual ELocalPlayerIndex PlayerIndex { get; } = ELocalPlayerIndex.One;
        public BaseGameMode GameMode { get; protected set; }
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanelGeneric as BaseRenderPanel;

        public abstract IPawn EditorPawn { get; }
        public abstract World World { get; }
        public abstract bool ShouldHideCursor { get; }

        protected abstract IUIRenderPanel RenderPanelGeneric { get; }
        
        protected void RenderPanel_MouseEnter(object sender, EventArgs e) => Cursor.Hide();
        protected void RenderPanel_MouseLeave(object sender, EventArgs e) => Cursor.Show();
        protected void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            if (File != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = File;
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
            base.OnHandleDestroyed(e);
        }
        protected override void OnShown(EventArgs e)
        {
            var rp = RenderPanelGeneric as BaseRenderPanel;
            rp.GotFocus += RenderPanel_GotFocus;
            if (ShouldHideCursor)
            {
                rp.MouseEnter += RenderPanel_MouseEnter;
                rp.MouseLeave += RenderPanel_MouseLeave;
            }

            RenderPanelGeneric.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            var rp = RenderPanelGeneric as BaseRenderPanel;
            rp.GotFocus -= RenderPanel_GotFocus;
            if (ShouldHideCursor)
            {
                rp.MouseEnter -= RenderPanel_MouseEnter;
                rp.MouseLeave -= RenderPanel_MouseLeave;
            }

            RenderPanelGeneric.FormClosed();
            base.OnClosing(e);
        }
    }
}
