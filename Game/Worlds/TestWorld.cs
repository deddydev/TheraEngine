using System;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using CustomEngine.Rendering.Cameras;
using BulletSharp;
using System.Drawing;
using System.Linq;

namespace Game.Worlds
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

            Bone rootBone = new Bone("Root", FrameState.Identity);
            Model model = new Model(new Skeleton(rootBone));
            Mesh mesh = new Sphere(5.0f, Vec3.Zero);
            mesh.Material = Material.GetTestMaterial();
            model.Children.Add(mesh);

            Bone floorBone = new Bone("Root", FrameState.Identity);
            Model floorModel = new Model(new Skeleton(floorBone));
            Mesh floorMesh = new Box(new Vec3(-20.0f, -2.0f, -20.0f), new Vec3(20.0f, 2.0f, 20.0f));
            floorMesh.Material = Material.GetTestMaterial();
            floorModel.Children.Add(floorMesh);

            //BoxShape boxCollisionShape = new BoxShape(5.0f);
            //MotionState state = new DefaultMotionState(Matrix4.CreateTranslation(rootBone.BindMatrix.GetPoint()));
            //RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(10.0f, state, boxCollisionShape);
            //info.AngularDamping = 0.5f;
            //info.LinearDamping = 0.3f;
            //mesh.CollisionObject = new RigidBody(info);

            AnimationInterpNode camPropAnim = new AnimationInterpNode(360, true, true);
            InterpKeyframe first = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            InterpKeyframe second = new InterpKeyframe(180.0f, 360.0f, 360.0f);
            InterpKeyframe last = new InterpKeyframe(360.0f, 0.0f, 0.0f);
            first.LinkNext(second).LinkNext(last);
            camPropAnim.Keyframes.AddFirst(first);
            
            AnimFolder yawAnim = new AnimFolder("Yaw", false, camPropAnim);
            AnimFolder stateFolder = new AnimFolder("Rotation", yawAnim);
            AnimationContainer anim = new AnimationContainer(stateFolder);
            
            ModelComponent modelComp = new ModelComponent(model);
            PointLightComponent lightComp = new PointLightComponent();
            ModelComponent floorComp = new ModelComponent(floorModel);

            lightComp.Translation.Y = 10.0f;
            floorComp.AddAnimation(anim, true);
            floorComp.Translation.Y = -10.0f;

            Actor sphereActor = new Actor(modelComp);
            Actor lightActor = new Actor(lightComp);
            Actor groundActor = new Actor(floorComp);

            _settings._defaultMaps.Add(new Map(this, new MapSettings(sphereActor, lightActor, groundActor, new FlyingCameraPawn(PlayerIndex.One))));
        }
    }
}
