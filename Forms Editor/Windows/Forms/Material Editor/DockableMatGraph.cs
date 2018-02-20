using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Shapes;
using TheraEngine.Timers;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMatGraph : DockContent, IEditorControl
    {
        public DockableMatGraph()
        {
            InitializeComponent();
            EditorPawn = new UIMaterialEditor(RenderPanel.ClientSize)
            {
                HUD = new EditorHud(RenderPanel.ClientSize),
                Name = "MaterialEditorPawn",
            };
            RenderPanel.AllowDrop = true;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
        }

        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
        }
        
        public UIMaterialEditor EditorPawn { get; private set; }

        LocalPlayerIndex IEditorControl.PlayerIndex => LocalPlayerIndex.One;
        BaseRenderPanel IEditorControl.RenderPanel => RenderPanel;
        IPawn IEditorControl.EditorPawn => EditorPawn;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            Engine.World?.SpawnActor(EditorPawn);
            base.OnShown(e);
        }

        private void RenderTick(object sender, FrameEventArgs e)
        {
            RenderPanel.Invalidate();
            Application.DoEvents();
        }

        protected override void OnClosed(EventArgs e)
        {
            Engine.World?.DespawnActor(EditorPawn);
            base.OnClosed(e);
        }

        public void AlignView(BoundingRectangle bounds)
        {
            ////Get aspect of the front plane of the aabb
            //aabb.GetCorners(out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            //float w = Math.Abs(TFR.X - TFL.X);
            //float h = Math.Abs(TFR.Y - BFR.Y);
            //float aspect = w / h;

            ////Get the aspect of the camera
            //PerspectiveCamera cam = EditorPawn.Camera;
            //float cAspect = cam.Width / cam.Height;

            ////Start the camera off at the center of the aabb
            //Vec3 pos = aabb.Translation;

            ////Move the camera back to start at the front plane of the aabb
            //pos.Z += aabb.HalfExtents.Z;

            ////Move the camera back to show all of the front plane rectangle
            //pos.Z += aspect > cAspect ? cam.FrustumDistanceAtWidth(w) : cam.FrustumDistanceAtHeight(h);

            //EditorPawn.RootComponent.SetTranslation(pos);
        }


        //protected override void OnResizeBegin(EventArgs e)
        //{
        //    RenderPanel.BeginResize();
        //    base.OnResizeBegin(e);
        //}
        //protected override void OnResizeEnd(EventArgs e)
        //{
        //    RenderPanel.EndResize();
        //    base.OnResizeEnd(e);
        //}

        //BaseFileWrapper _lastDraggedNode = null;
        //TFileObject _dragInstance = null;
        ////private float _preRenderFreq, _preUpdateFreq;
        //private TransformType _prevTransformType;
        //private void RenderPanel_DragEnter(object sender, DragEventArgs e)
        //{
        //    BaseWrapper[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;
        //    if (dragNodes == null || dragNodes.Length != 1)
        //        return;
        //    BaseFileWrapper wrapper = dragNodes[0] as BaseFileWrapper;
        //    if (wrapper == null)
        //        return;
        //    if (_lastDraggedNode != wrapper)
        //    {
        //        _lastDraggedNode = wrapper;
        //        _dragInstance = null;
        //    }
        //    TFileObject instance = _dragInstance ?? (_dragInstance = wrapper.GetNewInstance());
        //    if (instance is IActor actor)
        //    {
        //        //Editor.Instance.DoEvents = false;
        //        //_preRenderFreq = Engine.TargetRenderFreq;
        //        //_preUpdateFreq = Engine.TargetUpdateFreq;
        //        //Engine.TargetRenderFreq = 20.0f;
        //        //Engine.TargetUpdateFreq = 20.0f;
        //        BaseRenderPanel.HoveredPanel = RenderPanel;
        //        RenderPanel.Focus();
        //        EditorHud hud = EditorPawn.HUD as EditorHud;
        //        Engine.World.SpawnActor(actor, EditorPawn.RootComponent.WorldPoint + EditorPawn.RootComponent.WorldForwardDir * 20.0f);
        //        _prevTransformType = hud.TransformMode;
        //        hud.TransformMode = TransformType.DragDrop;
        //        hud.HighlightedComponent = actor.RootComponent;
        //        hud.DoMouseDown();
        //    }
        //}

        //private void RenderPanel_DragLeave(object sender, EventArgs e)
        //{
        //    EditorHud hud = EditorPawn.HUD as EditorHud;
        //    if (hud.DragComponent != null)
        //    {
        //        Engine.World.DespawnActor(hud.DragComponent.OwningActor);
        //        hud.DoMouseUp();
        //        hud.TransformMode = _prevTransformType;
        //    }
        //    //Engine.TargetUpdateFreq = _preUpdateFreq;
        //    //Engine.TargetRenderFreq = _preRenderFreq;
        //    //Editor.Instance.DoEvents = true;
        //}

        //private void RenderPanel_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Move;
        //    //RenderPanel.Invalidate();
        //}

        //private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        //{
        //    DragHelper.ImageList_DragLeave(Handle);
        //    _dragInstance = null;
        //    _lastDraggedNode = null;
        //    EditorHud hud = EditorPawn.HUD as EditorHud;
        //    if (hud.DragComponent != null)
        //    {
        //        hud.DoMouseUp();
        //        hud.TransformMode = _prevTransformType;
        //    }
        //    //Engine.TargetUpdateFreq = _preUpdateFreq;
        //    //Engine.TargetRenderFreq = _preRenderFreq;
        //    //Editor.Instance.DoEvents = true;
        //}
    }
}
