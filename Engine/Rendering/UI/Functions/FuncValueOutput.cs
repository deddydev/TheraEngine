using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IFuncValueOutput : IBaseFuncValue
    {
        void AddConnection(IFuncValueInput other);
        void RemoveConnection(IFuncValueInput other);
    }
    public class FuncValueOutput<TInput, TParent> : BaseFuncValue<TInput>, IFuncValueOutput
        where TInput : class, IFuncValueInput where TParent : class, IFunction
    {
        public override bool IsOutput => true;
        public EventList<TInput> ConnectedTo => _connectedTo;

        protected EventList<TInput> _connectedTo = new EventList<TInput>(false);

        public FuncValueOutput(string name, params int[] types)
            : base(name)
        {
            AllowedArgumentTypes = types;
            _connectedTo.PostAdded += _connectedTo_Added;
            _connectedTo.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TParent parent, params int[] types)
            : base(name, parent)
        {
            AllowedArgumentTypes = types;
            _connectedTo.PostAdded += _connectedTo_Added;
            _connectedTo.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TInput linkedMultiArg)
            : base(name)
        {
            SyncedArguments.Add(linkedMultiArg);
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            _connectedTo.PostAdded += _connectedTo_Added;
            _connectedTo.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TParent parent, TInput linkedMultiArg)
            : base(name, parent)
        {
            SyncedArguments.Add(linkedMultiArg);
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            _connectedTo.PostAdded += _connectedTo_Added;
            _connectedTo.PostRemoved += _connectedTo_Removed;
        }
        
        public bool TryConnectTo(TInput other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        public virtual void AddConnection(IFuncValueInput other) => DoConnection(other as TInput);
        public virtual void DoConnection(TInput other) { _connectedTo.Add(other, false, false); }
        public virtual void RemoveConnection(IFuncValueInput other) => ClearConnection(other as TInput);
        public virtual void ClearConnection(TInput other) { _connectedTo.Remove(other, false, false); }
        private void _connectedTo_Added(TInput item) { item.SetConnection(this); }
        private void _connectedTo_Removed(TInput item) { item.ClearConnection(); }

        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToPoint(Vec2 otherPoint, float time)
        {
            Vec2 p0 = ScreenTranslation;

            Vec2 p1 = p0;
            p1.X += 10.0f;

            Vec2 p3 = otherPoint;

            Vec2 p2 = p3;
            p2.X -= 10.0f;

            return Interp.CubicBezier(p0, p1, p2, p3, time);
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
                return ScreenTranslation;

            return BezierToPoint((_connectedTo[argIndex]).ScreenTranslation, time);
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
                    return AllowedArgumentTypes.Contains(otherType);

                //Has to be a multi arg as per the edge case check above
                TInput otherMultiArg = other;

                //Returns true if there are any matching allowed types between the two
                return AllowedArgumentTypes.Intersect(otherMultiArg.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
    }
}
