using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IFuncExecOutput : IBaseFuncValue
    {
        Vec2 Translation { get; set; }
        
        void SetConnection(IFuncExecInput other);
        void ClearConnection();
    }
    public class FuncExecOutput<TInput, TParent> : BaseFuncArg<TInput>, IFuncExecOutput
        where TInput : class, IFuncExecInput where TParent : class, IFunction
    {
        public override bool IsOutput => true;
        public TInput ConnectedTo => _connectedTo;

        protected TInput _connectedTo;

        public FuncExecOutput(string name)
            : base(name) { }
        public FuncExecOutput(string name, TParent parent)
            : base(name, parent) { }
        
        public bool TryConnectTo(TInput other)
        {
            if (!CanConnectTo(other))
                return false;
            SetConnection(other);
            return true;
        }
        public virtual void SetConnection(IFuncExecInput other)
            => SetConnection(other as TInput);
        public virtual void SetConnection(TInput other)
        {
            _connectedTo = other;
        }
        public virtual void ClearConnection()
        {
            _connectedTo = null;
        }
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
        public Vec2 BezierToInputArg(float time)
        {
            if (_connectedTo == null)
                return Translation;

            return BezierToPoint(_connectedTo.Translation, time);
        }
        public override bool CanConnectTo(TInput other)
        {
            if (other == null /*|| other.IsOutput == IsOutput*/)
                return false;

            int otherType = other.CurrentArgumentType;

            //Edge case: the other node is just invalid
            if (otherType < 0)
                return false;

            int thisType = CurrentArgumentType;
            if (thisType >= 0)
            {
                if (otherType >= 0)
                    return thisType == otherType;

                //Has to be a multi arg as per the edge case check above
                TInput otherMultiArg = other;

                return otherMultiArg.AllowedArgumentTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType >= 0)
                    return _allowedArgTypes.Contains(otherType);

                //Has to be a multi arg as per the edge case check above
                TInput otherMultiArg = other;

                //Returns true if there are any matching allowed types between the two
                return _allowedArgTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
