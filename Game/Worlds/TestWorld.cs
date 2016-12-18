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

namespace Game.Worlds
{
    public class TestWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");

            Bone rootBone = new Bone("Root", FrameState.Identity);
            //Bone childBone = new Bone("Child", new FrameState(new Vec3(0.0f, 0.0f, 0.0f), Rotator.GetZero(), Vec3.One));
            //rootBone.Children.Add(childBone);
            Model boxModel = new Model(new Skeleton(rootBone));

            //Vertex p0 = new Vertex(0, new Vec3(-1, -1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //Vertex p1 = new Vertex(1, new Vec3(1, -1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //Vertex p2 = new Vertex(2, new Vec3(1, 1, 0), null, Vec3.UnitZ, new Vec2(0, 0));
            //VertexTriangle triangle = new VertexTriangle(p0, p1, p2);
            //Mesh mesh = new Mesh(PrimitiveData.FromTriangles(Culling.None, new PrimitiveBufferInfo(), triangle));

            Mesh mesh = new Box(10.0f);

            Shader vert = Shader.TestVertexShader();
            Shader frag = Shader.TestFragmentShader();
            mesh.Material = new Material("Mat_Green", new MaterialSettings(), vert, frag);
            boxModel.Children.Add(mesh);

            //BoxShape boxCollisionShape = new BoxShape(5.0f);
            //MotionState state = new DefaultMotionState(Matrix4.CreateTranslation(rootBone.BindMatrix.GetPoint()));
            //RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(10.0f, state, boxCollisionShape);
            //info.AngularDamping = 0.5f;
            //info.LinearDamping = 0.3f;
            //mesh.CollisionObject = new RigidBody(info);

            //AnimationInterpNode camPropAnim = new AnimationInterpNode(360, true, true);
            //InterpKeyframe first = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            //InterpKeyframe second = new InterpKeyframe(180.0f, 360.0f, 360.0f);
            //InterpKeyframe last = new InterpKeyframe(360.0f, 0.0f, 0.0f);
            //first.LinkNext(second).LinkNext(last);
            //camPropAnim.Keyframes.AddFirst(first);
            //first.MakeOutLinear();
            //second.MakeInLinear();
            //second.MakeOutLinear();
            //last.MakeInLinear();

            //AnimFolder yawAnim = new AnimFolder("Yaw", false, camPropAnim);
            //AnimFolder pitchAnim = new AnimFolder("Pitch", false, camPropAnim);
            ////AnimFolder rollAnim = new AnimFolder("AddRotationRoll", true, camPropAnim);
            //AnimFolder stateFolder = new AnimFolder("Rotation", yawAnim, pitchAnim/*, rollAnim*/);
            //AnimationContainer anim = new AnimationContainer(stateFolder);

            ModelComponent modelComp = new ModelComponent(boxModel);
            //modelComp.AddAnimation(anim, true);
            
            _settings._defaultMaps.Add(new Map(this, new MapSettings(new Actor(modelComp), new FlyingCameraPawn(PlayerIndex.One))));
        }
    }
}
