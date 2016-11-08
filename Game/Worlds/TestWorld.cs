using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds.Maps;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Animation;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering.Models.Materials;

namespace Game.Worlds
{
    public class TestWorld : World
    {
        Gamepad g;
        public TestWorld() : base(new WorldSettings("TestWorld"))
        {
            Model boxModel = new Model();
            Mesh mesh = new Box(new Vec3(-20.0f, -20.0f, -20.0f), new Vec3(20.0f, 20.0f, 20.0f));

            //ResultBasicFunc materialResult = new ResultBasicFunc();

            Shader vert = Shader.TestVertexShader();
            Shader frag = Shader.TestFragmentShader();
            //vert.Generate(materialResult);
            //frag.Generate(materialResult);

            mesh.Material = new Material("Mat_Green", new MaterialSettings(), vert, frag);

            Skeleton skel = new Skeleton();
            skel.RootBone = new Bone("Root", FrameState.Identity);
            boxModel.AddMesh(mesh);
            boxModel.Skeleton = skel;

            ModelComponent modelComp = new ModelComponent(boxModel);
            Actor actor = new Actor(modelComp);
            _settings._maps.Add(new Map(new MapSettings(actor)));

            AnimationInterpNode propertyAnim = new AnimationInterpNode(360);
            InterpKeyframe start = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            InterpKeyframe end = new InterpKeyframe(360.0f, 360.0f, 360.0f);
            propertyAnim.Keyframes.AddFirst(start);
            propertyAnim.Keyframes.AddLast(end);
            start.MakeOutLinear();
            end.MakeInLinear();
            
            modelComp.AddAnimation(new AnimationContainer("Transform.EulerRotation.Z", propertyAnim));

            SpawnActor(actor);
        }
    }
}
