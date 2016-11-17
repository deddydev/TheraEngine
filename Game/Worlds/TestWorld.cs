using System;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors;
using CustomEngine.Rendering.Cameras;

namespace Game.Worlds
{
    public class TestWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("TestWorld");

            Model boxModel = new Model();
            Mesh mesh = new Box(new Vec3(-5.0f, -5.0f, -5.0f), new Vec3(5.0f, 5.0f, 5.0f));

            //ResultBasicFunc materialResult = new ResultBasicFunc();

            Shader vert = Shader.TestVertexShader();
            Shader frag = Shader.TestFragmentShader();
            vert.Compile();
            frag.Compile();
            mesh.Material = new Material("Mat_Green", new MaterialSettings(), vert, frag);
            mesh.Material.Compile();

            Skeleton skel = new Skeleton();
            skel.RootBone = new Bone("Root", FrameState.Identity);
            boxModel.Meshes.Add(mesh);
            boxModel.Skeleton = skel;
            
            AnimationInterpNode camPropAnim = new AnimationInterpNode(1);
            camPropAnim.Looped = true;
            camPropAnim.UseKeyframes = true;
            InterpKeyframe start = new InterpKeyframe(0.0f, 0.01f, 0.01f);
            //InterpKeyframe end = new InterpKeyframe(1000.0f, 360.0f, 360.0f);
            camPropAnim.Keyframes.AddFirst(start);
            //camPropAnim.Keyframes.AddLast(end);
            //start.MakeOutLinear();
            //end.MakeInLinear();

            AnimationContainer anim = new AnimationContainer("Anim_AddRotationY", true, camPropAnim);

            ModelComponent modelComp = new ModelComponent(boxModel);
            Camera camera = new PerspectiveCamera();
            camera.CurrentTransform = new FrameState(new Vec3(50,-50,200), Quaternion.Identity, Vec3.One);
            camera.CurrentTransform.AddAnimation(anim);
            anim.Start();
            CameraComponent cameraComp = new CameraComponent(camera);
            cameraComp.Parent = modelComp;
            _settings._maps.Add(new Map(this, new MapSettings(new Pawn(PlayerIndex.One, modelComp))));

            //AnimationInterpNode propertyAnim = new AnimationInterpNode(360);
            //propertyAnim.Looped = true;
            //propertyAnim.UseKeyframes = true;
            //start = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            //end = new InterpKeyframe(360.0f, 360.0f, 360.0f);
            //propertyAnim.Keyframes.AddFirst(start);
            //propertyAnim.Keyframes.AddLast(end);
            //start.MakeOutLinear();
            //end.MakeInLinear();

            //anim = new AnimationContainer("LocalTransform.Anim_SetRotationZ", true, propertyAnim);
            //modelComp.AddAnimation(anim);
            //anim.Start();
        }
    }
}
