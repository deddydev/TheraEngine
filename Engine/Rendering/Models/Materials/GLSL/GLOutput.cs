using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLOutput : BaseGLArgument
    {
        public override bool IsOutput { get { return true; } }
        public MonitoredList<GLInput> ConnectedTo { get { return _connectedTo; } }

        protected MonitoredList<GLInput> _connectedTo = new MonitoredList<GLInput>(false);

        public GLOutput(string name, params GLTypeName[] types) : base(name)
        {
            _allowedArgTypes = types;
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        public GLOutput(string name, MaterialFunction parent, params GLTypeName[] types) : base(name, parent)
        {
            _allowedArgTypes = types;
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        public GLOutput(string name, BaseGLArgument linkedMultiArg) : base(name)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        public GLOutput(string name, MaterialFunction parent, BaseGLArgument linkedMultiArg) : base(name, parent)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        
        public bool TryConnectTo(GLInput other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        internal virtual void DoConnection(GLInput other) { _connectedTo.AddSilent(other); }
        internal virtual void ClearConnection(GLInput other) { _connectedTo.RemoveSilent(other); }
        private void _connectedTo_Added(GLInput item) { item.DoConnection(this); }
        private void _connectedTo_Removed(GLInput item) { item.ClearConnection(); }
        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToPoint(Vec2 otherPoint, float time)
        {
            Vec2 p0 = Translation;

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
                return Translation;

            return BezierToPoint(_connectedTo[argIndex].Translation, time);
        }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null || other.IsOutput == IsOutput)
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
                    return _allowedArgTypes.Contains(otherType);

                //Has to be a GLMultiArgument as per the edge case check above
                GLInput otherMultiArg = (GLInput)other;

                //Returns true if there are any matching allowed types between the two
                return _allowedArgTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
