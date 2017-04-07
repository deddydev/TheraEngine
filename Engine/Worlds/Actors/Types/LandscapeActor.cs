using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
using BulletSharp;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Types
{
    public class LandscapeActor : Actor<PositionComponent>
    {
        Size _dimensions = new Size(100, 100);

        public LandscapeActor()
        {
            HeightfieldTerrainShape shape = new HeightfieldTerrainShape(_dimensions.Width, _dimensions.Height, null, 1.0f, 0.0f, 1.0f, 1, PhyScalarType.Single, false);
        }

        float BaseHeight { get { return Center.Y; } set { Center = new Vec3(Center.X, value, Center.Z); } }
        Vec3 Center
        {
            get { return RootComponent.Translation; }
            set { RootComponent.Translation = value; }
        }

        protected override PositionComponent OnConstruct()
        {
            return new PositionComponent();
        }
    }
}
