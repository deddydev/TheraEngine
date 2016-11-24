using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Types
{
    public class LandscapeActor : Actor
    {
        Vec2 _dimensions;
        int _xCount, _yCount;

        Vec3 Center
        {
            get { return RootComponent.WorldMatrix.GetPoint(); }

        }
        float BaseHeight { get { return _center.Y; } set { _center.Y = value; } }

        public float GetHeight(Vec2 position)
        {

        }
    }
}
