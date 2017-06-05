using System;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using CustomEngine.Worlds.Maps;
using System.Drawing;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors.Types;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Tests
{
    public unsafe class TestWorld : World
    {
        public static Collada.Scene ColladaScene;
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");
            Random r = new Random();
            IActor[] array = new IActor[100];
            for (int i = 0; i < array.Length; ++i)
            {
                PhysicsConstructionInfo sphereInfo = new PhysicsConstructionInfo()
                {
                    Mass = 50.0f,
                    AngularDamping = 0.05f,
                    LinearDamping = 0.005f,
                    Restitution = 0.8f,
                    Friction = 0.08f,
                    RollingFriction = 0.01f,
                    CollisionEnabled = true,
                    SimulatePhysics = true,
                    Group = CustomCollisionGroup.DynamicWorld,
                    CollidesWith = CustomCollisionGroup.StaticWorld | CustomCollisionGroup.DynamicWorld,
                };
                //SphereActor sphereActor = new SphereActor(
                //   "Sphere",
                //   sphereInfo,
                //   2.0f,
                //   new Vec3(0.0f, 20.0f, -100.0f),
                //   Rotator.GetZero(),
                //   Material.GetDefaultMaterial());
                float x = ((float)r.NextDouble() - 0.5f) * 2.0f * 18.0f;
                float y = ((float)r.NextDouble() - 0.5f) * 2.0f * 30.0f;
                float z = ((float)r.NextDouble() - 0.5f) * 2.0f * 18.0f;
                BoxActor b = new BoxActor(
                    "Box" + i,
                    sphereInfo,
                    new Vec3(2.0f),
                    new Vec3(5.0f + x, 50.0f + y, -100.0f + z),
                    new Rotator(0.0f, 0.0f, 0.0f, Rotator.Order.YPR),
                    Material.GetDefaultMaterial());
                array[i] = b;
            }
            //sphereActor.RootComponent.WorldTransformChanged += RootComponent_WorldTransformChanged;

            PhysicsConstructionInfo floorInfo = new PhysicsConstructionInfo()
            {
                Mass = 20.0f,
                Restitution = 0.5f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.Characters | CustomCollisionGroup.DynamicWorld,
            };
            BoxActor floorActor = new BoxActor(
                "Floor",
                floorInfo,
                new Vec3(20.0f, 0.5f, 20.0f),
                new Vec3(5.0f, 0.0f, -100.0f),
                new Rotator(0.0f, 0.0f, -5.0f, Rotator.Order.YPR),
                Material.GetDefaultMaterial());

            //PhysicsConstructionInfo floor2Info = new PhysicsConstructionInfo()
            //{
            //    Mass = 20.0f,
            //    Restitution = 0.5f,
            //    CollisionEnabled = true,
            //    SimulatePhysics = false,
            //    Group = CustomCollisionGroup.StaticWorld,
            //    CollidesWith = CustomCollisionGroup.Characters,
            //};
            //Material floor2Mat = Material.GetDefaultMaterial();
            //((GLVec4)floor2Mat.Parameters[0]).Value = (ColorF4)Color.Gray;
            //BoxActor floor2Actor = new BoxActor(
            //    "Floor2",
            //    floor2Info, 
            //    new Vec3(2000.0f, 0.5f, 2000.0f),
            //    new Vec3(0.0f, -20.0f, 0.0f),
            //    new Rotator(30.0f, 00.0f, 0.0f, Rotator.Order.YPR),
            //    floor2Mat);

            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                Color.DarkGray, 1.0f, 0.6f, new Rotator(-45.0f, 0.0f, 0.0f, Rotator.Order.YPR));
            dirLightComp.Translation.Y = 30.0f;

            //PropAnimFloat lightAnim = new PropAnimFloat(360, true, true);
            //FloatKeyframe first2 = new FloatKeyframe(0.0f, 0.0f, 0.0f, PlanarInterpType.Linear);
            //FloatKeyframe last2 = new FloatKeyframe(360.0f, 360.0f, 0.0f, PlanarInterpType.Linear);
            //first2.Link(last2);
            //lightAnim.Keyframes.Add(first2);
            //AnimationContainer lightAnimContainer = new AnimationContainer("Rotation.Yaw", false, lightAnim);
            //dirLightComp.AddAnimation(lightAnimContainer, true);
            //floorActor.RootComponent.AddAnimation(lightAnimContainer, true);

            Actor<DirectionalLightComponent> dirLightActor = new Actor<DirectionalLightComponent>(dirLightComp) { Name = "SunLight" };

            string desktop = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Desktop\\" :
                "C:\\Users\\David\\Desktop\\";
            string googleDrive = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Cloud Storage\\Google Drive\\Game\\" :
                "C:\\Users\\David\\Google Drive\\Game\\";

            Collada.ImportOptions options = new Collada.ImportOptions();
            options.InitialTransform.Scale = new Vec3(1.0f.InchesToMeters());
            //ColladaScene = Collada.Import(desktop + "TEST.DAE", options, false, true);

            ColladaScene = Collada.Import(desktop + "skybox.dae", options, false, true);
            StaticMeshComponent c = new StaticMeshComponent(ColladaScene.StaticModel, null);
            Actor<StaticMeshComponent> skybox = new Actor<StaticMeshComponent>(c);

            //Engine.Settings.Export(desktop, "EngineSettings", Files.FileFormat.Binary);

            //ColladaScene.SkeletalModel.Export(Engine.ContentFolderAbs, "TESTMESH", FileFormat.Binary);
            //foreach (SkeletalRigidSubMesh mesh in ColladaScene.SkeletalModel.RigidChildren)
            //    mesh.Data.ExportReference(Engine.ContentFolderAbs + "TESTMESH\\", mesh.Name + "_Prims", FileFormat.Binary);
            //ColladaScene.Skeleton.Export(Engine.ContentFolderAbs, "TESTSKEL", FileFormat.Binary);

            //Collada.Scene anims = Collada.Import(googleDrive + "Thera Assets\\Characters\\Temp\\Carly_Idle.dae", options, true, false);
            //anims.CleanAnimations(scene._skeletalModel, scene._skeleton);

            //IActor importedActor;
            //if (scene._skeletalModel != null)
            //{
            //    SkeletalMeshComponent comp = new SkeletalMeshComponent(scene._skeletalModel, scene._skeleton);

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

            //CharacterSpawnPointActor spawn = new CharacterSpawnPointActor();
            //spawn.RootComponent.Translation.Raw = new Vec3(0.0f, 20.0f, 0.0f);

            //int resolution = 200;

            //Actor<SplineComponent> splineActor = new Actor<SplineComponent>();
            //PropAnimVec3 spline = new PropAnimVec3(resolution, true, true);

            //Vec3 startPoint = new Vec3(-20.0f, 70.0f, 0.0f);
            //Vec3 midPoint = new Vec3(0.0f, 40.0f, 0.0f);
            //Vec3 endPoint = new Vec3(20.0f, 20.0f, -20.0f);
            //Vec3 left = Vec3.Left * 40.0f;
            //Vec3 right = Vec3.Right * 40.0f;

            //Vec3Keyframe first3 = new Vec3Keyframe(0.0f, startPoint, startPoint, left, right, PlanarInterpType.CubicBezier);
            //Vec3Keyframe second3 = new Vec3Keyframe(resolution / 2.0f, midPoint, midPoint, left, right, PlanarInterpType.CubicBezier);
            //Vec3Keyframe last3 = new Vec3Keyframe(resolution, endPoint, endPoint, left, right, PlanarInterpType.CubicBezier);
            //spline.Keyframes.Add(first3);
            //spline.Keyframes.Add(second3);
            //spline.Keyframes.Add(last3);
            //splineActor.RootComponent.RenderTangents = false;
            //splineActor.RootComponent.Spline = spline;

            cam = new PerspectiveCameraActor();
            cam.Camera.LocalPoint = new Vec3(0.0f, 50.0f, 0.0f);
            //AnimationContainer cameraAnim = new AnimationContainer("LocalPoint", false, spline);
            //camera.RootComponent.Camera.AddAnimation(cameraAnim, true);
            cam.RootComponent.Camera.ViewTarget = new Vec3(0.0f, 0.0f, 0.0f);

            //cam.RootComponent.SetCurrentForPlayer(PlayerIndex.One);

            IActor[] actors = new IActor[]
            {
                //sphereActor,
                floorActor,
                dirLightActor,
                skybox,
                //splineActor,
                //camera,
                //floor2Actor,
                //spawn,
                //importedActor,
                new FlyingCameraPawn(PlayerIndex.One) { Name = "PlayerCamera" },
                //new CharacterPawn(PlayerIndex.One, scene._skeletalModel, scene._skeleton) { Name = "PlayerCharacter", },
            };
            
            _settings.GameMode = new GameMode<FlyingCameraPawn>();
            _settings.Maps.Add(new Map(this, new MapSettings(actors)));
            _settings.Maps[0].Settings._defaultActors.AddRange(array);

            //Export(Engine.ContentFolderAbs, "TestWorld", FileFormat.XML);

            //_settings.AmbientSound = new SoundFile() { SoundPath = desktop + "test.wav" };
            //_settings.AmbientParams.SourceRelative.Value = false;
            //_settings.AmbientParams.ReferenceDistance.Value = 1.0f;
            //_settings.AmbientParams.MaxDistance.Value = 50.0f;
        }
        BoxActor sphereActor;
        PerspectiveCameraActor cam;
        private void RootComponent_WorldTransformChanged()
        {
            cam.Camera.ViewTarget.Raw = sphereActor.RootComponent.GetWorldPoint();
        }
    }
}