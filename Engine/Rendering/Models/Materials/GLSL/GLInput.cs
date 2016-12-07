using System;
using System.Drawing;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLInput<T> : BaseGLArgument where T : GLVar
    {
        public GLInput(string name) : base(name) { }

        public override bool IsOutput { get { return false; } }
        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        protected BaseGLArgument _connectedTo = null;
        public BaseGLArgument ConnectedTo { get { return _connectedTo; } }

        public override void ClearConnection(BaseGLArgument other)
        {
            if (_connectedTo == null || _connectedTo != other)
                return;

            BaseGLArgument o = _connectedTo;
            _connectedTo = null;
            o.ClearConnection(this);
        }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            return other != null && GetArgType() == other.GetArgType() && IsOutput != other.IsOutput;
        }
        protected override void DoConnection(BaseGLArgument other)
        {
            if (_connectedTo != null)
                _connectedTo.ClearConnection(this);
            _connectedTo = other;
            _connectedTo?.TryConnectTo(this);
        }

        public Vec2 GetBezierPoint(float t)
        {
            if (_connectedTo == null)
                return Location;
            
            float invT = 1.0f - t;
            float invT2 = invT * invT;
            float invT3 = invT2 * invT;
            float t2 = t * t;
            float t3 = t2 * t;

            Vec2 p0 = _connectedTo.Location;

            Vec2 p1 = p0;
            p1.X += 10.0f;

            Vec2 p3 = Location;
            Vec2 p2 = p3;
            p2.X -= 10.0f;

            return 
                p0 * invT3 + 
                p1 * 3.0f * invT2 * t +
                p2 * 3.0f * invT * t2 +
                p3 * t3;
        }
    }
}
