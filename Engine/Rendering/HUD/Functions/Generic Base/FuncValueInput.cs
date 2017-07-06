using System;
using System.Linq;
using TheraEngine.Rendering.HUD;

namespace TheraEngine.Rendering
{
    public interface IFuncValueInput : IBaseFuncValue
    {
        void SetConnection(IFuncValueOutput other);
        void ClearConnection();
    }
    public class FuncValueInput<TOutput, TParent> : BaseFuncValue<TOutput>, IFuncValueInput
        where TOutput : HudComponent, IFuncValueOutput where TParent : HudComponent, IFunction
    {
        public override bool IsOutput => false;
        public TOutput ConnectedTo
        {
            get => _connectedTo;
            set => TryConnectTo(value);
        }
        protected TOutput _connectedTo;

        public FuncValueInput(string name, params int[] types)
            : base(name)
        {
            _allowedArgTypes = types;
        }
        public FuncValueInput(string name, TParent parent, params int[] types)
            : base(name, parent)
        {
            _allowedArgTypes = types;
        }
        public FuncValueInput(string name, IBaseFuncValue linkedMultiArg)
            : base(name)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }
        public FuncValueInput(string name, TParent parent, IBaseFuncValue linkedMultiArg)
            : base(name, parent)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg.AllowedArgumentTypes;
        }

        public bool TryConnectTo(TOutput other)
        {
            if (!CanConnectTo(other))
                return false;
            SetConnection(other);
            return true;
        }
        public virtual void SetConnection(IFuncValueOutput other)
            => SetConnection(other as TOutput);
        public virtual void SetConnection(TOutput other)
        {
            _connectedTo?.RemoveConnection(this);
            _connectedTo = other;
            _connectedTo?.AddConnection(this);
        }
        public virtual void ClearConnection()
        {
            _connectedTo?.RemoveConnection(this);
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

            return CustomMath.CubicBezier(p0, p1, p2, p3, time);
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
        public override bool CanConnectTo(TOutput other)
        {
            if (other == null)
                return true;

            //if (other.IsOutput == IsOutput)
            //    return false;

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
                TOutput otherMultiArg = other;

                return otherMultiArg.AllowedArgumentTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType >= 0)
                    return AllowedArgumentTypes.Contains(otherType);

                //Has to be a multi arg as per the edge case check above
                TOutput otherMultiArg = other;

                //Returns true if there are any matching allowed types between the two
                return AllowedArgumentTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
