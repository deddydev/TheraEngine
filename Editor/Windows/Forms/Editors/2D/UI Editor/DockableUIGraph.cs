using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(IUserInterface))]
    public partial class DockableUIGraph : DockContent, IEditorRenderableControl
    {
        public DockableUIGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;

            GameMode = new UIEditorGameMode() { RenderPanel = RenderPanel };

            tearOffToolStrip1.RenderMode = ToolStripRenderMode.Professional;
            tearOffToolStrip1.Renderer = new TheraToolstripRenderer();
        }
        public DockableUIGraph(IUserInterface ui) : this()
        {
            TargetUI = ui;
        }
        public IUserInterface TargetUI
        {
            get => RenderPanel.UI.TargetUI;
            internal set
            {
                RenderPanel.UI.TargetUI = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
        }
        private void RenderPanel_MouseEnter(object sender, EventArgs e) => Cursor.Hide();
        private void RenderPanel_MouseLeave(object sender, EventArgs e) => Cursor.Show();
        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            if (!Engine.DesignMode)
                Editor.SetActiveEditorControl(this);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (!Engine.DesignMode)
                Editor.SetActiveEditorControl(this);
        }

        public UIEditorGameMode GameMode { get; set; }

        ELocalPlayerIndex IEditorRenderableControl.PlayerIndex => ELocalPlayerIndex.One;
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanel;
        IPawn IEditorRenderableControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorRenderableControl.GameMode => GameMode;
        World IEditorRenderableControl.World => RenderPanel.World;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (!Engine.DesignMode && Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanel.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            RenderPanel.FormClosed();
            base.OnClosed(e);
        }
        //private void newUIElementToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UIComponent comp = Editor.UserCreateInstanceOf<UIComponent>();
        //    if (comp != null)
        //        RenderPanel.UI?.TargetHUD?.RootComponent?.ChildComponents?.Add(comp);
        //}
        //private void removeSelectedElementToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}
        private void btnZoomExtents_Click(object sender, EventArgs e)
        {
            RenderPanel.UI.ZoomExtents();
        }
    }
    public class UIEditorController : LocalPlayerController
    {
        public UIEditorController(ELocalPlayerIndex index) : this(index, null) { }
        public UIEditorController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class UIGraphRenderPanel : UIRenderPanel<UIEditorUI, UIEditorGameMode, UIEditorController> { }
    public class UIEditorGameMode : UIGameMode<UIEditorUI, UIEditorController> { }
}
