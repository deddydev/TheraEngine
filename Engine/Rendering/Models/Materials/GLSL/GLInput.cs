using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLInput : BaseGLArgument
    {
        public override bool IsOutput { get { return false; } }
        public GLOutput ConnectedTo
        {
            get { return _connectedTo; }
            set { TryConnectTo(value); }
        }
        protected GLOutput _connectedTo;

        public GLInput(string name, params GLTypeName[] types) : base(name)
        {
            _allowedArgTypes = types;
        }
        public GLInput(string name, MaterialFunction parent, params GLTypeName[] types) : base(name, parent)
        {
            _allowedArgTypes = types;
        }
        public GLInput(string name, BaseGLArgument linkedMultiArg) : base(name)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }
        public GLInput(string name, MaterialFunction parent, BaseGLArgument linkedMultiArg) : base(name, parent)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }

        public bool TryConnectTo(GLOutput other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        internal virtual void DoConnection(GLOutput other)
        {
            _connectedTo?.ClearConnection(this);
            _connectedTo = other;
            _connectedTo?.DoConnection(this);
        }
        internal virtual void ClearConnection()
        {
            _connectedTo?.ClearConnection(this);
            _connectedTo = null;
        }

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

            Vec2 p3 = Translation;

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
                return Translation;

            return BezierFromPoint(_connectedTo.Translation, time);
        }
        public Vec2[] BezierPointsFromPoint(Vec2 otherPoint, int count)
        {
            Vec2 p0 = otherPoint;

            Vec2 p1 = p0;
            p1.X += 10.0f;

            Vec2 p3 = Translation;

            Vec2 p2 = p3;
            p2.X -= 10.0f;

            return CustomMath.GetBezierPoints(p0, p1, p2, p3, count);
        }
        public Vec2[] BezierPointsFromOutputArg(Vec2 otherPoint, int count)
        {
            if (_connectedTo == null)
                return null;

            return BezierPointsFromPoint(_connectedTo.Translation, count);
        }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null)
                return true;

            if (other.IsOutput == IsOutput)
                return false;

            GLTypeName otherType = other.CurrentArgumentType;

            //Edge case: the other node is just invalid
            if (otherType == GLTypeName.Invalid)
                return false;

            GLTypeName thisType = CurrentArgumentType;
            if (thisType != GLTypeName.Invalid)
            {
                if (otherType != GLTypeName.Invalid)
                    return thisType == otherType;

                //Has to be a GLMultiArgument as per the edge case check above
                GLInput otherMultiArg = (GLInput)other;

                return otherMultiArg.AllowedArgumentTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType != GLTypeName.Invalid)
                    return AllowedArgumentTypes.Contains(otherType);

                //Has to be a GLMultiArgument as per the edge case check above
                GLInput otherMultiArg = (GLInput)other;

                //Returns true if there are any matching allowed types between the two
                return AllowedArgumentTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
