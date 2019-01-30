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

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimFloat))]
    public partial class DockablePropAnimFloatGraph : DockContent, IEditorControl
    {
        public DockablePropAnimFloatGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;
            GameMode = new PropAnimFloatEditorGameMode() { RenderPanel = RenderPanel };
        }
        public DockablePropAnimFloatGraph(PropAnimFloat anim) : this()
        {
            TargetAnimation = anim;
        }

        public PropAnimFloatEditorGameMode GameMode { get; set; }

        ELocalPlayerIndex IEditorControl.PlayerIndex => ELocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorControl.GameMode => GameMode;

        public PropAnimFloat TargetAnimation
        {
            get => RenderPanel.UI.TargetAnimation;
            internal set
            {
                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
        }

        private void RenderPanel_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }
        private void RenderPanel_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }
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
    }
    public class PropAnimFloatPlayerController : LocalPlayerController
    {
        public PropAnimFloatPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimFloatPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimFloatGraphRenderPanel : UIRenderPanel<UIPropAnimFloatEditor, PropAnimFloatEditorGameMode, PropAnimFloatPlayerController> { }
    public class PropAnimFloatEditorGameMode : UIGameMode<UIPropAnimFloatEditor, PropAnimFloatPlayerController> { }
}
