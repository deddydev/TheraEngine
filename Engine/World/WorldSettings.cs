using System;

namespace CustomEngine.Worlds
{
    public class WorldSettings
    {
        public Box OriginRebaseBounds { get { return _originRebaseBounds; } }
        public Box WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public Vec3 Gravity { get { return _gravity; } set { _gravity = value; } }
        
        private Box _worldBounds;
        private Box _originRebaseBounds;
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new Box(distance, distance, distance);
        }
    }
}
