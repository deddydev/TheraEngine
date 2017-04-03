using System;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering.Animation;
using CustomEngine.Worlds.Actors;
using System.Drawing;
using CustomEngine.Rendering;
using BulletSharp;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds
{
    public unsafe class TestWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");

            #region Sphere
            PhysicsDriverInfo sphereInfo = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(
                    50.0f,
                    new DefaultMotionState(/*Matrix4.CreateTranslation(new Vec3(0.0f, 20.0f, 0.0f))*/),
                    new SphereShape(1.0f))
                {
                    AngularDamping = 0.05f,
                    LinearDamping = 0.005f,
                    Restitution = 0.9f,
                    Friction = 0.01f,
                    RollingFriction = 0.01f,
                },
                CollisionEnabled = true,
                SimulatePhysics = true,
                Group = CustomCollisionGroup.DynamicWorld,
                CollidesWith = CustomCollisionGroup.StaticWorld,
            };
            Sphere sphere = new Sphere(1.0f, Vec3.Zero);
            StaticMesh sphereModel = new StaticMesh("Sphere", sphere);
            sphereModel.RigidChildren.Add(new StaticRigidSubMesh(
                sphere.GetMesh(30.0f, false),
                sphere,
                Material.GetDefaultMaterial(),
                "Mesh"));
            Actor sphereActor = new Actor(new StaticMeshComponent(
                sphereModel,
                new Vec3(0.0f, 20.0f, 0.0f),
                Rotator.GetZero(),
                Vec3.One,
                sphereInfo))
            {
                Name = "SphereActor"
            };
            #endregion

            #region Floor
            PhysicsDriverInfo floorInfo = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(
                    20.0f, null,
                    /*new DefaultMotionState(Matrix4.CreateFromAxisAngle(Vec3.Forward, 10.0f)),*/
                    new BoxShape(new Vec3(20.0f, 0.5f, 20.0f)))
                {
                    Restitution = 0.5f,
                },
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld,
            };
            BoundingBox floorBox = new BoundingBox(new Vec3(20.0f, 0.5f, 20.0f), Vec3.Zero);
            StaticMesh floorModel = new StaticMesh("Floor", floorBox);
            floorModel.RigidChildren.Add(new StaticRigidSubMesh(floorBox.GetMesh(false), floorBox, Material.GetDefaultMaterial(), "FloorMesh"));
            StaticMeshComponent floorComp = new StaticMeshComponent(
                floorModel, Vec3.Zero,
                new Rotator(0.0f, 0.0f, 0.0f, Rotator.Order.YPR),
                Vec3.One, floorInfo);
            Actor floorActor = new Actor(floorComp)
            {
                Name = "Floor"
            };
            #endregion

            #region Skylight
            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                Color.Beige, 1.0f, 0.6f, new Rotator(-90.0f, 0.0f, 0.0f, Rotator.Order.YPR));
            dirLightComp.Translation.Y = 30.0f;

            AnimationScalar lightAnim = new AnimationScalar(360, true, true);
            ScalarKeyframe first2 = new ScalarKeyframe(0.0f, 0.0f, 0.0f);
            ScalarKeyframe last2 = new ScalarKeyframe(360.0f, 360.0f, 0.0f);
            first2.LinkNext(last2);
            first2.MakeOutLinear();
            last2.MakeInLinear();
            lightAnim.Keyframes.AddFirst(first2);
            AnimFolder lightPitchFolder = new AnimFolder("Pitch", false, lightAnim);
            AnimFolder lightRotationFolder = new AnimFolder("Rotation", lightPitchFolder);
            AnimationContainer lightAnimContainer = new AnimationContainer(lightRotationFolder);
            dirLightComp.AddAnimation(lightAnimContainer, false);

            Actor<DirectionalLightComponent> dirLightActor = new Actor<DirectionalLightComponent>(dirLightComp) { Name = "SunLight" };
            #endregion

            string desktop = Environment.MachineName == "DAVID-DESKTOP" ?
                "X:\\Desktop\\" :
                "C:\\Users\\David\\Desktop\\";
            Collada.ImportOptions options = new Collada.ImportOptions();
            ModelScene scene = Collada.Import(desktop + "TEST.DAE", options);

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
                sphereActor,
                floorActor,
                dirLightActor,
                //importedActor,
                new FlyingCameraPawn(PlayerIndex.One) { Name = "PlayerCamera" },
                new CharacterPawn(PlayerIndex.Two, scene._skeletalModel, scene._skeleton) { Name = "PlayerCharacter", },
            };

            _settings._maps.Add(new Map(this, new MapSettings(actors)));
        }
    }
}
