﻿using System;
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
    public partial class DockableUIGraph : DockableRenderableFileEditor<IUserInterface, UIGraphRenderHandler>
    {
        public DockableUIGraph()
        {
            InitializeComponent();

            tearOffToolStrip1.RenderMode = ToolStripRenderMode.Professional;
            tearOffToolStrip1.Renderer = new TheraToolstripRenderer();
        }
        public override bool ShouldHideCursor => false;
        public DockableUIGraph(IUserInterface ui) : this()
        {
            File = ui;
        }
        protected override bool TrySetFile(IUserInterface file)
        {
            if (!base.TrySetFile(file))
                return false;

            RenderPanel.RenderHandler.UI.TargetUI = file;
            if (file != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = file;

            return true;
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanel.RenderHandler.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            RenderPanel.RenderHandler.FormClosed();
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
            RenderPanel.RenderHandler.UI.ZoomExtents();
        }
    }
    public class UIEditorController : LocalPlayerController
    {
        public UIEditorController(ELocalPlayerIndex index) : this(index, null) { }
        public UIEditorController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => InheritControlledPawnCamera = InheritControlledPawnHUD = false;
    }
    public class UIGraphRenderHandler : UIRenderHandler<UIEditorUI, UIEditorGameMode, UIEditorController> { }
    public class UIEditorGameMode : UIGameMode<UIEditorUI, UIEditorController> { }
}
