using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Components.Logic.Animation;
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
            ModelEditorText.Font = Engine.MakeFont("origicide", 10.0f, FontStyle.Regular);
            DockPanel.Theme = new TheraEditorTheme();
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = false;
            formMenu.Renderer = new TheraToolstripRenderer();
            FormTitle2.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            ModelEditorText.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
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

        #endregion
        
        private LocalFileRef<World> ModelEditorWorld
            = new LocalFileRef<World>(/*Engine.EngineWorldsPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld"))*/);

        public async Task InitWorldAsync()
        {
            //bool loaded = ModelEditorWorld.IsLoaded;
            //bool fileDoesNotExist = !ModelEditorWorld.FileExists;
            World world = await ModelEditorWorld.GetInstanceAsync();
            if (world == null)
            {
                List<IActor> actors = new List<IActor>();

                DirectionalLightActor light = new DirectionalLightActor();
                DirectionalLightComponent comp = light.RootComponent;
                comp.DiffuseIntensity = 1.0f;
                comp.LightColor = new EventColorF3(1.0f);
                comp.Rotation.Yaw = 45.0f;
                comp.Rotation.Pitch = -45.0f;
                comp.Scale = new Vec3(5.0f);
                actors.Add(light);

                Vec3 max = 1000.0f;
                Vec3 min = -max;
                TextureFile2D skyTex = await Engine.LoadEngineTexture2DAsync("modelviewerbg1.png");
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
                        BoundingBox.ECubemapTextureUVs.HeightLarger, 0.0f),
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

                IBLProbeGridActor iblProbes = new IBLProbeGridActor();
                iblProbes.AddProbe(Vec3.Zero);
                //iblProbes.SetFrequencies(BoundingBox.FromHalfExtentsTranslation(100.0f, Vec3.Zero), new Vec3(0.02f));
                actors.Add(iblProbes);

                ModelEditorWorld.File = world = new World()
                {
                    Settings = new WorldSettings("ModelEditorWorld", new Map(new MapSettings(actors))),
                };

                world.BeginPlay();
                iblProbes.InitAndCaptureAll(256);
            }
            else
            {
                world.BeginPlay();
                var ibl = world.State.GetSpawnedActorsOfType<IBLProbeGridActor>().ToArray();
                if (ibl.Length > 0 && ibl[0] != null)
                {
                    ibl[0].InitAndCaptureAll(256);
                }
            }
            
            //DirectionalLightActor light = w.State.GetSpawnedActorsOfType<DirectionalLightActor>().ToArray()[0];
            //w.Scene.Add(light.RootComponent.ShadowCamera);
            //if (fileDoesNotExist)
            //    ModelEditorWorld.File.Export(Engine.EngineWorldsPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld")));
        }

        public World World
        {
            get
            {
                return ModelEditorWorld.File;
            }
        }
        
        public IActor TargetActor;
        public IModelFile Model { get; private set; }

        public void SetModel(StaticModel stm)
        {
            FormTitle2.Text = stm?.FilePath ?? stm?.Name ?? string.Empty;

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);

            Model = stm;
            TargetActor = new Actor<StaticMeshComponent>(new StaticMeshComponent(stm));
            World.SpawnActor(TargetActor);
            
            MeshList.DisplayMeshes(stm);
            MaterialList.DisplayMaterials(stm);

            //BoundingBox aabb = stm?.CalculateCullingAABB() ?? new BoundingBox();
            //RenderForm1.AlignView(aabb);
        }
        public void SetModel(SkeletalModel skm)
        {
            Skeleton skel = skm.SkeletonRef?.File;

            FormTitle2.Text = string.Format("{0} [{1}]", 
                skm?.FilePath ?? skm?.Name ?? string.Empty,
                skel?.FilePath ?? skel?.Name ?? string.Empty);

            if (TargetActor != null && TargetActor.IsSpawned)
                World.DespawnActor(TargetActor);

            Model = skm;
            TargetActor = new Actor<SkeletalMeshComponent>(new SkeletalMeshComponent(skm, skel));
            AnimStateMachineComponent machine = new AnimStateMachineComponent(skm.SkeletonRef.File);
            TargetActor.LogicComponents.Add(machine);
            World.SpawnActor(TargetActor);
            World.Scene.Add(skel);

            MeshList.DisplayMeshes(skm);
            MaterialList.DisplayMaterials(skm);
            BoneTreeForm.SetSkeleton(skel);

            //BoundingBox aabb = skm.CalculateBindPoseCullingAABB();
            //RenderForm1.AlignView(aabb);
        }
        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //Editor.Instance.SetRenderTicking(false);
            RenderForm1.RenderPanel.CaptureContext();
            await InitWorldAsync();
            SetRenderTicking(true);
            AnimList.Show();
        }
        protected override void OnClosed(EventArgs e)
        {
            SetRenderTicking(false);
            //Editor.Instance.SetRenderTicking(true);
            World.DespawnActor(TargetActor);
            if (Model is SkeletalModel skm && skm.SkeletonRef?.IsLoaded == true)
                World.Scene.Remove(skm.SkeletonRef.File);
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
            if (World.Scene != null)
            {
                World.Scene.Lights.SwapBuffers();
            }
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.SwapBuffers();
        }

        private void UpdateTick(object sender, FrameEventArgs e)
        {
            World.Scene.UpdateShadowMaps();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.UpdateTick(sender, e);
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            //try { Invoke((Action)Redraw); } catch { }
            Redraw();
        }
        private void Redraw()
        {
            RenderForm1.RenderPanel.CaptureContext();

            World.Scene.Voxelize();
            World.Scene.RenderShadowMaps();

            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();

            //Application.DoEvents();
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
            if (TargetActor is Actor<SkeletalMeshComponent> skel &&
                skel.RootComponent?.SkeletonOverride != null)
            {
                bool inScene = skel.RootComponent.SkeletonOverride.RenderInfo.Scene != null;
                if (inScene == chkViewBones.Checked)
                    return;
                if (inScene)
                    skel.RootComponent.OwningScene.Remove(skel.RootComponent.SkeletonOverride);
                else
                    skel.RootComponent.OwningScene.Add(skel.RootComponent.SkeletonOverride);
            }
        }
        private void chkViewConstraints_Click(object sender, EventArgs e)
        {
            chkViewConstraints.Checked = !chkViewConstraints.Checked;
        }
    }
}
