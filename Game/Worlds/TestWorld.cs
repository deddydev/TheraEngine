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

namespace Game.Worlds
{
    public class TestWorld : World
    {
        public TestWorld() : base(new WorldSettings("TestWorld"))
        {
            Model boxModel = new Model();
            Mesh mesh = new Box(new Vec3(-20.0f, -20.0f, -20.0f), new Vec3(20.0f, 20.0f, 20.0f));
            Skeleton skel = new Skeleton();
            skel.RootBone = new Bone("Root", FrameState.Identity);
            boxModel.AddMesh(mesh);
            boxModel.SetSkeleton(skel);

            ModelComponent modelComp = new ModelComponent(boxModel);
            Actor actor = new Actor(modelComp);

            Map map = new Map(new MapSettings(true, Vec3.Zero, actor));
            _settings._maps.Add(map);

            AnimationInterpNode propertyAnim = new AnimationInterpNode(360);
            InterpKeyframe start = new InterpKeyframe(0.0f, 0.0f, 0.0f);
            InterpKeyframe end = new InterpKeyframe(360.0f, 360.0f, 360.0f);
            propertyAnim.Keyframes.AddFirst(start);
            propertyAnim.Keyframes.AddLast(end);
            start.MakeOutLinear();
            end.MakeInLinear();
            AnimFolder root = new AnimFolder("Transform", null, new AnimFolder("EulerRotation", null, new AnimFolder("Z", propertyAnim)));
            AnimationContainer anim = new AnimationContainer(root);
            modelComp.AddAnimation(anim);
        }
    }
}
