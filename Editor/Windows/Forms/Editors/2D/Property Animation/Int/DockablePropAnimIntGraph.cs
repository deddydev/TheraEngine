using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Input;
using TheraEngine.Worlds;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimInt))]
    public partial class DockablePropAnimIntGraph : DockableRenderableFileEditor<PropAnimInt>
    {
        public DockablePropAnimIntGraph()
        {
            InitializeComponent();
            
            tsPropAnimInt.RenderMode = ToolStripRenderMode.Professional;
            tsPropAnimInt.Renderer = new TheraToolstripRenderer();
        }
        public DockablePropAnimIntGraph(PropAnimInt anim) : this() => File = anim;

        public override IPawn EditorPawn => RenderPanel.UI;
        public override IWorld World => RenderPanel.World;
        protected override IUIRenderPanel RenderPanelGeneric => RenderPanel;
        public override bool ShouldHideCursor => true;
        
        public override PropAnimInt File
        {
            get => base.File;
            set
            {
                base.File = value;

                RenderPanel.UI.TargetAnimation = value;
                if (value != null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = value;
            }
        }
        private void btnZoomExtents_Click(object sender, EventArgs e)
            => RenderPanel.UI.ZoomExtents();
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
    public class PropAnimIntPlayerController : LocalPlayerController
    {
        public PropAnimIntPlayerController(ELocalPlayerIndex index) : this(index, null) { }
        public PropAnimIntPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => SetViewportCamera = SetViewportHUD = false;
    }
    public class PropAnimIntGraphRenderPanel : UIRenderPanel<EditorUIPropAnimInt, PropAnimIntEditorGameMode, PropAnimIntPlayerController> { }
    public class PropAnimIntEditorGameMode : UIGameMode<EditorUIPropAnimInt, PropAnimIntPlayerController> { }
}
