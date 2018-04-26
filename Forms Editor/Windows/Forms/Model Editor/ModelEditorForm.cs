using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Animation;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Maps;
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
        public ModelEditorForm(SkeletalModel m) : this() => SetModel(m);
        public ModelEditorForm(StaticModel m) : this() => SetModel(m);
        
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
                value.Show(DockPanel, state);
            }
            return value;
        }

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

        private DockableMeshList _meshList;
        public bool MeshListActive => _meshList != null;
        public DockableMeshList MeshList => GetForm(
            ref _meshList, DockState.DockLeft);
        
        private DockableMaterialList _materialList;
        public bool MaterialListActive => _materialList != null;
        public DockableMaterialList MaterialList => GetForm(
            ref _materialList, DockState.DockLeft);

        #endregion
        
        private LocalFileRef<World> ModelEditorWorld
            = new LocalFileRef<World>(Engine.EngineWorldsPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld")));
        
        public World World
        {
            get
            {
                bool loaded = ModelEditorWorld.IsLoaded;
                bool fileDoesNotExist = !ModelEditorWorld.FileExists;

                if (fileDoesNotExist)
                {
                    List<IActor> actors = new List<IActor>();

                    DirectionalLightActor light = new DirectionalLightActor();
                    DirectionalLightComponent comp = light.RootComponent;
                    comp.DiffuseIntensity = 1.0f;
                    comp.LightColor = new EventColorF3(1.0f);
                    comp.Rotation.Yaw = 45.0f;
                    comp.Rotation.Pitch = -45.0f;
                    actors.Add(light);

                    Vec3 max = 1000.0f;
                    Vec3 min = -max;
                    TextureFile2D skyTex = Engine.LoadEngineTexture2D("skybox.png");
                    StaticModel skybox = new StaticModel("Skybox");
                    TexRef2D texRef = new TexRef2D("SkyboxTexture", skyTex)
                    {
                        MagFilter = ETexMagFilter.Nearest,
                        MinFilter = ETexMinFilter.Nearest
                    };
                    StaticRigidSubMesh mesh = new StaticRigidSubMesh("Mesh", true,
                        BoundingBox.FromMinMax(min, max),
                        BoundingBox.SolidMesh(min, max, true,
                        skyTex.Bitmaps[0].Width > skyTex.Bitmaps[0].Height ?
                            BoundingBox.ECubemapTextureUVs.WidthLarger :
                            BoundingBox.ECubemapTextureUVs.HeightLarger),
                        TMaterial.CreateUnlitTextureMaterialForward(texRef, new RenderingParameters()
                        {
                            DepthTest = new DepthTest()
                            {
                                Enabled = ERenderParamUsage.Enabled,
                                UpdateDepth = false,
                                Function = EComparison.Less
                            }
                        }));
                    mesh.RenderInfo.RenderPass = ERenderPass.Background;
                    skybox.RigidChildren.Add(mesh);
                    Actor<StaticMeshComponent> skyboxActor = new Actor<StaticMeshComponent>();
                    skyboxActor.RootComponent.ModelRef = skybox;
                    actors.Add(skyboxActor);
                    
                    ModelEditorWorld.File = new World()
                    {
                        Settings = new WorldSettings("ModelEditorWorld", new Map(new MapSettings(actors))),
                    };
                }
                
                World w = ModelEditorWorld.File;
                if (!loaded)
                {
                    w.BeginPlay();
                    //if (fileDoesNotExist)
                    //    ModelEditorWorld.File.Export(Engine.EngineWorldsPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld")));
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
            
            MeshList.DisplayMeshes(stm);
            MaterialList.DisplayMaterials(stm);

            BoundingBox aabb = stm?.CalculateCullingAABB() ?? new BoundingBox();
            RenderForm1.AlignView(aabb);
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

            MeshList.DisplayMeshes(skm);
            MaterialList.DisplayMaterials(skm);
            BoneTreeForm.NodeTree.DisplayNodes(skel);

            BoundingBox aabb = skm.CalculateBindPoseCullingAABB();
            RenderForm1.AlignView(aabb);
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
                Engine.RegisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
        }

        private void SwapBuffers()
        {
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.SwapBuffers();
        }

        private void UpdateTick(object sender, FrameEventArgs e)
        {
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.UpdateTick(sender, e);
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
