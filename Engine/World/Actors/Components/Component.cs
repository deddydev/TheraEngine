using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class Component : ObjectBase
    {
        private Actor _owner;
        public Actor Owner { get { return _owner; } set { _owner = value; } }
    }
}
