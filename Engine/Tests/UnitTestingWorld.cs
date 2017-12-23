using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes;
using TheraEngine.Worlds.Maps;

namespace TheraEngine.Tests
{
    public class UnitTestingWorld : World
    {
        protected internal override void OnLoaded()
        {
            float margin = 5.0f;
            float radius = 5.0f;
            ColorF4 sphereColor = Color.Red;
            ColorF4 boxColor = Color.Blue;
            float diam = radius * 2.0f;
            float originDist = diam + margin;

            List<IActor> actors = new List<IActor>();

            IActor actor;
            for (int i = -5; i < 5; ++i)
            {
                actor = new SphereActor("TestSphere", radius, new Vec3(i * originDist, 0.0f, 0.0f), Rotator.GetZero(),
                    TMaterial.CreateLitColorMaterial(sphereColor), new TRigidBodyConstructionInfo()
                    {
                        UseMotionState = false,
                    });
                actors.Add(actor);
            }
            for (int i = -5; i < 5; ++i)
            {
                actor = new BoxActor("TestBox", radius, new Vec3(i * originDist, originDist, 0.0f), new Rotator(0.0f, 0.0f, i * 30.0f, RotationOrder.YPR),
                    TMaterial.CreateLitColorMaterial(boxColor), new TRigidBodyConstructionInfo()
                    {
                        UseMotionState = false,
                    });
                actors.Add(actor);
            }
            Settings = new WorldSettings("UnitTestingWorld", new Map(new MapSettings(true, Vec3.Zero, actors)))
            {
                Bounds = new BoundingBox(500.0f),
                OriginRebaseBounds = new BoundingBox(50.0f),
            };
        }
    }
}
