//using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering.UI;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableUIGraph : DockContent, IEditorControl
    {
        public DockableUIGraph()
        {
            InitializeComponent();

            RenderPanel.AllowDrop = false;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            GameMode = new UIEditorGameMode() { RenderPanel = RenderPanel };
        }
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

        ELocalPlayerIndex IEditorControl.PlayerIndex => ELocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => RenderPanel.UI;
        BaseGameMode IEditorControl.GameMode => GameMode;

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
        private void newUIElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIComponent comp = Editor.UserCreateInstanceOf<UIComponent>();
            if (comp != null)
                RenderPanel.UI.BaseTransformComponent.ChildComponents.Add(comp);
        }
        private void removeSelectedElementToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
