using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.GameModes;
using TheraEngine.Input;
using WeifenLuo.WinFormsUI.Docking;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimInt))]
    public partial class DockablePropAnimIntGraph : DockContent, IEditorControl
    {
        public DockablePropAnimIntGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;

            GameMode = new PropAnimIntEditorGameMode() { RenderPanel = RenderPanel };

            tearOffToolStrip1.RenderMode = ToolStripRenderMode.Professional;
            tearOffToolStrip1.Renderer = new TheraToolstripRenderer();
        }
        public DockablePropAnimIntGraph(PropAnimInt anim) : this()
        {
            TargetAnimation = anim;
        }

        public PropAnimIntEditorGameMode GameMode { get; set; }

        ELocalPlayerIndex IEditorControl.PlayerIndex => ELocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorControl.GameMode => GameMode;

        public PropAnimInt TargetAnimation
        {
            get => RenderPanel.UI.TargetAnimation;
            internal set
            {
                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
        }
        private void RenderPanel_MouseEnter(object sender, EventArgs e) => Cursor.Hide();
        private void RenderPanel_MouseLeave(object sender, EventArgs e) => Cursor.Show();
        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            if (TargetAnimation != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = TargetAnimation;
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
            base.OnHandleDestroyed(e);
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanel.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            RenderPanel.FormClosed();
            base.OnClosing(e);
        }
        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            RenderPanel.UI.ZoomExtents();
        }
        private void chkAutoTangents_Click(object sender, EventArgs e)
        {
            chkAutoTangents.Checked = !chkAutoTangents.Checked;
            RenderPanel.UI.AutoGenerateTangents = chkAutoTangents.Checked;
        }
        private void chkSnapToIncrement_Click(object sender, EventArgs e)
        {
            chkSnapToIncrement.Checked = !chkSnapToIncrement.Checked;
            RenderPanel.UI.SnapToIncrement = chkSnapToIncrement.Checked;
        }
    }
    public class PropAnimIntPlayerController : LocalPlayerController
    {
        public PropAnimIntPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimIntPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimIntGraphRenderPanel : UIRenderPanel<EditorUIPropAnimInt, PropAnimIntEditorGameMode, PropAnimIntPlayerController> { }
    public class PropAnimIntEditorGameMode : UIGameMode<EditorUIPropAnimInt, PropAnimIntPlayerController> { }
}
