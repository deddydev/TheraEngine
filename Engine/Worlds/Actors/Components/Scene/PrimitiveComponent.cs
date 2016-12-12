using CustomEngine.Rendering.Models;
using System;
using System.Reflection;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class PrimitiveComponent : TRSComponent
    {
        public PrimitiveComponent() { }
        public PrimitiveComponent(IRenderableObjectContainer obj) { _primitive = obj; }

        private IRenderableObjectContainer _primitive;
        internal IRenderableObjectContainer Primitive
        {
            get { return _primitive; }
            set
            {
                if (_primitive == value)
                    return;
                IRenderableObjectContainer oldPrim = _primitive;
                _primitive = value;
                if (oldPrim != null)
                    oldPrim.LinkedComponent = null;
                if (_primitive != null)
                {
                    _primitive.WorldMatrix = WorldMatrix;
                    _primitive.InverseWorldMatrix = InverseLocalMatrix;
                    _primitive.LinkedComponent = this;
                }
                else
                {
                    _primitive.WorldMatrix = Matrix4.Identity;
                    _primitive.InverseWorldMatrix = Matrix4.Identity;
                }
            }
        }
        public override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            if (_primitive != null)
            {
                _primitive.WorldMatrix = WorldMatrix;
                _primitive.InverseWorldMatrix = InverseWorldMatrix;
            }
        }
    }
}
