using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Worlds.Actors.Types
{
    public class LandscapeActor : Actor
    {
        Vec2 _dimensions;
        int _xCount, _yCount;

        PositionComponent Root { get { return RootComponent as PositionComponent; } }

        float BaseHeight { get { return Center.Y; } set { Center = new Vec3(Center.X, value, Center.Z); } }
        Vec3 Center
        {
            get { return Root.Translation; }
            set { Root.Translation = value; }
        }

        protected override SceneComponent SetupComponents() { return new PositionComponent(); }
    }
}
