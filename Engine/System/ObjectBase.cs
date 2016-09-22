using CustomEngine.Rendering.Animation;
using CustomEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.System
{
    public class ObjectBase
    {
        public static List<ObjectBase> _changedObjects = new List<ObjectBase>();

        protected bool _changed;
       
        protected void Changed(MethodBase property)
        {
            Console.WriteLine($"Changed property {property.Name} in {property.ReflectedType.Name}");
            _changed = true;
            _changedObjects.Add(this);
        }

        public WorldBase GetWorld() { return CustomGameForm.Instance._currentWorld; }
        public double DeltaTime { get { return CustomGameForm.Instance.RenderTime; } }

        public void ApplyAnimation(PropertyAnim animation)
        {
            string property = animation.PropertyName;
        }
        public void ApplyAnimation(PropertyAnim animation, float frame)
        {
            string property = animation.PropertyName;
        }
    }
}
