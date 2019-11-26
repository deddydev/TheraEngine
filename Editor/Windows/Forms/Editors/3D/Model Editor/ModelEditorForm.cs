using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Reflection;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(SkeletalModel), typeof(StaticModel), typeof(IActor), typeof(PropAnimVec3))]
    public partial class ModelEditorForm : TheraForm, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => DockPanel1;

        public ModelEditorForm()
        {
            InitializeComponent();

            ModelEditorText.Font = Engine.MakeFont("origicide", 10.0f, FontStyle.Regular);
            DockPanel1.Theme = new TheraEditorTheme();
            AutoScaleMode = AutoScaleMode.Font;
            DoubleBuffered = false;
            formMenu.Renderer = new TheraToolStripRenderer();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            ModelEditorText.MouseDown += TitleBar_MouseDown;
        }

        public ModelEditorForm(SkeletalModel model) : this() => SetModel(model);
        public ModelEditorForm(StaticModel model) : this() => SetModel(model);
        public ModelEditorForm(BaseActor actor) : this() => SetActor(actor);
        public ModelEditorForm(PropAnimVec3 vec3Anim) : this() => SetAnim(vec3Anim);

        public int WorldManagerId => WorldManager?.ID ?? -1;
        public event Action WorldManagerChanged;
        public ModelEditorWorldManager WorldManager
        {
            get => _worldManager;
            private set
            {
                _worldManager = value;
                WorldManagerChanged?.Invoke();
            }
        }
        private ModelEditorWorldManager _worldManager;

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
            if (form is null || form.IsDisposed)
            {
                Engine.PrintLine("Created model editor viewport " + (i + 1).ToString());
                form = _renderForms[i] = new DockableModelEditorRenderForm(ELocalPlayerIndex.One, i, this);
                form.Show(DockPanel1, DockState.Document);
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
            if (value is null || value.IsDisposed)
            {
                value = new T();
                //Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(pane, align, prop);
            }
            return value;
        }

        public T GetForm<T>(ref T value, DockState state) where T : DockContent, new()
        {
            if (value is null || value.IsDisposed)
            {
                value = new T();
                //Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(DockPanel1, state);
            }
            return value;
        }

        private DockableMaterialPreview _matPreviewForm;
        public bool MatPreviewFormActive => _matPreviewForm != null;
        public DockableMaterialPreview MatPreviewForm => GetForm(
            ref _matPreviewForm, DockState.DockRight);

        private DockableTexRefControl _texRefForm;
        public bool TexRefFormActive => _texRefForm != null;
        public DockableTexRefControl TexRefForm => GetForm(
            ref _texRefForm, DockState.DockLeft);

        private DockableBoneTree _boneTreeForm;
        public bool BoneTreeFormActive => _boneTreeForm != null;
        public DockableBoneTree BoneTreeForm => GetForm(
            ref _boneTreeForm, DockState.DockRight);

        private DockableMaterialEditor _materialEditor;
        public bool MaterialEditorActive => _materialEditor != null;
        public DockableMaterialEditor MaterialEditor => GetForm(
            ref _materialEditor, DockState.DockRight);

        private DockableMeshEditor _meshEditor;
        public bool MeshEditorActive => _meshEditor != null;
        public DockableMeshEditor MeshEditor => GetForm(
            ref _meshEditor, DockState.DockRight);

        private DockableAnimationList _animList;
        public bool AnimListActive => _meshEditor != null;
        public DockableAnimationList AnimList => GetForm(
            ref _animList, DockState.DockRight);

        private DockableMeshList _meshList;
        public bool MeshListActive => _meshList != null;
        public DockableMeshList MeshList => GetForm(
            ref _meshList, DockState.DockLeft);

        private DockableMaterialList _materialList;
        public bool MaterialListActive => _materialList != null;
        public DockableMaterialList MaterialList => GetForm(
            ref _materialList, DockState.DockLeft);

        private DockablePropertyGrid _propGrid;
        public bool PropGridActive => _propGrid != null;
        public DockablePropertyGrid PropGrid => GetForm(
            ref _propGrid, DockState.DockLeft);

        #endregion



        public void SetActor(BaseActor actor)
        {
            WorldManager?.SetActor(actor);
            PropGrid.PropertyGrid.TargetObject = actor;
            RenderForm1.Focus();
        }
        public void SetModel(StaticModel stm)
        {
            WorldManager?.SetModel(stm);
            MaterialList.DisplayMaterials(stm);
            PropGrid.PropertyGrid.TargetObject = stm;
            RenderForm1.Focus();
        }
        public void SetModel(SkeletalModel skm)
        {
            WorldManager?.SetModel(skm);
            MaterialList.DisplayMaterials(skm);
            BoneTreeForm.SetSkeleton(skm.SkeletonRef?.File);
            AnimList.Show();
            PropGrid.PropertyGrid.TargetObject = skm;
            RenderForm1.Focus();
        }
        public void SetAnim(PropAnimVec3 vec3anim)
        {
            WorldManager?.SetAnim(vec3anim);
            PropGrid.PropertyGrid.TargetObject = vec3anim;
            RenderForm1.Focus();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            WorldManager = Engine.DomainProxy.RegisterAndGetWorldManager<ModelEditorWorldManager>();
            AppDomainHelper.Sponsor(WorldManager);
            WorldManager.OnShown();
            WorldManager.TargetActorLoaded += WorldManager_TargetActorLoaded;
        }

        public event Action CloseInvoked;
        protected override void OnClosed(EventArgs e)
        {
            WorldManager.OnClosed();
            WorldManager.TargetActorLoaded -= WorldManager_TargetActorLoaded;
            Engine.DomainProxy.UnregisterWorldManager(WorldManagerId);
            AppDomainHelper.ReleaseSponsor(WorldManager);
            WorldManager = null;

            //if (Model is SkeletalModel skm && skm.SkeletonRef?.IsLoaded == true)
            //    World.Scene3D?.Renderables.Remove(skm.SkeletonRef.File);

            CloseInvoked?.Invoke();

            base.OnClosed(e);
        }

        private void WorldManager_TargetActorLoaded(IActor obj)
        {
            switch (obj)
            {
                case Actor<StaticMeshComponent> staticActor:
                    MeshList.DisplayMeshes(staticActor.RootComponent);
                    break;
                case Actor<SkeletalMeshComponent> skelActor:
                    MeshList.DisplayMeshes(skelActor.RootComponent);
                    break;
                case Actor<Spline3DComponent> _:

                    break;
                case BaseActor _:

                    break;
            }
            FormTitle2.Text = WorldManager?.FormTitleText;
            RenderForm1.Focus();
        }

        private void btnViewport1_Click(object sender, EventArgs e) => RenderForm1.Focus();
        private void btnViewport2_Click(object sender, EventArgs e) => RenderForm2.Focus();
        private void btnViewport3_Click(object sender, EventArgs e) => RenderForm3.Focus();
        private void btnViewport4_Click(object sender, EventArgs e) => RenderForm4.Focus();
        private void btnMeshList_Click(object sender, EventArgs e) => MeshList.Focus();
        private void btnMaterialList_Click(object sender, EventArgs e) => MaterialList.Focus();
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
            if (WorldManager != null)
                WorldManager.ViewCollisions = chkViewCollisions.Checked;
        }
        private void chkViewConstraints_Click(object sender, EventArgs e)
        {
            chkViewConstraints.Checked = !chkViewConstraints.Checked;
            if (WorldManager != null)
                WorldManager.ViewConstraints = chkViewConstraints.Checked;
        }
        private void chkViewCullingVolumes_Click(object sender, EventArgs e)
        {
            chkViewCullingVolumes.Checked = !chkViewCullingVolumes.Checked;
            if (WorldManager != null)
                WorldManager.ViewCullingVolumes = chkViewCullingVolumes.Checked;
        }
        private void chkViewBones_Click(object sender, EventArgs e)
        {
            chkViewBones.Checked = !chkViewBones.Checked;
            if (WorldManager != null)
                WorldManager.ViewBones = chkViewBones.Checked;
        }
    }
}
