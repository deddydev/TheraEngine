using CustomEngine.GameModes;
using System;
using System.Collections.Generic;

namespace CustomEngine.Worlds
{
    public class WorldSettings
    {
        public Box OriginRebaseBounds { get { return _originRebaseBounds; } }
        public Box WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public Vec3 Gravity { get { return _gravity; } set { _gravity = value; } }
        public GameMode GameMode { get { return _gameMode; } set { _gameMode = value; } }

        private Box _worldBounds;
        private Box _originRebaseBounds;
        private Vec3 _gravity;
        private GameMode _gameMode;
        public List<Map> _maps;

        public WorldSettings()
        {
            _worldBounds = new Box(new Vec3(-5000.0f), new Vec3(5000.0f));
            _originRebaseBounds = _worldBounds;
            _gravity = new Vec3(0.0f, -9.81f, 0.0f);
            _gameMode = new GameMode();
            _maps = new List<Map>();
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new Box(distance, distance, distance);
        }
    }
}
