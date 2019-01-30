//using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Input;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableHudGraph : DockContent, IEditorControl
    {
        public DockableHudGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            GameMode = new HudEditorGameMode() { RenderPanel = RenderPanel };
        }
        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Editor.SetActiveEditorControl(this);
        }

        public HudEditorGameMode GameMode { get; set; }

        ELocalPlayerIndex IEditorControl.PlayerIndex => ELocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorControl.GameMode => GameMode;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm == this)
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
    }
    public class HudEditorController : LocalPlayerController
    {
        public HudEditorController(ELocalPlayerIndex index) : this(index, null) { }
        public HudEditorController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class HudGraphRenderPanel : UIRenderPanel<UIHudEditor, HudEditorGameMode, HudEditorController> { }
    public class HudEditorGameMode : UIGameMode<UIHudEditor, HudEditorController> { }
}
