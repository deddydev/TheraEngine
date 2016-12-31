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

namespace Game.Worlds
{
    public unsafe class TestWorld : World
    {
        StaticMeshComponent _sphere;
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
                BodyInfo = new RigidBodyConstructionInfo(50.0f, new DefaultMotionState(
                    Matrix4.CreateTranslation(new Vec3(0.0f, 20.0f, 0.0f))), new SphereShape(1.0f))
                {
                    AngularDamping = 0.05f,
                    LinearDamping = 0.005f,
                    Restitution = 0.2f,
                    Friction = 0.2f,
                    RollingFriction = 0.2f,
                },
                CollisionEnabled = true,
                SimulatePhysics = true,
                Group = CustomCollisionGroup.DynamicWorld,
                CollidesWith = CustomCollisionGroup.StaticWorld,
            };
            Sphere sphere = new Sphere(1.0f);
            StaticMesh sphereModel = new StaticMesh(
                "Sphere", sphere.GetMesh(30.0f, false),
                Material.GetTestMaterial(), sphere);
            PhysicsDriverInfo floorInfo = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(20.0f, new DefaultMotionState(
                    Matrix4.CreateFromAxisAngle(Vec3.Forward, 10.0f)), new BoxShape(new Vec3(20.0f, 0.5f, 20.0f)))
                {

                },
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld,
            };
            BoundingBox floorBox = new BoundingBox(Vec3.Zero, new Vec3(-20.0f, -0.5f, -20.0f), new Vec3(20.0f, 0.5f, 20.0f));
            StaticMesh floorModel = new StaticMesh(
                "Floor", floorBox.GetMesh(false),
                Material.GetTestMaterial(), floorBox);

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
            
            lightComp.Translation.Y = 10.0f;
            lightComp.Translation.X = 10.0f;
            //floorComp.AddAnimation(anim, true);

            Actor sphereActor = new Actor(_sphere = new StaticMeshComponent(sphereModel, sphereInfo, true));
            Actor floorActor = new Actor(new StaticMeshComponent(floorModel, floorInfo, true));
            Actor lightActor = new Actor(lightComp);
            Actor dirLightActor = new Actor(dirLightComp);

            _settings._defaultMaps.Add(new Map(this, new MapSettings(sphereActor, lightActor, floorActor, dirLightActor, new FlyingCameraPawn(PlayerIndex.One))));
        }

        public override void BeginPlay()
        {
            base.BeginPlay();
        }
    }
}
