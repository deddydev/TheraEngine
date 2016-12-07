﻿using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLOutput<T> : BaseGLOutput where T : GLVar
    {
        public GLOutput(string name, MaterialFunction parent) : base(name, parent) { }
        public GLOutput(string name) : base(name) { }

        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToPoint(Vec2 otherPoint, float time)
        {
            Vec2 p0 = Location;

            Vec2 p1 = p0;
            p1.X += 10.0f;

            Vec2 p3 = otherPoint;

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
        public Vec2 BezierToInputArg(int argIndex, float time)
        {
            if (_connectedTo == null || 
                argIndex >= _connectedTo.Count ||
                argIndex < 0 || 
                _connectedTo[argIndex] == null)
                return Location;

            return BezierToPoint(_connectedTo[argIndex].Location, time);
        }
    }
}
