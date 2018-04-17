﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(SkeletalModel), typeof(StaticModel))]
    public partial class ModelEditorForm : TheraForm
    {
        public ModelEditorForm()
        {
            InitializeComponent();
            DockPanel.Theme = new TheraEditorTheme();
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = false;
            formMenu.Renderer = new TheraToolstripRenderer();
            FormTitle2.MouseDown += new MouseEventHandler(TitleLabel_MouseDown);
            ModelEditorText.MouseDown += new MouseEventHandler(TitleLabel_MouseDown);
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
        
        public T GetForm<T>(ref T value, DockPane pane, DockAlignment align, double prop) where T : DockContent, new()
        {
            if (value == null || value.IsDisposed)
            {
                value = new T();
                Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(pane, align, prop);
            }
            return value;
        }
        
        private DockableBoneTree _boneTreeForm;
        public DockableBoneTree BoneTreeForm => GetForm(ref _boneTreeForm, RenderForm1.Pane, DockAlignment.Right, 0.2);
        
        private DockableMeshList _meshesForm;
        public DockableMeshList MeshesForm => GetForm(ref _meshesForm, RenderForm1.Pane, DockAlignment.Left, 0.3);
        
        private DockablePropertyGrid _propertyGridForm;
        public DockablePropertyGrid PropertyGridForm => GetForm(ref _propertyGridForm, MeshesForm.Pane, DockAlignment.Bottom, 0.6);

        private DockableMaterialList _materialsForm;
        public DockableMaterialList MaterialsForm => GetForm(ref _materialsForm, MeshesForm.Pane, DockAlignment.Bottom, 0.5);

        #endregion
        
        private LocalFileRef<World> ModelEditorWorld
            = new LocalFileRef<World>(Engine.EngineWorldsPath("ModelEditorWorld.xworld"));
        
        public World World
        {
            get
            {
                bool loaded = ModelEditorWorld.IsLoaded;
                if (!ModelEditorWorld.FileExists)
                {
                    ModelEditorWorld.File = new ModelEditorWorld();
                }
                World w = ModelEditorWorld.File;
                if (!loaded)
                {
                    w.BeginPlay();
                    //ModelEditorWorld.File.Export(Engine.EngineWorldsPath("ModelEditorWorld.xworld"));
                }
                return w;
            }
        }

        private Actor<StaticMeshComponent> _static;
        private Actor<SkeletalMeshComponent> _skeletal;
        public IModelFile Model { get; private set; }

        public void SetModel(StaticModel stm)
        {
            FormTitle2.Text = stm?.FilePath ?? stm?.Name ?? string.Empty;

            if (_static != null && _static.IsSpawned)
                World.DespawnActor(_static);
            if (_skeletal != null && _skeletal.IsSpawned)
                World.DespawnActor(_skeletal);

            Model = stm;
            _static = new Actor<StaticMeshComponent>(new StaticMeshComponent(stm));
            World.SpawnActor(_static);
            
            MeshesForm.DisplayMeshes(_static);
            MaterialsForm.DisplayMaterials(_static);

            BoundingBox aabb = stm.CalculateAABB();
            RenderForm1.AlignView(aabb);

            //PropertyGridForm.PropertyGrid.TargetObject = stm;
        }
        public void SetModel(SkeletalModel skm)
        {
            Skeleton skel = skm.SkeletonRef?.File;

            FormTitle2.Text = string.Format("{0} [{1}]", 
                skm?.FilePath ?? skm?.Name ?? string.Empty,
                skel?.FilePath ?? skel?.Name ?? string.Empty);

            if (_static != null && _static.IsSpawned)
                World.DespawnActor(_static);
            if (_skeletal != null && _skeletal.IsSpawned)
                World.DespawnActor(_skeletal);

            Model = skm;
            _skeletal = new Actor<SkeletalMeshComponent>(new SkeletalMeshComponent(skm, skel));
            World.SpawnActor(_skeletal);
            World.Scene.Add(skel);

            MeshesForm.DisplayMeshes(_skeletal);
            MaterialsForm.DisplayMaterials(_skeletal);
            BoneTreeForm.NodeTree.DisplayNodes(skel);

            //PropertyGridForm.PropertyGrid.TargetObject = skm;
        }
        public void LoadAnimations(IEnumerable<SkeletalAnimation> anims)
        {

        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //Editor.Instance.SetRenderTicking(false);
            GetRenderForm(0);
            SetRenderTicking(true);
        }
        protected override void OnClosed(EventArgs e)
        {
            SetRenderTicking(false);
            //Editor.Instance.SetRenderTicking(true);
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
            RenderForm1.RenderPanel.CaptureContext();

            World.Scene.Voxelize();
            World.Scene.RenderShadowMaps();

            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();

            Application.DoEvents();
        }
        private void btnViewport1_Click(object sender, EventArgs e) => RenderForm1.Focus();
        private void btnViewport2_Click(object sender, EventArgs e) => RenderForm2.Focus();
        private void btnViewport3_Click(object sender, EventArgs e) => RenderForm3.Focus();
        private void btnViewport4_Click(object sender, EventArgs e) => RenderForm4.Focus();
        private void btnMeshList_Click(object sender, EventArgs e) => MeshesForm.Focus();
        private void btnMaterialList_Click(object sender, EventArgs e) => MaterialsForm.Focus();
        private void btnSkeleton_Click(object sender, EventArgs e) => BoneTreeForm.Focus();

        private void chkViewNormals_Click(object sender, EventArgs e)
        {
            chkViewNormals.Checked = !chkViewNormals.Checked;
        }
        private void chkViewBinormals_Click(object sender, EventArgs e)
        {
            chkViewBinormals.Checked = !chkViewBinormals.Checked;
        }
        private void chkViewTangents_Click(object sender, EventArgs e)
        {
            chkViewTangents.Checked = !chkViewTangents.Checked;
        }
        private void chkViewWireframe_Click(object sender, EventArgs e)
        {
            chkViewWireframe.Checked = !chkViewWireframe.Checked;
        }
        private void chkViewCollisions_Click(object sender, EventArgs e)
        {
            chkViewCollisions.Checked = !chkViewCollisions.Checked;
        }
        private void chkViewCullingVolumes_Click(object sender, EventArgs e)
        {
            chkViewCullingVolumes.Checked = !chkViewCullingVolumes.Checked;
        }
        private void chkViewBones_Click(object sender, EventArgs e)
        {
            chkViewBones.Checked = !chkViewBones.Checked;
        }
        private void chkViewConstraints_Click(object sender, EventArgs e)
        {
            chkViewConstraints.Checked = !chkViewConstraints.Checked;
        }
    }
}
