using System;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Timers;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using WeifenLuo.WinFormsUI.Docking;
using TheraEngine.Actors.Types;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class ModelEditorForm : TheraForm
    {
        public ModelEditorForm()
        {
            InitializeComponent();
        }

        #region Instanced Dock Forms
        //Dockable forms with a limited amount of instances
        private DockableModelEditorRenderForm[] _renderForms = new DockableModelEditorRenderForm[4];
        public bool RenderFormActive(int i)
        {
            DockableModelEditorRenderForm form = _renderForms[i];
            return form != null && !form.IsDisposed;
        }
        public DockableModelEditorRenderForm GetRenderForm(int i)
        {
            DockableModelEditorRenderForm form = _renderForms[i];
            if (form == null || form.IsDisposed)
            {
                Engine.PrintLine("Created model editor viewport " + (i + 1).ToString());
                form = _renderForms[i] = new DockableModelEditorRenderForm(LocalPlayerIndex.One, i, this);
                form.Show(DockPanel, DockState.Document);
            }
            return form;
        }
        public DockableModelEditorRenderForm RenderForm1 => GetRenderForm(0);
        public DockableModelEditorRenderForm RenderForm2 => GetRenderForm(1);
        public DockableModelEditorRenderForm RenderForm3 => GetRenderForm(2);
        public DockableModelEditorRenderForm RenderForm4 => GetRenderForm(3);

        //public static GlobalFileRef<ModelEditorSettings> GetSettingsRef() => Instance.Project?.EditorSettingsRef ?? DefaultSettingsRef;
        //public static ModelEditorSettings GetSettings() => GetSettingsRef()?.File;
        
        public T GetForm<T>(ref T value) where T : DockContent, new()
        {
            if (value == null || value.IsDisposed)
            {
                value = new T();
                Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(DockPanel);
            }
            return value;
        }
        
        private DockableBoneTree _boneTreeForm;
        public DockableBoneTree BoneTreeForm => GetForm(ref _boneTreeForm);
        
        private DockableMeshList _meshesForm;
        public DockableMeshList MeshesForm => GetForm(ref _meshesForm);
        
        private DockablePropertyGrid _propertyGridForm;
        public DockablePropertyGrid PropertyGridForm => GetForm(ref _propertyGridForm);
        
        #endregion
        
        public ModelEditorWorld World { get; private set; } = new ModelEditorWorld();

        private Actor<StaticMeshComponent> _static;
        private Actor<SkeletalMeshComponent> _skeletal;
        public IModelFile Model { get; private set; }

        public void SetModel(StaticModel stm)
        {
            if (_static != null && _static.IsSpawned)
                World.DespawnActor(_static);
            if (_skeletal != null && _skeletal.IsSpawned)
                World.DespawnActor(_skeletal);

            Model = stm;
            _static = new Actor<StaticMeshComponent>(new StaticMeshComponent(stm));
            World.SpawnActor(_static);
        }
        public void SetModel(SkeletalModel skm, Skeleton skel)
        {
            if (_static != null && _static.IsSpawned)
                World.DespawnActor(_static);
            if (_skeletal != null && _skeletal.IsSpawned)
                World.DespawnActor(_skeletal);

            Model = skm;
            _skeletal = new Actor<SkeletalMeshComponent>(new SkeletalMeshComponent(skm, skel));
            World.SpawnActor(_skeletal);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetRenderTicking(true);
            Editor.Instance.SetRenderTicking(false);
            GetRenderForm(0);
        }
        protected override void OnClosed(EventArgs e)
        {
            SetRenderTicking(false);
            Editor.Instance.SetRenderTicking(true);
            base.OnClosed(e);
        }

        public bool IsRenderTicking { get; private set; }
        public void SetRenderTicking(bool isRendering)
        {
            if (isRendering && !IsRenderTicking)
            {
                IsRenderTicking = true;
                Engine.RegisterRenderTick(RenderTick);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterRenderTick(RenderTick);
            }
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            try { Invoke((Action)Redraw); } catch { }
        }
        private void Redraw()
        {
            World.Scene.Voxelize();
            World.Scene.RenderShadowMaps();

            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();

            Application.DoEvents();
        }
    }
}
