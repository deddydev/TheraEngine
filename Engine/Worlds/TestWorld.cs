using System;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using BulletSharp;
using System.Drawing;
using System.Linq;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models.Collada;

namespace CustomEngine.Worlds
{
    public unsafe class TestWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");

            //Bone childBone = new Bone("Child", new FrameState(new Vec3(0.0f, 0.0f, 0.0f), Rotator.GetZero(), Vec3.One));
            //rootBone.Children.Add(childBone);

            //Vertex p0 = new Vertex(new Vec3(-1, -1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //Vertex p1 = new Vertex(new Vec3(1, -1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //Vertex p2 = new Vertex(new Vec3(1, 1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //VertexTriangle triangle = new VertexTriangle(p0, p1, p2);
            //Mesh mesh = new Mesh(PrimitiveData.FromTriangles(Culling.None, new PrimitiveBufferInfo(), triangle));

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
            StaticMesh sphereModel = new StaticMesh(
                "Sphere", sphere.GetMesh(30.0f, false),
                Material.GetDefaultMaterial(), sphere);
            PhysicsDriverInfo floorInfo = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(
                    20.0f, null,
                    /*new DefaultMotionState(Matrix4.CreateFromAxisAngle(Vec3.Forward, 10.0f)),*/
                    new BoxShape(new Vec3(20.0f, 0.5f, 20.0f)))
                {
                    Restitution = 0.3f,
                },
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld,
            };
            BoundingBox floorBox = new BoundingBox(new Vec3(-20.0f, -0.5f, -20.0f), new Vec3(20.0f, 0.5f, 20.0f));
            StaticMesh floorModel = new StaticMesh(
                "Floor", floorBox.GetMesh(false),
                Material.GetDefaultMaterial(), floorBox);

            AnimationInterpNode camPropAnim = new AnimationInterpNode(360, true, true);
            InterpKeyframe first = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            InterpKeyframe second = new InterpKeyframe(180.0f, 360.0f, 360.0f);
            InterpKeyframe last = new InterpKeyframe(360.0f, 0.0f, 0.0f);
            first.LinkNext(second).LinkNext(last);
            camPropAnim.Keyframes.AddFirst(first);
            
            AnimFolder yawAnim = new AnimFolder("Yaw", false, camPropAnim);
            AnimFolder stateFolder = new AnimFolder("Rotation", yawAnim);
            AnimationContainer anim = new AnimationContainer(stateFolder);

            PointLightComponent lightComp = new PointLightComponent(1.0f, 0.0f, 0.5f, Color.Beige, 1.0f, 0.2f);
            DirectionalLightComponent dirLightComp = new DirectionalLightComponent(
                new Rotator(-90.0f, 0.0f, 0.0f, Rotator.Order.YPR), Color.DarkGray, 1.0f, 0.2f);
            
            StaticMeshComponent floorComp = new StaticMeshComponent(
                floorModel, Vec3.Zero,
                new Rotator(0.0f, 0.0f, -10.0f, Rotator.Order.YPR),
                Vec3.One, floorInfo);

            lightComp.Translation.Y = 0.0f;
            lightComp.Translation.X = 0.0f;
            floorComp.AddAnimation(anim, true);

            Actor sphereActor = new Actor(new StaticMeshComponent(
                sphereModel,
                new Vec3(0.0f, 20.0f, 0.0f),
                Rotator.GetZero(),
                Vec3.One,
                sphereInfo));

            Actor floorActor = new Actor(floorComp);

            Actor lightActor = new Actor(lightComp);
            Actor dirLightActor = new Actor(dirLightComp);

            ColladaImportOptions options = new ColladaImportOptions();
            SkeletalMesh skelM;
            StaticMesh staticM;
            Skeleton skeleton;
            Collada.ImportModel(
                Environment.MachineName == "DAVID-DESKTOP" ? "X:\\Desktop\\TEST.DAE" : "C:\\Users\\David\\Desktop\\TEST.DAE", 
                options, out staticM, out skelM, out skeleton);

            Actor skelActor;
            if (skelM != null)
            {
                SkeletalMeshComponent skelComp = new SkeletalMeshComponent(skelM, skeleton);
                skelActor = new Actor(skelComp);
            }
            else
            {
                StaticMeshComponent skelComp = new StaticMeshComponent(staticM,
                    new PhysicsDriverInfo()
                    {
                        BodyInfo = new RigidBodyConstructionInfo(1.0f, null, new SphereShape(1.0f))
                    });
                skelActor = new Actor(skelComp);
            }

            _settings._defaultMaps.Add(new Map(this, new MapSettings(/*sphereActor,*/ lightActor, /*floorActor,*/ dirLightActor, skelActor, new FlyingCameraPawn(PlayerIndex.One))));
        }
    }
}
