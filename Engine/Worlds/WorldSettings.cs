using CustomEngine.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Worlds
{
    public class WorldSettings
    {
        public Box OriginRebaseBounds { get { return _originRebaseBounds; } }
        public Box WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public Vec3 Gravity { get { return _gravity; } set { _gravity = value; } }
        public GameMode GameMode { get { return _gameMode; } set { _gameMode = value; } }

        private Box _worldBounds = new Box(new Vec3(-5000.0f), new Vec3(5000.0f));
        private Box _originRebaseBounds;
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        private GameMode _gameMode;
        public List<Map> _maps;

        public WorldSettings(params Map[] maps)
        {
            _maps = maps.ToList();
            _originRebaseBounds = _worldBounds;
        }

        public WorldSettings()
        {
            _originRebaseBounds = _worldBounds;
            _maps = new List<Map>();
        }

        public void SetGameMode(GameMode mode)
        {
            _gameMode = mode;
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new Box(distance, distance, distance);
        }
    }
}
