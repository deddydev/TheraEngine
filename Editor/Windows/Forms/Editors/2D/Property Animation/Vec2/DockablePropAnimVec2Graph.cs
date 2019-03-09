using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using TheraEngine.Input;
using TheraEngine.Worlds;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimVec2))]
    public partial class DockablePropAnimVec2Graph : DockableRenderableFileEditor
    {
        public DockablePropAnimVec2Graph()
        {
            InitializeComponent();
            
            GameMode = new PropAnimVec2EditorGameMode() { RenderPanel = RenderPanel };

            tsPropAnimFloat.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimFloat.Renderer = new TheraToolstripRenderer();
        }
        public DockablePropAnimVec2Graph(PropAnimVec2 anim) : this() => TargetAnimation = anim;
        
        public override IPawn EditorPawn => RenderPanel.UI;
        public override World World => RenderPanel.World;
        protected override TFileObject File => TargetAnimation;
        protected override IUIRenderPanel RenderPanelGeneric => RenderPanel;
        public override bool ShouldHideCursor => true;

        public PropAnimVec2 TargetAnimation
        {
            get => RenderPanel.UI.TargetAnimation;
            internal set
            {
                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
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
        private void chkSnapToUnits_Click(object sender, EventArgs e)
        {
            chkSnapToUnits.Checked = !chkSnapToUnits.Checked;
            RenderPanel.UI.SnapToUnits = chkSnapToUnits.Checked;
        }
    }
    public class PropAnimVec2PlayerController : LocalPlayerController
    {
        public PropAnimVec2PlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimVec2PlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimVec2GraphRenderPanel : UIRenderPanel<EditorUIPropAnimVec2, PropAnimVec2EditorGameMode, PropAnimVec2PlayerController> { }
    public class PropAnimVec2EditorGameMode : UIGameMode<EditorUIPropAnimVec2, PropAnimVec2PlayerController> { }
}
