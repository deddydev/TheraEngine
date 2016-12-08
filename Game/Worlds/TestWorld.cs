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
            Bone childBone = new Bone("Child", new FrameState(new Vec3(0.0f, 25.0f, 0.0f), Rotator.GetZero(), Vec3.One));
            rootBone.Children.Add(childBone);
            Model boxModel = new Model(new Skeleton(rootBone));

            Mesh mesh = new Box(10.0f);

            Shader vert = Shader.TestVertexShader();
            Shader frag = Shader.TestFragmentShader();
            mesh.Material = new Material("Mat_Green", new MaterialSettings(), vert, frag);
            boxModel.Meshes.Add(mesh);

            BoxShape boxCollisionShape = new BoxShape(5.0f);
            MotionState state = new DefaultMotionState(Matrix4.CreateTranslation(rootBone.BindMatrix.GetPoint()));
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(10.0f, state, boxCollisionShape);
            info.AngularDamping = 0.5f;
            info.LinearDamping = 0.3f;
            mesh.CollisionObject = new RigidBody(info);

            AnimationInterpNode camPropAnim = new AnimationInterpNode(360, true, true);
            InterpKeyframe first = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            InterpKeyframe second = new InterpKeyframe(180.0f, 360.0f, 360.0f);
            InterpKeyframe last = new InterpKeyframe(360.0f, 0.0f, 0.0f);
            first.LinkNext(second).LinkNext(last);
            camPropAnim.Keyframes.AddFirst(first);
            
            AnimFolder yawAnim = new AnimFolder("SetRotationYaw", true, camPropAnim);
            //AnimFolder pitchAnim = new AnimFolder("AddRotationPitch", true, camPropAnim);
            //AnimFolder rollAnim = new AnimFolder("AddRotationRoll", true, camPropAnim);
            AnimFolder stateFolder = new AnimFolder("LocalFrameState", yawAnim/*, pitchAnim, rollAnim*/);
            AnimationContainer anim = new AnimationContainer(stateFolder);

            ModelComponent modelComp = new ModelComponent(boxModel);
            modelComp.AddAnimation(anim, true);
            
            _settings._maps.Add(new Map(this, new MapSettings(new Actor(modelComp), new FlyingCameraPawn(PlayerIndex.One))));
        }
    }
}
