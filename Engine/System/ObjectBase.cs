using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    public class ObjectBase
    {
        public static List<ObjectBase> _changedObjects = new List<ObjectBase>();

        protected bool _changed;

        public AbstractRenderer Renderer { get { return Engine.Renderer; } }
       
        protected void Changed(MethodBase property)
        {
            Console.WriteLine($"Changed property {property.Name} in {property.ReflectedType.Name}");
            _changed = true;
            _changedObjects.Add(this);
        }

        public virtual void Update() { }
    }
}
