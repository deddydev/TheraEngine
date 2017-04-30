﻿using System;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering.Animation;
using System.Drawing;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Audio;
using CustomEngine.Worlds;
using CustomEngine.Input;

namespace CustomEngine.Tests
{
    public unsafe class TestWorld : World
    {
        public static Collada.Scene ColladaScene;
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");

            PhysicsConstructionInfo sphereInfo = new PhysicsConstructionInfo()
            {
                Mass = 50.0f,
                AngularDamping = 0.05f,
                LinearDamping = 0.005f,
                Restitution = 0.9f,
                Friction = 0.01f,
                RollingFriction = 0.01f,
                CollisionEnabled = true,
                SimulatePhysics = true,
                Group = CustomCollisionGroup.DynamicWorld,
                CollidesWith = CustomCollisionGroup.StaticWorld,
            };
            SphereActor sphereActor = new SphereActor(
               "Sphere",
               sphereInfo,
               5.0f,
               new Vec3(0.0f, 20.0f, 0.0f),
               Rotator.GetZero(),
               Material.GetDefaultMaterial());

            PhysicsConstructionInfo floorInfo = new PhysicsConstructionInfo()
            {
                Mass = 20.0f,
                Restitution = 0.5f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld | CustomCollisionGroup.Characters,
            };
            BoxActor floorActor = new BoxActor(
                "Floor", 
                floorInfo,
                new Vec3(20.0f, 0.5f, 20.0f), 
                new Vec3(0.0f, 10.0f, 0.0f),
                Rotator.GetZero(),
                Material.GetDefaultMaterial());

            PhysicsConstructionInfo floor2Info = new PhysicsConstructionInfo()
            {
                Mass = 20.0f,
                Restitution = 0.5f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld | CustomCollisionGroup.Characters,
            };
            Material floor2Mat = Material.GetDefaultMaterial();
            ((GLVec4)floor2Mat.Parameters[0]).Value = (ColorF4)Color.Green;
            BoxActor floor2Actor = new BoxActor(
                "Floor2",
                floor2Info, 
                new Vec3(2000.0f, 0.5f, 2000.0f),
                new Vec3(0.0f, -20.0f, 0.0f),
                new Rotator(30.0f, 00.0f, 0.0f, Rotator.Order.YPR),
                floor2Mat);

            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                Color.Beige, 1.0f, 0.6f, new Rotator(-45.0f, 0.0f, 0.0f, Rotator.Order.YPR));
            dirLightComp.Translation.Y = 30.0f;

            PropAnimFloat lightAnim = new PropAnimFloat(360, true, true);
            FloatKeyframe first2 = new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.Linear);
            FloatKeyframe last2 = new FloatKeyframe(360.0f, 360.0f, 0.0f, PlanarInterpType.Linear);
            first2.LinkNext(last2);
            lightAnim.Keyframes.Add(first2);
            AnimationContainer lightAnimContainer = new AnimationContainer("Rotation.Yaw", false, lightAnim);
            //dirLightComp.AddAnimation(lightAnimContainer, false);
            floorActor.RootComponent.AddAnimation(lightAnimContainer, true);

            Actor<DirectionalLightComponent> dirLightActor = new Actor<DirectionalLightComponent>(dirLightComp) { Name = "SunLight" };

            string desktop = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Desktop\\" :
                "C:\\Users\\David\\Desktop\\";
            string googleDrive = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Cloud Storage\\Google Drive\\Game\\" :
                "C:\\Users\\David\\Google Drive\\Game\\";

            Collada.ImportOptions options = new Collada.ImportOptions();
            ////0.02646f
            options.InitialTransform.Scale = new Vec3(1.0f.InchesToMeters());
            ColladaScene = Collada.Import(desktop + "TEST.DAE", options, false, true);
            ColladaScene._skeletalModel.Export(Engine.ContentFolderAbs, "TESTMESH", true);
            ColladaScene._skeleton.Export(Engine.ContentFolderAbs, "TESTSKEL", true);

            //Collada.Scene anims = Collada.Import(googleDrive + "Thera Assets\\Characters\\Temp\\Carly_Idle.dae", options, true, false);
            //anims.CleanAnimations(scene._skeletalModel, scene._skeleton);

            //IActor importedActor;
            //if (scene._skeletalModel != null)
            //{
            //    SkeletalMeshComponent comp = new SkeletalMeshComponent(scene._skeletalModel, scene._skeleton);

            //    AnimationScalar modelAnim = new AnimationScalar(360, true, true);
            //    ScalarKeyframe first = new ScalarKeyframe(0.0f, 0.0f, 0.0f);
            //    ScalarKeyframe second = new ScalarKeyframe(180.0f, 360.0f, 360.0f);
            //    ScalarKeyframe last = new ScalarKeyframe(360.0f, 0.0f, 0.0f);
            //    first.LinkNext(second).LinkNext(last);
            //    modelAnim.Keyframes.AddFirst(first);
            //    scene._skeleton["LElbow"]?.FrameState.AddAnimation(new AnimationContainer("Yaw", false, modelAnim), true);

            //    scene._skeleton.Export(desktop, "TEST_SKELETON", true);
            //    //Skeleton newSkel = Import<Skeleton>(desktop + "TEST_SKELETON.xcskl");

            //    comp.Translation.Raw = new Vec3(100.0f, 100.0f, -100.0f);
            //    importedActor = new Actor<SkeletalMeshComponent>(comp) { Name = "SkeletalMeshActor" };
            //}
            //else
            //{
            //    StaticMeshComponent comp = new StaticMeshComponent(scene._staticModel, null);
            //    comp.Translation.Raw = new Vec3(100.0f, 100.0f, -100.0f);
            //    importedActor = new Actor<StaticMeshComponent>(comp) { Name = "StaticMeshActor" };
            //}

            IActor[] actors = new IActor[]
            {
                //sphereActor,
                floorActor,
                dirLightActor,
                floor2Actor,
                //importedActor,
                //new FlyingCameraPawn(PlayerIndex.One) { Name = "PlayerCamera" },
                //new CharacterPawn(PlayerIndex.One, scene._skeletalModel, scene._skeleton) { Name = "PlayerCharacter", },
            };
            
            _settings.GameMode = new TestGameMode();
            _settings.Maps.Add(new Map(this, new MapSettings(actors)));
            //_settings.AmbientSound = new SoundFile() { SoundPath = desktop + "test.wav" };
            //_settings.AmbientParams.SourceRelative.Value = false;
            //_settings.AmbientParams.ReferenceDistance.Value = 1.0f;
            //_settings.AmbientParams.MaxDistance.Value = 50.0f;
        }
    }
}