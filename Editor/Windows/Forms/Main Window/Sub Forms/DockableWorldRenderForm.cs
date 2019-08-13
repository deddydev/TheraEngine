using System;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Windows.Forms;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWorldRenderForm : DockableWorldRenderFormBase<EditorWorldRenderPanel>
    {
        public DockableWorldRenderForm(ELocalPlayerIndex playerIndex, int formIndex) : base(playerIndex, formIndex)
        {
            RenderPanel.AllowDrop = true;
            RenderPanel.DragDrop += RenderPanel_DragDrop;
            RenderPanel.DragEnter += RenderPanel_DragEnter;
            RenderPanel.DragOver += RenderPanel_DragOver;
            RenderPanel.DragLeave += RenderPanel_DragLeave;

            InitializeComponent();
            Controls.Add(RenderPanel);
            Text = $"Viewport {(FormIndex + 1).ToString()}";

            Engine.Instance.PreWorldChanged += Engine_WorldPreChanged;
            Engine.Instance.PostWorldChanged += Engine_WorldPostChanged;
        }
        protected override void OnShown(EventArgs e)
        {
            Editor.Instance.EditorGameMode.TargetRenderPanels.Add(RenderPanel);
            base.OnShown(e);
        }
        protected override void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Engine.SetWorldPanel(RenderPanel, false);
            base.RenderPanel_GotFocus(sender, e);
        }
        private void Engine_WorldPreChanged()
        {
            if (World != null)
            {
                if (EditorPawn != null)
                    World.DespawnActor(EditorPawn);
            }
        }
        private void Engine_WorldPostChanged()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)Engine_WorldPostChanged, BaseRenderPanel.EPanelType.Rendering))
                return;

            if (World == null)
            {
                Text = $"Viewport {(FormIndex + 1).ToString()}";
            }
            else
            {
                World.SpawnActor(EditorPawn);
                Text = $"{World.Name} (Viewport {(FormIndex + 1).ToString()})";
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (Editor.ActiveRenderForm == this)
                Engine.SetWorldPanel(null, false);
        }

        #region Drag / Drop Actors

        BaseFileWrapper _lastDraggedNode = null;
        IFileObject _dragInstance = null;
        //private float _preRenderFreq, _preUpdateFreq;
        private TransformType _prevTransformType;
        private async void RenderPanel_DragEnter(object sender, DragEventArgs e)
        {
            BaseWrapper[] dragNodes = Editor.Instance.ContentTree?.DraggedNodes;

            if (dragNodes == null || dragNodes.Length != 1)
                return;

            if (!(dragNodes[0] is BaseFileWrapper wrapper))
                return;

            if (_lastDraggedNode != wrapper)
            {
                _lastDraggedNode = wrapper;
                _dragInstance = null;
            }

            IFileObject instance = _dragInstance ?? (_dragInstance = await wrapper.GetNewInstanceAsync());
            if (!(instance is BaseActor actor))
                return;

            //Editor.Instance.DoEvents = false;
            //_preRenderFreq = Engine.TargetRenderFreq;
            //_preUpdateFreq = Engine.TargetUpdateFreq;
            //Engine.TargetRenderFreq = 20.0f;
            //Engine.TargetUpdateFreq = 20.0f;

            Engine.Instance.HoveredPanel = RenderPanel.Context;
            RenderPanel.Focus();
            EditorUI3D hud = EditorPawn.HUD.File as EditorUI3D;
            IMap map = World.Settings.FindOrCreateMap(World.Settings.NewActorTargetMapName);
            map.Actors.Add(actor);
            Vec3 point = EditorPawn.CameraComp.WorldPoint + EditorPawn.Camera.ForwardVector * hud.DraggingTestDistance;
            World.SpawnActor(actor, point);
            _prevTransformType = hud.TransformMode;
            hud.TransformMode = TransformType.DragDrop;
            hud.HighlightedComponent = actor.RootComponentGeneric;
            hud.DoMouseDown();
        }

        private void RenderPanel_DragLeave(object sender, EventArgs e)
        {
            EditorUI3D hud = EditorPawn.HUD.File as EditorUI3D;
            if (hud?.DragComponent is null)
                return;

            World.DespawnActor(hud.DragComponent.OwningActor);
            hud.DoMouseUp();
            hud.TransformMode = _prevTransformType;
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }

        private void RenderPanel_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            //RenderPanel.Invalidate();
        }

        private void RenderPanel_DragDrop(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            _dragInstance = null;
            _lastDraggedNode = null;

            EditorUI3D hud = EditorPawn.HUD?.File as EditorUI3D;
            if (hud?.DragComponent is null)
                return;

            hud.DoMouseUp();
            hud.TransformMode = _prevTransformType;
            //Engine.TargetUpdateFreq = _preUpdateFreq;
            //Engine.TargetRenderFreq = _preRenderFreq;
            //Editor.Instance.DoEvents = true;
        }

        #endregion
    }
}
