using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
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
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public class ModelEditorWorldManager : WorldManager
    {
        public ModelEditorWorldManager()
        {
            ModelEditorWorldRef = new LocalFileRef<World>(/*Engine.Files.WorldPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld"))*/);
        }

        private void ModelEditorWorldRef_Unloaded(World obj)
        {
            if (obj == TargetWorld)
                TargetWorld = null;
        }
        private void ModelEditorWorldRef_Loaded(World world)
            => TargetWorld = world;

        private LocalFileRef<World> _modelEditorWorldRef;

        private bool _viewConstraints;
        private bool _viewCollisions;
        private bool _viewCullingVolumes;
        private bool _viewBones;

        public event Action<IActor> TargetActorLoaded;

        public IActor TargetActor { get; private set; }
        public IModelFile Model { get; private set; }
        public string FormTitleText { get; private set; }
        public bool ViewConstraints
        {
            get => _viewConstraints;
            set
            {
                _viewConstraints = value;

                var physics = TargetWorld?.PhysicsWorld3D;
                if (physics != null)
                {
                    physics.DrawConstraints = _viewConstraints;
                    physics.DrawConstraintLimits = _viewConstraints;
                }
                //if (TargetActor?.RootComponentGeneric is SkeletalMeshComponent skel && skel.TargetSkeleton != null)
                //    foreach (Bone bone in skel.TargetSkeleton.BoneIndexCache.Values)
                //        bone.ParentPhysicsConstraint
            }
        }

        public bool ViewCollisions
        {
            get => _viewCollisions;
            set
            {
                _viewCollisions = value;

                if (TargetActor?.RootComponent is SkeletalMeshComponent skel && skel.TargetSkeleton != null)
                    foreach (Bone bone in skel.TargetSkeleton.BoneIndexCache.Values)
                        if (bone?.RigidBodyCollision?.CollisionShape != null)
                            bone.RigidBodyCollision.CollisionShape.DebugRender = _viewCollisions;
            }
        }

        public bool ViewCullingVolumes
        {
            get => _viewCullingVolumes;
            set
            {
                _viewCullingVolumes = value;

                var comp = TargetActor?.RootComponent;
                IRenderInfo3D r3D;
                switch (comp)
                {
                    case SkeletalMeshComponent skelComp:
                        if (skelComp.Meshes != null)
                            foreach (var mesh in skelComp.Meshes)
                                if ((r3D = mesh?.RenderInfo?.CullingVolume?.RenderInfo) != null)
                                    r3D.IsVisible = _viewCullingVolumes;
                        break;
                    case StaticMeshComponent staticComp:
                        if (staticComp.Meshes != null)
                            foreach (var mesh in staticComp.Meshes)
                                if ((r3D = mesh?.RenderInfo?.CullingVolume?.RenderInfo) != null)
                                    r3D.IsVisible = _viewCullingVolumes;
                        break;
                }
            }
        }

        public bool ViewBones
        {
            get => _viewBones;
            set
            {
                _viewBones = value;

                if (TargetActor?.RootComponent is SkeletalMeshComponent skel)
                    skel.TargetSkeleton.RenderInfo.IsVisible = _viewBones;
            }
        }

        public LocalFileRef<World> ModelEditorWorldRef
        {
            get => _modelEditorWorldRef;
            set => SetRef<World, LocalFileRef<World>>(
                ref _modelEditorWorldRef, value,
                ModelEditorWorldRef_Loaded, ModelEditorWorldRef_Unloaded);
        }

        public async Task InitWorldAsync()
        {
            //bool fileDoesNotExist = !ModelEditorWorld.FileExists;
            World world;// = await ModelEditorWorld.GetInstanceAsync();
                        //if (world is null)
                        //{
            List<BaseActor> actors = new List<BaseActor>();

            Actor<DirectionalLightComponent> light = new Actor<DirectionalLightComponent>();
            DirectionalLightComponent comp = light.RootComponent;
            comp.DiffuseIntensity = 1.0f;
            comp.LightColor = new EventColorF3(1.0f);
            comp.Rotation = Quat.Euler(-45.0f, 45.0f, 0.0f);
            comp.Scale = new Vec3(2000.0f);
            actors.Add(light);

            TextureFile2D skyTex = await Engine.Files.LoadEngineTexture2DAsync("modelviewerbg1.png");
            SkyboxActor skyboxActor = new SkyboxActor(skyTex, 1000.0f);
            actors.Add(skyboxActor);

            IBLProbeGridActor iblProbes = new IBLProbeGridActor();
            iblProbes.AddProbe(Vec3.Zero);
            actors.Add(iblProbes);

            ModelEditorWorldRef.File = world = new World()
            {
                Settings = new WorldSettings("ModelEditorWorld", new Map(actors)),
            };

            //}
            world.BeginPlay();

            //if (fileDoesNotExist)
            //    await ModelEditorWorld.File.ExportAsync(Engine.Files.WorldPath(Path.Combine("ModelEditorWorld", "ModelEditorWorld.xworld")));
        }
        public async void LoadActor(IFileRef<IActor> actorRef)
        {
            IActor actor = await actorRef.GetInstanceAsync();
            await SetActorAsync(actor);
        }
        public async void SetActor(IActor model)
            => await SetActorAsync(model);
        public async Task SetActorAsync(IActor actor)
        {
            await UsageChecksAsync();
            Shutdown();

            FormTitleText = actor?.FilePath ?? actor?.Name ?? string.Empty;

            TargetActor = actor;
            TargetWorld.SpawnActor(TargetActor);
            TargetActorLoaded?.Invoke(TargetActor);
        }
        public async void LoadModel(FileRef<StaticModel> staticModelRef)
        {
            StaticModel model = await staticModelRef.GetInstanceAsync();
            await SetModelAsync(model);
        }
        public async void SetModel(StaticModel model)
            => await SetModelAsync(model);
        public async Task SetModelAsync(StaticModel stm)
        {
            await UsageChecksAsync();
            Shutdown();

            FormTitleText = $"{stm?.FilePath ?? stm?.Name ?? string.Empty}";
            Model = stm;

            StaticMeshComponent comp = new StaticMeshComponent(stm);
            TargetActor = new Actor<StaticMeshComponent>(comp);
            TargetWorld.SpawnActor(TargetActor);
            TargetActorLoaded?.Invoke(TargetActor);
        }
        public async void LoadModel(FileRef<SkeletalModel> skeletalModelRef)
        {
            SkeletalModel model = await skeletalModelRef.GetInstanceAsync();
            await SetModelAsync(model);
        }
        public async void SetModel(SkeletalModel model)
            => await SetModelAsync(model);
        public async Task SetModelAsync(SkeletalModel skm)
        {
            await UsageChecksAsync();
            Shutdown();

            Skeleton skel = await skm.SkeletonRef?.GetInstanceAsync();
            FormTitleText = $"{skm?.FilePath ?? skm?.Name ?? string.Empty} [{skel?.FilePath ?? skel?.Name ?? string.Empty}]";
            Model = skm;

            SkeletalMeshComponent comp = new SkeletalMeshComponent(skm, skel);
            TargetActor = new Actor<SkeletalMeshComponent>(comp);
            AnimStateMachineComponent machine = new AnimStateMachineComponent(skm.SkeletonRef?.File);
            TargetActor.LogicComponents.Add(machine);
            TargetWorld.SpawnActor(TargetActor);
            TargetActorLoaded?.Invoke(TargetActor);
        }
        public async void SetAnim(PropAnimVec3 vec3anim)
        {
            await UsageChecksAsync();
            Shutdown();

            FormTitleText = vec3anim?.FilePath ?? vec3anim?.Name ?? string.Empty;

            Spline3DComponent comp = new Spline3DComponent(vec3anim);
            TargetActor = new Actor<Spline3DComponent>(comp);
            TargetWorld.SpawnActor(TargetActor);
            TargetActorLoaded?.Invoke(TargetActor);
        }
        private async Task UsageChecksAsync()
        {
            if (TargetWorld is null)
                await InitWorldAsync();
        }
        private void Shutdown()
        {
            if (TargetActor != null && TargetActor.IsSpawned)
                TargetWorld.DespawnActor(TargetActor);
        }

        public void OnShown() { }
        public void OnClosed() { }
    }
}
