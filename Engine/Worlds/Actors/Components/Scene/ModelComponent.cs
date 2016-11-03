using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ModelComponent : PrimitiveComponent<Model>
    {
        public ModelComponent() { }
        public ModelComponent(Model m) { Model = m; }

        public Model Model
        {
            get { return _primitive; }
            set
            {
                if (_primitive == value)
                    return;
                if (_primitive != null)
                    _primitive.UnlinkComponent(this);
                if ((_primitive = value) != null)
                    _primitive.LinkComponent(this);
            }
        }
    }
}
