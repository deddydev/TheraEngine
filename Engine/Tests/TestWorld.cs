﻿using System;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Worlds;

namespace TheraEngine.Tests
{
    public class TestWorld : World
    {
        protected override async void OnBeginPlay()
        {
            Settings = new WorldSettings("TestWorld")
            {
                Bounds = BoundingBox.FromHalfExtentsTranslation(new Vec3(200.0f), Vec3.Zero),
            };

            List<BaseActor> actors = new List<BaseActor>();
            BaseActor actor;

            Random r = new Random();
            BaseActor[] array = new BaseActor[100];
            BoundingBox spawnBounds = new BoundingBox(18.0f, 30.0f, 18.0f, 0.0f, 50.0f, 0.0f);
            for (int i = 0; i < array.Length; ++i)
            {
                TRigidBodyConstructionInfo physicsInfo = new TRigidBodyConstructionInfo()
                {
                    Mass = 15.0f,
                    AngularDamping = 0.1f,
                    LinearDamping = 0.05f,
                    Restitution = 1.0f,
                    Friction = 0.7f,
                    RollingFriction = 0.1f,
                    CollisionEnabled = true,
                    SimulatePhysics = true,
                    CollisionGroup = (ushort)ETheraCollisionGroup.DynamicWorld,
                    CollidesWith = (ushort)(ETheraCollisionGroup.StaticWorld | ETheraCollisionGroup.DynamicWorld | ETheraCollisionGroup.Characters | ETheraCollisionGroup.Camera),
                };
                float x = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.X;
                float y = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.Y;
                float z = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.Z;
                BoxActor box = new BoxActor(
                    "Box" + i, 0.4f,
                    new Vec3(x, y + spawnBounds.Translation.Y, z),
                    Quat.Euler(x, y, z),
                    TMaterial.CreateLitColorMaterial(Color.Purple), physicsInfo);
                box.RootComponent.RigidBodyCollision.Collided += OnBoxHit;
                array[i] = box;
            }
            actors.AddRange(array);

            //sphereActor = array[0];
            //sphereActor.RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;

            TRigidBodyConstructionInfo floorInfo = new TRigidBodyConstructionInfo()
            {
                Friction = 0.7f,
                Mass = 20.0f,
                Restitution = 0.5f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                CollisionGroup = (ushort)ETheraCollisionGroup.StaticWorld,
                CollidesWith = (ushort)(ETheraCollisionGroup.Characters | ETheraCollisionGroup.DynamicWorld | ETheraCollisionGroup.Camera),
            };
            BoxActor floorActor1 = new BoxActor(
                "Floor1",
                new Vec3(50.0f, 0.2f, 50.0f),
                new Vec3(0.0f, 0.0f, 0.0f),
                Quat.Euler(00.0f, 0.0f, 0.0f),
                TMaterial.CreateLitColorMaterial(Color.FromArgb(180, 200, 230)), floorInfo);
            //floorActor1.RootComponent.PhysicsDriver.Kinematic = true
            actors.Add(floorActor1);
            BoxActor floorActor2 = new BoxActor(
                "Floor2",
                new Vec3(100.0f, 20.0f, 100.0f),
                new Vec3(0.0f, -20.0f, 0.0f),
                Quat.Euler(0.0f, 0.0f, 0.0f),
                TMaterial.CreateLitColorMaterial(Color.FromArgb(180, 200, 230)), floorInfo);
            actors.Add(floorActor2);
            BoxActor floorActor3 = new BoxActor(
                "Floor3",
                new Vec3(2.0f, 10.0f, 7.0f),
                new Vec3(6.0f, 0.0f, 0.0f),
                Quat.Euler(-10.0f, 60.0f, 0.0f),
                TMaterial.CreateLitColorMaterial(Color.Gray), floorInfo);
            actors.Add(floorActor3);
            //BoxActor floorActor4 = new BoxActor(
            //    "Floor4",
            //    floorInfo,
            //    new Vec3(50.0f, 0.5f, 50.0f),
            //    new Vec3(20.0f, 0.0f, 0.0f),
            //    new Rotator(0.0f, 0.0f, 20.0f, RotationOrder.YPR),
            //    Material.GetLitColorMaterial(Color.Green));
            TextureFile2D skyTex = await Engine.Files.LoadEngineTexture2DAsync("modelviewerbg2.png");
            SkyboxActor skyboxActor = new SkyboxActor(skyTex, 1000.0f);
            actors.Add(skyboxActor);
            //PhysicsConstructionInfo floor2Info = new PhysicsConstructionInfo()
            //{
            //    Mass = 20.0f,
            //    Restitution = 0.5f,
            //    CollisionEnabled = true,
            //    SimulatePhysics = false,
            //    Group = CustomCollisionGroup.StaticWorld,
            //    CollidesWith = CustomCollisionGroup.Characters | CustomCollisionGroup.DynamicWorld,
            //};
            //BoxActor floor2Actor = new BoxActor(
            //    "Floor2",
            //    floor2Info,
            //    new Vec3(2000.0f, 0.5f, 2000.0f),
            //    new Vec3(0.0f, -70.0f, 0.0f),
            //    new Rotator(30.0f, 00.0f, 0.0f, Rotator.Order.YPR),
            //    Material.GetLitColorMaterial(Color.Green));

            //_settings.GlobalAmbient = new ColorF3(0.01f, 0.01f, 0.01f);

            //Actor<PointLightComponent> lightActor = new Actor<PointLightComponent>("TestLightActor");
            //lightActor.RootComponent.Radius = 30.0f;
            //lightActor.RootComponent.AmbientIntensity = 0.0f;
            //lightActor.RootComponent.DiffuseIntensity = 1000.0f;
            //lightActor.RootComponent.LightColor = (ColorF3)Color.Beige;
            //actors.Add(lightActor);

            //PropAnimFloat lightAnim = new PropAnimFloat(7.0f, true, false);
            //FloatKeyframe k0 = new FloatKeyframe(0.0f, 5.0f, 0.0f, PlanarInterpType.CubicHermite);
            //FloatKeyframe k1 = new FloatKeyframe(3.5f, 25.0f, 0.0f, PlanarInterpType.CubicHermite);
            //FloatKeyframe k2 = new FloatKeyframe(7.0f, 5.0f, 0.0f, PlanarInterpType.CubicHermite);
            //lightAnim.Keyframes.Add(k0, k1, k2);
            //AnimationContainer lightAnimContainer = new AnimationContainer(
            //    "LightTranslationAnim", "Translation.Y", false, lightAnim);
            //lightActor.RootComponent.AddAnimation(lightAnimContainer, true);

            //floorActor1.RootComponent.AddAnimation(lightAnimContainer, true, ETickGroup.PostPhysics, ETickOrder.BoneAnimation, InputPauseType.TickOnlyWhenUnpaused);

            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                (ColorF3)Color.Beige, 1.0f, new Rotator(-35.0f, 30.0f, 0.0f))
            {
                Scale = Settings.Bounds.HalfExtents.LengthFast
            };
            actor = new Actor<DirectionalLightComponent>(dirLightComp) { Name = "SunLight" };
            actors.Add(actor);

            ColladaImportOptions objOptions = new ColladaImportOptions()
            {
                InitialTransform = new TTransform(Vec3.Zero, Quat.Identity, 0.1f, ETransformOrder.TRS),
                //InitialTransform = new FrameState(new Vec3(-100.0f, -100.0f, -1700.0f), Quat.Identity, Vec3.One, TransformOrder.TRS),
            };
            //StaticMesh testModel = OBJ.Import(/*"E:\\Documents\\StationSquare\\main1\\landtable.obj"*/"X:\\Repositories\\TheraEngine\\Build\\test\\test.obj", objOptions);

            ColladaImportOptions options = new ColladaImportOptions()
            {
                InitialTransform = new TTransform(Vec3.Zero, Quat.Identity, new Vec3(1.0f), ETransformOrder.TRS),
            };
            
            //Collada.Data anim = Collada.Import(TestDefaults.GoogleDrivePath + "Assets\\Characters\\Carly\\Animations\\Carly_Idle.dae", options);

            //ColladaScene = Collada.Import(desktop + "carly\\carly.dae", options, false, true);
            //Collada.Data model = Collada.Import(TestDefaults.DesktopPath + "TEST.DAE", options);

            //ModelScene scene = Collada.Import(desktop + "skybox.dae", options);
            //StaticMeshComponent c = new StaticMeshComponent(scene.StaticModel, null);
            //Actor<StaticMeshComponent> skybox = new Actor<StaticMeshComponent>(c) { Name = "Skybox" };

            //if (model.Models.Count > 0)
            //{
            //    ModelScene scene = model.Models[0];
            //    if (scene.SkeletalModel != null)
            //    {
            //        SkeletalMeshComponent comp = new SkeletalMeshComponent(model.Models[0].SkeletalModel, model.Models[0].Skeleton);
                    
            //        comp.Translation.Raw = Vec3.Up * 70.0f;
                    
            //        IActor importedActor = new Actor<SkeletalMeshComponent>(comp) { Name = "SkeletalMeshActor" };
            //        actors.Add(importedActor);
            //    }
            //}

            //int timeInSeconds = 20;
            //int frames = timeInSeconds * 60;

            //Actor<SplineComponent> splineActor = new Actor<SplineComponent>();
            //PropAnimVec3 spline = new PropAnimVec3(frames, true, true);

            //float dist = 200.0f;
            //Vec3 front = center - Vec3.Forward * dist - Vec3.Up * 10.0f;
            //Vec3 right = center + Vec3.Right * dist + Vec3.Up * 10.0f;
            //Vec3 back = center + Vec3.Forward * dist - Vec3.Up * 10.0f;
            //Vec3 left = center - Vec3.Right * dist + Vec3.Up * 10.0f;

            //Vec3 rightTan = Vec3.Right * 100.0f;
            //Vec3 forwardTan = Vec3.Forward * 100.0f;

            //spline.Keyframes.Add(new Vec3Keyframe(0.0f, front, front, -rightTan, rightTan, PlanarInterpType.CubicBezier));
            //spline.Keyframes.Add(new Vec3Keyframe(frames / 4.0f, right, right, -forwardTan, forwardTan, PlanarInterpType.CubicBezier));
            //spline.Keyframes.Add(new Vec3Keyframe(frames / 2.0f, back, back, rightTan, -rightTan, PlanarInterpType.CubicBezier));
            //spline.Keyframes.Add(new Vec3Keyframe(frames / 4.0f * 3.0f, left, left, forwardTan, -forwardTan, PlanarInterpType.CubicBezier));
            //spline.Keyframes.Add(new Vec3Keyframe(frames, front, front, -rightTan, rightTan, PlanarInterpType.CubicBezier));

            //splineActor.RootComponent.RenderTangents = false;
            //splineActor.RootComponent.Spline = spline;

            //cam = new PerspectiveCameraActor();
            ////cam.Camera.LocalPoint = new Vec3(0.0f, 50.0f, 0.0f);
            //AnimationContainer cameraAnim = new AnimationContainer("LocalPoint", false, spline);
            //cam.RootComponent.Camera.AddAnimation(cameraAnim, true);
            //cam.RootComponent.Camera.ViewTarget = center;

            //int timeToZoom = 50 * 60;
            //PropAnimFloat fovAnim = new PropAnimFloat(timeToZoom, false, true);
            //fovAnim.Keyframes.Add(new FloatKeyframe(0.0f, 90.0f, 1.0f, PlanarInterpType.CubicHermite));
            //fovAnim.Keyframes.Add(new FloatKeyframe(timeToZoom, 60.0f, 0.0f, PlanarInterpType.CubicHermite));
            //AnimationContainer camZoomAnim = new AnimationContainer("HorizontalFieldOfView", false, fovAnim);
            //cam.RootComponent.Camera.AddAnimation(camZoomAnim, true);

            //Engine.TimeDilation = 0.3f;
            //cam.RootComponent.SetCurrentForPlayer(PlayerIndex.One);

            SpotLightComponent spotLightComp = new SpotLightComponent(200.0f, new ColorF3(0.7f, 0.9f, 0.9f), 1.0f, Vec3.Down, 30.0f, 10.0f, 40.0f, 1.0f);
            spotLightComp.Transform.Translation.Y = 50.0f;
            Actor<SpotLightComponent> spotlight = new Actor<SpotLightComponent>("SpotLight", spotLightComp);
            actors.Add(spotlight);

            CharacterSpawnPointActor spawn = new CharacterSpawnPointActor();
            spawn.RootComponent.Transform.Translation.Value = Vec3.Up * 100.0f;
            actors.Add(spawn);

            IBLProbeGridActor ibl = new IBLProbeGridActor();
            ibl.AddProbe(new Vec3(0.0f, 100.0f, 0.0f));
            actors.Add(ibl);

            //Actor<BlockingVolumeComponent> block = new Actor<BlockingVolumeComponent>(new BlockingVolumeComponent(
            //        new Vec3(500.0f, 10.0f, 500.0f),
            //        new Vec3(0.0f, -10.22f, 0.0f),
            //        Rotator.GetZero(),
            //        CustomCollisionGroup.StaticWorld,
            //        CustomCollisionGroup.Characters | CustomCollisionGroup.DynamicWorld)) { Name = "Floor" };

            //options = new ModelImportOptions()
            //{
            //    IgnoreFlags =
            //         Core.Files.IgnoreFlags.Extra |
            //         Core.Files.IgnoreFlags.Lights |
            //         Core.Files.IgnoreFlags.Cameras |
            //         Core.Files.IgnoreFlags.Animations,
            //    InitialTransform = new Transform(Vec3.Zero, Quat.Identity, new Vec3(1.0f), TransformOrder.TRS),
            //};

            //var dae = Collada.Import(TestDefaults.DesktopPath + "gun.DAE", options);
            //ModelScene gunScene = dae.Models[0];
            //Actor<StaticMeshComponent> gunActor = new Actor<StaticMeshComponent>(new StaticMeshComponent(gunScene.StaticModel, null)) { Name = "PBRGunTest" };

            Settings.DefaultGameModeRef = new CharacterGameMode<CharacterPawn, CharacterPlayerController>();

            string mapName = "DefaultMap";
            Settings.Maps.Add(mapName, new Map(true, Vec3.Zero, actors) { Name = mapName });

            //Settings.Maps[0].File.Settings.StaticActors.AddRange(array);

            //Export(Engine.ContentFolderAbs, "TestWorld", FileFormat.XML);

            //_settings.AmbientSound = new SoundFile() { SoundPath = desktop + "test.wav" };
            //_settings.AmbientParams.SourceRelative.Value = false;
            //_settings.AmbientParams.ReferenceDistance.Value = 1.0f;
            //_settings.AmbientParams.MaxDistance.Value = 50.0f;

            //_collideSound = new SoundFile(desktop + "test.wav");
            //_param = new AudioSourceParameters();
            //_param.SourceRelative.Value = false;
            //_param.ReferenceDistance.Value = 1.0f;
            //_param.MaxDistance.Value = 50.0f;

            //ToXML(TestDefaults.DesktopPath, "testworld");

            //Task.Run(() => OBJ.Import(Path.Combine(Engine.Settings.ModelsFolder, "Sponza", "sponza.obj"), objOptions)).ContinueWith(t =>
            //{
            //    Actor<StaticMeshComponent> testActor = new Actor<StaticMeshComponent>(new StaticMeshComponent(t.Result, null)) { Name = "MapActor" };
            //    SpawnActor(testActor);
            //});

            base.OnBeginPlay();
        }

        private void OnBoxHit(TCollisionObject @this, TCollisionObject other, TContactInfo info, bool thisIsA)
        {
            ShaderVec3 color = (ShaderVec3)((StaticMeshComponent)@this.Owner).ModelRef.File.RigidChildren[0].LODs[0].MaterialRef.File.Parameters[0];
            color.Value = (ColorF3)Color.Green;
            //_collideSound.Play(_param);
        }
        
        //SoundFile _collideSound;
        //AudioSourceParameters _param;

        //BoxActor sphereActor;
        //PerspectiveCameraActor cam;
        //private void RootComponent_WorldTransformChanged()
        //{
        //    cam.Camera.ViewTarget.Raw = sphereActor.RootComponent.GetWorldPoint();
        //}
    }
}