using System;

namespace CustomEngine.Worlds
{
    public class WorldSettings
    {
        public Box OriginRebaseBounds { get { return _originRebaseBounds; } }
        public Box WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }

        private Box _worldBounds;
        private Box _originRebaseBounds;
        private float _gravity;

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new Box(distance, distance, distance);
        }
    }
}
