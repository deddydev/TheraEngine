using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class GenericPrimitiveComponent : GenericSceneComponent
    {
        public GenericPrimitiveComponent() { }
        public GenericPrimitiveComponent(RenderableObjectContainer obj) { _primitive = obj; }

        private RenderableObjectContainer _primitive;
        internal RenderableObjectContainer Primitive
        {
            get { return _primitive; }
            set
            {
                if (_primitive == value)
                    return;
                RenderableObjectContainer oldPrim = _primitive;
                _primitive = value;
                if (oldPrim != null)
                    oldPrim.LinkedComponent = null;
                if (_primitive != null)
                    _primitive.LinkedComponent = this;
            }
        }
    }
}
