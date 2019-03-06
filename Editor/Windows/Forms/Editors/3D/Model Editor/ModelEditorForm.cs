using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Animation;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Timers;
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
            formMenu.Renderer = new TheraToolstripRenderer();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            ModelEditorText.MouseDown += TitleBar_MouseDown;
        }

        public ModelEditorForm(SkeletalModel model) : this() => SetModel(model);
        public ModelEditorForm(StaticModel model) : this() => SetModel(model);
        public ModelEditorForm(BaseActor actor) : this() => SetActor(actor);
        public ModelEditorForm(PropAnimVec3 vec3Anim) : this() => SetAnim(vec3Anim);

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
            if (value == null || value.IsDisposed)
            {
                value = new T();
                //Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(pane, align, prop);
            }
            return value;
        }

        public T GetForm<T>(ref T value, DockState state) where T : DockContent, new()
        {
            if (value == null || value.IsDisposed)
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

        private LocalFileRef<World> ModelEditorWorld
            = new LocalFileRef<World>(/*Engine.Files.WorldPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld"))*/);

        public async Task InitWorldAsync()
        {
            //bool fileDoesNotExist = !ModelEditorWorld.FileExists;
            World world;// = await ModelEditorWorld.GetInstanceAsync();
            //if (world == null)
            //{
                List<BaseActor> actors = new List<BaseActor>();

                DirectionalLightActor light = new DirectionalLightActor();
                DirectionalLightComponent comp = light.RootComponent;
                comp.DiffuseIntensity = 1.0f;
                comp.LightColor = new EventColorF3(1.0f);
                comp.Rotation.Yaw = 45.0f;
                comp.Rotation.Pitch = -45.0f;
                comp.Scale = new Vec3(2000.0f);
                actors.Add(light);
                
                TextureFile2D skyTex = await Engine.Files.LoadEngineTexture2DAsync("modelviewerbg1.png");
                SkyboxActor skyboxActor = new SkyboxActor(skyTex, 1000.0f);
                actors.Add(skyboxActor);

                IBLProbeGridActor iblProbes = new IBLProbeGridActor();
                iblProbes.AddProbe(Vec3.Zero);
                actors.Add(iblProbes);

                ModelEditorWorld.File = world = new World()
                {
                    Settings = new WorldSettings("ModelEditorWorld", new Map(actors)),
                };
            //}
            world.BeginPlay();
            
            //if (fileDoesNotExist)
            //    await ModelEditorWorld.File.ExportAsync(Engine.Files.WorldPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld")));
        }

        public World World => ModelEditorWorld.File;        
        public BaseActor TargetActor { get; private set; }
        public IModelFile Model { get; private set; }

        public async void SetActor(BaseActor actor)
        {
            if (World == null)
            {
                await InitWorldAsync();
            }

            FormTitle2.Text = actor?.FilePath ?? actor?.Name ?? string.Empty;

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);
            
            TargetActor = actor;
            World.SpawnActor(TargetActor);

            PropGrid.PropertyGrid.TargetObject = actor;
        }
        public async void SetModel(StaticModel stm)
        {
            if (World == null)
                await InitWorldAsync();

            FormTitle2.Text = stm?.FilePath ?? stm?.Name ?? string.Empty;

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);

            Model = stm;
            StaticMeshComponent comp = new StaticMeshComponent(stm);
            TargetActor = new Actor<StaticMeshComponent>(comp);
            World.SpawnActor(TargetActor);
            
            MeshList.DisplayMeshes(comp);
            MaterialList.DisplayMaterials(stm);

            //BoundingBox aabb = stm?.CalculateCullingAABB() ?? new BoundingBox();
            //RenderForm1.AlignView(aabb);

            PropGrid.PropertyGrid.TargetObject = stm;
        }
        public async void SetModel(SkeletalModel skm)
        {
            if (World == null)
                await InitWorldAsync();

            Skeleton skel = skm.SkeletonRef?.File;

            FormTitle2.Text = $"{skm?.FilePath ?? skm?.Name ?? string.Empty} [{skel?.FilePath ?? skel?.Name ?? string.Empty}]";

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);

            Model = skm;
            SkeletalMeshComponent comp = new SkeletalMeshComponent(skm, skel);
            TargetActor = new Actor<SkeletalMeshComponent>(comp);
            AnimStateMachineComponent machine = new AnimStateMachineComponent(skm.SkeletonRef?.File);
            TargetActor.LogicComponents.Add(machine);
            World.SpawnActor(TargetActor);

            MeshList.DisplayMeshes(comp);
            MaterialList.DisplayMaterials(skm);
            BoneTreeForm.SetSkeleton(skel);
            AnimList.Show();

            //BoundingBox aabb = skm.CalculateBindPoseCullingAABB();
            //RenderForm1.AlignView(aabb);

            PropGrid.PropertyGrid.TargetObject = skm;
        }
        public async void SetAnim(PropAnimVec3 vec3anim)
        {
            if (World == null)
                await InitWorldAsync();

            FormTitle2.Text = vec3anim?.FilePath ?? vec3anim?.Name ?? string.Empty;

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);

            Spline3DComponent comp = new Spline3DComponent(vec3anim);
            TargetActor = new Actor<Spline3DComponent>(comp);
            World.SpawnActor(TargetActor);

            PropGrid.PropertyGrid.TargetObject = vec3anim;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //Editor.Instance.SetRenderTicking(false);
            if (World == null)
                await InitWorldAsync();
            RenderForm1.RenderPanel.CaptureContext();
            SetRenderTicking(true);
        }
        protected override void OnClosed(EventArgs e)
        {
            SetRenderTicking(false);
            //Editor.Instance.SetRenderTicking(true);
            World.DespawnActor(TargetActor);
            //if (Model is SkeletalModel skm && skm.SkeletonRef?.IsLoaded == true)
            //    World.Scene3D?.Renderables.Remove(skm.SkeletonRef.File);
            base.OnClosed(e);
        }

        public bool IsRenderTicking { get; private set; }

        public void SetRenderTicking(bool isRendering)
        {
            if (isRendering && !IsRenderTicking)
            {
                IsRenderTicking = true;
                Engine.RegisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
        }

        private void UpdateTick(object sender, FrameEventArgs e)
        {
            World.Scene?.GlobalUpdate();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.UpdateTick(sender, e);
        }
        private void SwapBuffers()
        {
            World.Scene?.GlobalSwap();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.SwapBuffers();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            RenderForm1.RenderPanel.CaptureContext();

            World.Scene?.GlobalPreRender();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();
        }

        private void btnViewport1_Click     (object sender, EventArgs e) => RenderForm1.Focus();
        private void btnViewport2_Click     (object sender, EventArgs e) => RenderForm2.Focus();
        private void btnViewport3_Click     (object sender, EventArgs e) => RenderForm3.Focus();
        private void btnViewport4_Click     (object sender, EventArgs e) => RenderForm4.Focus();
        private void btnMeshList_Click      (object sender, EventArgs e) => MeshList.Focus();
        private void btnMaterialList_Click  (object sender, EventArgs e) => MaterialList.Focus();
        private void btnSkeleton_Click      (object sender, EventArgs e) => BoneTreeForm.Focus();

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
            if (TargetActor?.RootComponentGeneric is SkeletalMeshComponent skel && skel.TargetSkeleton != null)
                foreach (Bone bone in skel.TargetSkeleton.BoneIndexCache.Values)
                    if (bone?.RigidBodyCollision?.CollisionShape != null)
                        bone.RigidBodyCollision.CollisionShape.DebugRender = true;
        }
        private void chkViewConstraints_Click(object sender, EventArgs e)
        {
            chkViewConstraints.Checked = !chkViewConstraints.Checked;
            World.PhysicsWorld3D.DrawConstraints = chkViewConstraints.Checked;
            World.PhysicsWorld3D.DrawConstraintLimits = chkViewConstraints.Checked;
            //if (TargetActor?.RootComponentGeneric is SkeletalMeshComponent skel && skel.TargetSkeleton != null)
            //    foreach (Bone bone in skel.TargetSkeleton.BoneIndexCache.Values)
            //        bone.ParentPhysicsConstraint
        }
        private void chkViewCullingVolumes_Click(object sender, EventArgs e)
        {
            chkViewCullingVolumes.Checked = !chkViewCullingVolumes.Checked;
            var comp = TargetActor?.RootComponentGeneric;
            RenderInfo3D r3D;
            switch (comp)
            {
                case SkeletalMeshComponent skelComp:
                    if (skelComp.Meshes != null)
                        foreach (var mesh in skelComp.Meshes)
                            if ((r3D = mesh?.RenderInfo?.CullingVolume?.RenderInfo) != null)
                                r3D.Visible = chkViewCullingVolumes.Checked;
                    break;
                case StaticMeshComponent staticComp:
                    if (staticComp.Meshes != null)
                        foreach (var mesh in staticComp.Meshes)
                            if ((r3D = mesh?.RenderInfo?.CullingVolume?.RenderInfo) != null)
                                r3D.Visible = chkViewCullingVolumes.Checked;
                    break;
            }
        }
        private void chkViewBones_Click(object sender, EventArgs e)
        {
            chkViewBones.Checked = !chkViewBones.Checked;
            if (TargetActor?.RootComponentGeneric is SkeletalMeshComponent skel)
                skel.TargetSkeleton.RenderInfo.Visible = chkViewBones.Checked;
        }
    }
}
