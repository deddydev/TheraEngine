using CustomEngine.World;
using System;

namespace CustomEngine.Components
{
    public abstract class Component : ObjectBase
    {
        private Actor _owner;
        public Actor Owner { get { return _owner; } set { _owner = value; } }
    }
}
