using System;
using System.Drawing;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLInput<T> : BaseGLInput where T : GLVar
    {
        public GLInput(string name) : base(name) { }
        
        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierFromPoint(Vec2 otherPoint, float time)
        {
            Vec2 p0 = otherPoint;

            Vec2 p1 = p0;
            p1.X += 10.0f;

            Vec2 p3 = Location;

            Vec2 p2 = p3;
            p2.X -= 10.0f;

            return CustomMath.Bezier(p0, p1, p2, p3, time);
        }
        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierFromOutputArg(float time)
        {
            if (_connectedTo == null)
                return Location;

            return BezierFromPoint(_connectedTo.Location, time);
        }
    }
}
