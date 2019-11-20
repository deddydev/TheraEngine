using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Input;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimVec2))]
    public partial class DockablePropAnimVec2Graph : DockableRenderableFileEditor<PropAnimVec2, PropAnimVec2GraphRenderHandler>
    {
        public DockablePropAnimVec2Graph()
        {
            InitializeComponent();

            tsPropAnimVec2.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimVec2.Renderer = new TheraToolStripRenderer();
        }
        public DockablePropAnimVec2Graph(PropAnimVec2 anim) : this() => File = anim;

        public override bool ShouldHideCursor => true;

        protected override bool TrySetFile(PropAnimVec2 file)
        {
            if (!base.TrySetFile(file))
                return false;

            RenderPanel.RenderHandler.UI.TargetAnimation = file;
            if (file != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = file;

            return true;
        }

        private void btnZoomExtents_Click(object sender, EventArgs e)
            => RenderPanel.RenderHandler.UI.ZoomExtents();

        private void chkAutoTangents_Click(object sender, EventArgs e)
        {
            chkAutoTangents.Checked = !chkAutoTangents.Checked;
            RenderPanel.RenderHandler.UI.AutoGenerateTangents = chkAutoTangents.Checked;
        }

        private void chkSnapToUnits_Click(object sender, EventArgs e)
        {
            chkSnapToUnits.Checked = !chkSnapToUnits.Checked;
            RenderPanel.RenderHandler.UI.SnapToUnits = chkSnapToUnits.Checked;
        }
    }
    public class PropAnimVec2PlayerController : LocalPlayerController
    {
        public PropAnimVec2PlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimVec2PlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => InheritControlledPawnCamera = InheritControlledPawnHUD = false;
    }
    public class PropAnimVec2GraphRenderHandler : UIRenderHandler<EditorUIPropAnimVec2, PropAnimVec2EditorGameMode, PropAnimVec2PlayerController> { }
    public class PropAnimVec2EditorGameMode : UIGameMode<EditorUIPropAnimVec2, PropAnimVec2PlayerController> { }
}
