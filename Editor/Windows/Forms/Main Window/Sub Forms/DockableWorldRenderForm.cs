using System;
using System.Windows.Forms;
using TheraEditor.Actors.Types.Pawns;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.GameModes;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWorldRenderForm : DockContent, IEditorRenderableControl
    {
        public DockableWorldRenderForm(ELocalPlayerIndex playerIndex, int formIndex)
        {
            FormIndex = formIndex;
            PlayerIndex = playerIndex;

            InitializeComponent();

            Text = $"Viewport {(FormIndex + 1).ToString()}";

            EditorPawn = new EditorCameraPawn(PlayerIndex)
            {
                HUD = new EditorUI3D(RenderPanel.ClientSize),
                Name = $"Viewport{(FormIndex + 1).ToString()}_EditorCamera"
            };

            Engine.PreWorldChanged += Engine_WorldPreChanged;
            Engine.PostWorldChanged += Engine_WorldPostChanged;

            RenderPanel.AllowDrop = true;
            RenderPanel.GotFocus += RenderPanel_GotFocus;
        }
        
        private void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Engine.SetWorldPanel(RenderPanel, false);
            Editor.SetActiveEditorControl(this);
        }

        private void Engine_WorldPreChanged()
        {
            if (Engine.World != null && EditorPawn != null)
                Engine.World.DespawnActor(EditorPawn);
        }
        private void Engine_WorldPostChanged()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)Engine_WorldPostChanged, BaseRenderPanel.EPanelType.Rendering))
                return;

            if (Engine.World == null || EditorPawn == null)
            {
                Text = $"Viewport {(FormIndex + 1).ToString()}";
            }
            else
            {
                Engine.World.SpawnActor(EditorPawn);
                Text = $"{Engine.World.Name} (Viewport {(FormIndex + 1).ToString()})";
            }
        }
        
        public int FormIndex { get; private set; }
        public ELocalPlayerIndex PlayerIndex { get; private set; } = ELocalPlayerIndex.One;
        public EditorCameraPawn EditorPawn { get; private set; }

        ELocalPlayerIndex IEditorRenderableControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanel;
        IPawn IEditorRenderableControl.EditorPawn => EditorPawn;
        BaseGameMode IEditorRenderableControl.GameMode => Engine.World?.CurrentGameMode;
        World IEditorRenderableControl.World => Engine.World;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Editor.ActiveRenderForm != this)
                return;
            Engine.SetWorldPanel(null, false);
            Editor.SetActiveEditorControl(null);
        }
        protected override void OnShown(EventArgs e)
        {
            Engine.World?.SpawnActor(EditorPawn);
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            Engine.World?.DespawnActor(EditorPawn);
            base.OnClosed(e);
        }

        protected override string GetPersistString()
            => GetType() + "," + FormIndex;

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

            BaseRenderPanel.HoveredPanel = RenderPanel;
            RenderPanel.Focus();
            EditorUI3D hud = EditorPawn.HUD.File as EditorUI3D;
            Map map = Engine.World.Settings.FindOrCreateMap(Engine.World.Settings.NewActorTargetMapName);
            map.Actors.Add(actor);
            Vec3 point = EditorPawn.CameraComp.WorldPoint + EditorPawn.Camera.ForwardVector * hud.DraggingTestDistance;
            Engine.World.SpawnActor(actor, point);
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

            Engine.World.DespawnActor(hud.DragComponent.OwningActor);
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

            EditorUI3D hud = EditorPawn.HUD.File as EditorUI3D;
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
