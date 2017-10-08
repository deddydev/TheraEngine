﻿using ObjLoader.Loader.Loaders;
using System;
using System.Drawing;
using System.IO;
using TheraEngine.Animation;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types;
using TheraEngine.Worlds.Maps;

namespace TheraEngine.Tests
{
    public unsafe class TestWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");
            Random r = new Random();
            IActor[] array = new IActor[20];
            BoundingBox spawnBounds = new BoundingBox(18.0f, 30.0f, 18.0f, 0.0f, 50.0f, 0.0f);
            for (int i = 0; i < array.Length; ++i)
            {
                PhysicsConstructionInfo physicsInfo = new PhysicsConstructionInfo()
                {
                    Mass = 10.0f,
                    AngularDamping = 0.1f,
                    LinearDamping = 0.1f,
                    Restitution = 0.8f,
                    Friction = 0.5f,
                    RollingFriction = 0.2f,
                    CollisionEnabled = true,
                    SimulatePhysics = true,
                    CollisionGroup = CustomCollisionGroup.DynamicWorld,
                    CollidesWith = CustomCollisionGroup.StaticWorld | CustomCollisionGroup.DynamicWorld | CustomCollisionGroup.Characters,
                };
                float x = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.X;
                float y = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.Y;
                float z = ((float)r.NextDouble() - 0.5f) * 2.0f * spawnBounds.HalfExtents.Z;
                SphereActor actor = new SphereActor(
                    "Box" + i, physicsInfo, 0.4f,
                    new Vec3(x, y + spawnBounds.Translation.Y, z),
                    new Rotator(0.0f, 0.0f, 0.0f, RotationOrder.YPR),
                    Material.GetLitColorMaterial(Color.Purple));
                actor.RootComponent.PhysicsDriver.OnHit += PhysicsDriver_OnHit;
                array[i] = actor;
            }
            //sphereActor = array[0];
            //sphereActor.RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;

            PhysicsConstructionInfo floorInfo = new PhysicsConstructionInfo()
            {
                Mass = 20.0f,
                Restitution = 0.5f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                CollisionGroup = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.Characters | CustomCollisionGroup.DynamicWorld,
            };
            BoxActor floorActor1 = new BoxActor(
                "Floor1",
                floorInfo,
                new Vec3(5.0f, 0.2f, 5.0f),
                new Vec3(-5.0f, 0.0f, 0.0f),
                new Rotator(0.0f, 0.0f, 0.0f, RotationOrder.YPR),
                Material.GetLitColorMaterial(Color.Orange));
            //floorActor1.RootComponent.PhysicsDriver.Kinematic = true;
            BoxActor floorActor2 = new BoxActor(
                "Floor2",
                floorInfo,
                new Vec3(2.0f, 0.3f, 2.0f),
                new Vec3(0.0f, 0.0f, 0.0f),
                new Rotator(0.0f, 0.0f, 0.0f, RotationOrder.YPR),
                Material.GetLitColorMaterial(Color.Blue));
            BoxActor floorActor3 = new BoxActor(
                "Floor3",
                floorInfo,
                new Vec3(3.0f, 5.0f, 2.0f),
                new Vec3(2.0f, 0.0f, 0.0f),
                new Rotator(0.0f, 40.0f, 0.0f, RotationOrder.YPR),
                Material.GetLitColorMaterial(Color.Gray));
            //BoxActor floorActor4 = new BoxActor(
            //    "Floor4",
            //    floorInfo,
            //    new Vec3(50.0f, 0.5f, 50.0f),
            //    new Vec3(20.0f, 0.0f, 0.0f),
            //    new Rotator(0.0f, 0.0f, 20.0f, RotationOrder.YPR),
            //    Material.GetLitColorMaterial(Color.Green));

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

            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                (ColorF3)Color.Beige, 1.0f, 0.1f, new Rotator(-35.0f, 30.0f, 0.0f, RotationOrder.YPR));
            //dirLightComp.Translation.Y = 30.0f;

            PropAnimFloat lightAnim = new PropAnimFloat(1000, 60.0f, true, true);
            FloatKeyframe first2 = new FloatKeyframe(0, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe x2 = new FloatKeyframe(500, 60.0f, 20.0f, 0.0f, PlanarInterpType.CubicHermite);
            FloatKeyframe last2 = new FloatKeyframe(1000, 60.0f, 0.0f, 0.0f, PlanarInterpType.CubicHermite);
            lightAnim.Keyframes.Add(first2);
            lightAnim.Keyframes.Add(x2);
            lightAnim.Keyframes.Add(last2);
            AnimationContainer lightAnimContainer = new AnimationContainer("Translation.Y", false, lightAnim);
            //dirLightComp.AddAnimation(lightAnimContainer, true);
            floorActor1.RootComponent.AddAnimation(lightAnimContainer, true, ETickGroup.PostPhysics, ETickOrder.BoneAnimation, InputPauseType.TickOnlyWhenUnpaused);

            Actor<DirectionalLightComponent> dirLightActor = new Actor<DirectionalLightComponent>(dirLightComp) { Name = "SunLight" };
            
            ModelImportOptions objOptions = new ModelImportOptions()
            {
                UseForwardShaders = Engine.Settings.ShadingStyle == ShadingStyle.Forward,
                InitialTransform = new FrameState(Vec3.Zero, Quat.Identity, 0.1f, TransformOrder.TRS),
                //InitialTransform = new FrameState(new Vec3(-100.0f, -100.0f, -1700.0f), Quat.Identity, Vec3.One, TransformOrder.TRS),
            };
            StaticMesh testModel = OBJ.Import(/*"E:\\Documents\\StationSquare\\main1\\landtable.obj"*/"X:\\Repositories\\TheraEngine\\Build\\test\\test.obj", objOptions);
            Actor<StaticMeshComponent> testActor = new Actor<StaticMeshComponent>(new StaticMeshComponent(testModel, null)) { Name = "MapActor" };

            //ModelImportOptions options = new ModelImportOptions()
            //{
            //    ImportAnimations = true,
            //    ImportModels = true,
            //    InitialTransform = new FrameState(Vec3.Zero, Quat.Identity, new Vec3(1.0f), TransformOrder.TRS),
            //};

            //ModelScene m = Collada.Import(googleDrive + "Assets\\Characters\\Carly\\Animations\\Carly_Idle.dae", options);

            //ColladaScene = Collada.Import(desktop + "carly\\carly.dae", options, false, true);
            //ColladaScene = Collada.Import(desktop + "TEST.DAE", options, false, true);

            //ModelScene scene = Collada.Import(desktop + "skybox.dae", options);
            //StaticMeshComponent c = new StaticMeshComponent(scene.StaticModel, null);
            //Actor<StaticMeshComponent> skybox = new Actor<StaticMeshComponent>(c) { Name = "Skybox" };

            //Engine.Settings.Export(desktop, "EngineSettings", Files.FileFormat.Binary);

            //ColladaScene.SkeletalModel.Export(Engine.ContentFolderAbs, "TESTMESH", FileFormat.Binary);
            //foreach (SkeletalRigidSubMesh mesh in ColladaScene.SkeletalModel.RigidChildren)
            //    mesh.Data.ExportReference(Engine.ContentFolderAbs + "TESTMESH\\", mesh.Name + "_Prims", FileFormat.Binary);
            //ColladaScene.Skeleton.Export(Engine.ContentFolderAbs, "TESTSKEL", FileFormat.Binary);

            //Collada.Scene anims = Collada.Import(googleDrive + "Thera Assets\\Characters\\Temp\\Carly_Idle.dae", options, true, false);
            //anims.CleanAnimations(scene._skeletalModel, scene._skeleton);

            //IActor importedActor;
            //if (ColladaScene.SkeletalModel != null)
            //{
            //    SkeletalMeshComponent comp = new SkeletalMeshComponent(ColladaScene.SkeletalModel, ColladaScene.Skeleton);

            //    //ColladaScene._skeleton.Export(desktop, "TEST_SKELETON", true);
            //    //Skeleton newSkel = Import<Skeleton>(desktop + "TEST_SKELETON.xcskl");

            //    comp.Translation.Raw = center + Vec3.Up * 70.0f;
            //    importedActor = new Actor<SkeletalMeshComponent>(comp) { Name = "SkeletalMeshActor" };
            //}
            //else
            //{
            //    StaticMeshComponent comp = new StaticMeshComponent(ColladaScene.StaticModel, null);

            //    int fc = 10 * 60;
            //    PropAnimFloat yawAnim = new PropAnimFloat(fc, true, true);
            //    yawAnim.Keyframes.Add(new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.Linear));
            //    yawAnim.Keyframes.Add(new FloatKeyframe(fc, 360.0f, 0.0f, PlanarInterpType.Linear));
            //    AnimationContainer staticMeshAnim = new AnimationContainer("Rotation.Yaw", false, yawAnim);
            //    comp.AddAnimation(staticMeshAnim, true);

            //    comp.Translation.Raw = center + Vec3.Up * 70.0f;
            //    importedActor = new Actor<StaticMeshComponent>(comp) { Name = "StaticMeshActor" };
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

            SpotLightComponent spotLightComp = new SpotLightComponent(200.0f, new ColorF3(0.7f, 0.9f, 0.9f), 1.0f, 0.3f, new Rotator(-70.0f, 0.0f, 0.0f, RotationOrder.YPR), 30.0f, 10.0f, 40.0f, 100.0f);
            spotLightComp.Translation.Y = 50.0f;
            Actor<SpotLightComponent> spotlight = new Actor<SpotLightComponent>(spotLightComp) { Name = "SpotLight" };

            CharacterSpawnPointActor spawn = new CharacterSpawnPointActor();
            spawn.RootComponent.Translation.Raw = Vec3.Up * 100.0f;

            Actor<BlockingVolumeComponent> block = new Actor<BlockingVolumeComponent>(new BlockingVolumeComponent(
                    new Vec3(50.0f, 10.0f, 50.0f),
                    new Vec3(0.0f, -10.22f, 0.0f),
                    Rotator.GetZero(),
                    CustomCollisionGroup.StaticWorld,
                    CustomCollisionGroup.Characters | CustomCollisionGroup.DynamicWorld)) { Name = "Floor" };

            IActor[] actors = new IActor[]
            {
                //spotlight,
                block,
                testActor,
                floorActor1,
                floorActor2,
                floorActor3,
                //floorActor4,
                dirLightActor,
                //skybox,
                //splineActor,
                //cam,
                //floor2Actor,
                spawn,
                //importedActor,
                //new FlyingCameraPawn(PlayerIndex.One) { Name = "PlayerCamera" },
                //new CharacterPawn(PlayerIndex.Two, ColladaScene?.SkeletalModel, ColladaScene?.Skeleton) { Name = "PlayerCharacter", },
            };

            _settings.GameMode = new TestGameMode();// new GameMode<FlyingCameraPawn>();
            _settings.Maps.Add(new Map(this, new MapSettings(actors)));
            _settings.Maps[0].Settings.DefaultActors.AddRange(array);

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
        }
        
        private void PhysicsDriver_OnHit(IPhysicsDrivable me, IPhysicsDrivable other, BulletSharp.ManifoldPoint point)
        {
            ShaderVec4 color = (ShaderVec4)((StaticMeshComponent)me).Model.RigidChildren[0].Material.Parameters[0];
            color.Value = (ColorF4)Color.Green;

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