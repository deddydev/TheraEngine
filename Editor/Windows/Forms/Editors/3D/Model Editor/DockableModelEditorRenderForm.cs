using System;
using System.Collections.Generic;
using TheraEditor.Actors.Types.Pawns;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableModelEditorRenderForm : DockContent, IEditorRenderableControl
    {
        public DockableModelEditorRenderForm(ELocalPlayerIndex playerIndex, int formIndex, ModelEditorForm form)
        {
            ModelWindow = form;
            FormIndex = formIndex;
            PlayerIndex = playerIndex;

            InitializeComponent();

            GameMode = new EditorGameMode();
            EditorPawn = new EditorCameraPawn(PlayerIndex)
            {
                MouseTranslateSpeed = 0.02f,
                ScrollSpeed = 0.5f,
                GamepadTranslateSpeed = 15.0f,

                HUD = new EditorUI3D(RenderPanel.ClientSize),
                Name = $"ModelViewport{(FormIndex + 1).ToString()}_EditorCamera",
            };
            RenderPanel.ValidPlayerIndices = new List<ELocalPlayerIndex>() { playerIndex };
            RenderPanel.Owner = this;

            Text = $"Model Viewport {(FormIndex + 1).ToString()}";

            RenderPanel.AllowDrop = true;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
            //RenderPanel.LostFocus += RenderPanel_LostFocus;
        }
        
        private void RenderPanel_GotFocus(object sender, EventArgs e)
            => Editor.SetActiveEditorControl(this);
        
        //private void RenderPanel_LostFocus(object sender, EventArgs e)
        //{
        //    if (Editor.ActiveRenderForm == this)
        //        Editor.SetActiveEditorControl(null);
        //}

        public EditorGameMode GameMode { get; set; }
        public ModelEditorForm ModelWindow { get; private set; }
        public int FormIndex { get; private set; }
        public ELocalPlayerIndex PlayerIndex { get; private set; } = ELocalPlayerIndex.One;
        public EditorCameraPawn EditorPawn { get; private set; }

        ELocalPlayerIndex IEditorRenderableControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanel;
        IPawn IEditorRenderableControl.EditorPawn => EditorPawn;
        IGameMode IEditorRenderableControl.GameMode => GameMode;
        IWorld IEditorRenderableControl.World => ModelWindow.World;

        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //    base.OnHandleDestroyed(e);
        //    if (Editor.ActiveRenderForm == this)
        //    {
        //        Editor.SetActiveEditorControl(null);
        //        Form.World.CurrentGameMode = null;
        //    }
        //}
        protected override void OnShown(EventArgs e)
        {
            ModelWindow.World.SpawnActor(EditorPawn);
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            ModelWindow.World.DespawnActor(EditorPawn);
            base.OnClosed(e);
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

        protected override string GetPersistString()
            => GetType().ToString() + "," + FormIndex;

        public void AlignView(BoundingBox aabb)
        {
            //Get aspect of the front plane of the aabb
            aabb.GetCorners(out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            float w = Math.Abs(TFR.X - TFL.X);
            float h = Math.Abs(TFR.Y - BFR.Y);
            float aspect = w / h;

            //Get the aspect of the camera
            PerspectiveCamera cam = EditorPawn.Camera as PerspectiveCamera;
            float cAspect = cam.Width / cam.Height;

            //Start the camera off at the center of the aabb
            Vec3 pos = aabb.Translation;

            //Move the camera back to start at the front plane of the aabb
            pos.Z += aabb.HalfExtents.Z;
            
            //Move the camera back to show all of the front plane rectangle
            pos.Z += aspect > cAspect ? cam.FrustumDistanceAtWidth(w) : cam.FrustumDistanceAtHeight(h);

            EditorPawn.RootComponent.Translation = pos;
        }

        #region Drag Drop
        //BaseFileWrapper _lastDraggedNode = null;
        //FileObject _dragInstance = null;
        ////private float _preRenderFreq, _preUpdateFreq;
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
        //    FileObject instance = _dragInstance ?? (_dragInstance = wrapper.GetNewInstance());
        //    if (instance is IActor actor)
        //    {
        //        //Editor.Instance.DoEvents = false;
        //        //_preRenderFreq = Engine.TargetRenderFreq;
        //        //_preUpdateFreq = Engine.TargetUpdateFreq;
        //        //Engine.TargetRenderFreq = 20.0f;
        //        //Engine.TargetUpdateFreq = 20.0f;
        //        BaseRenderPanel.HoveredPanel = RenderPanel;
        //        EditorHud hud = EditorPawn.HUD as EditorHud;
        //        Engine.World.SpawnActor(actor/*, EditorPawn.RootComponent.GetWorldPoint() +
        //            EditorPawn.RootComponent.GetForwardDir() * hud._hitDistance*/);
        //        hud.SetTransformMode(TransformType.DragDrop);
        //        hud.SetSelectedComponent(true, actor.RootComponent);
        //    }
        //}

        //private void RenderPanel_DragLeave(object sender, EventArgs e)
        //{
        //    EditorHud hud = EditorPawn.HUD as EditorHud;
        //    if (hud.DragComponent != null)
        //    {
        //        Engine.World.DespawnActor(hud.DragComponent.OwningActor);
        //        hud.DoMouseUp();
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
        //    }
        //    //Engine.TargetUpdateFreq = _preUpdateFreq;
        //    //Engine.TargetRenderFreq = _preRenderFreq;
        //    //Editor.Instance.DoEvents = true;
        //}
        #endregion
    }
}
