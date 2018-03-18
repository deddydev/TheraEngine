using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IFuncValueOutput : IBaseFuncValue, IEnumerable<IFuncValueInput>
    {
        void CallbackAddConnection(IFuncValueInput other);
        void CallbackRemoveConnection(IFuncValueInput other);
        void ClearConnections();
        bool ConnectionsContains(IFuncValueInput other);
    }
    public class FuncValueOutput<TInput, TParent> : BaseFuncValue<TInput>, IFuncValueOutput
        where TInput : class, IFuncValueInput where TParent : class, IFunction
    {
        public override bool IsOutput => true;
        public EventList<TInput> Connections => _connections;
        public new TParent ParentSocket => (TParent)base.ParentSocket;
        
        protected EventList<TInput> _connections = new EventList<TInput>(false);

        public FuncValueOutput(string name, params int[] types)
            : base(name)
        {
            AllowedArgumentTypes = types;
            _connections.PostAdded += _connectedTo_Added;
            _connections.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TParent parent, params int[] types)
            : base(name, parent)
        {
            AllowedArgumentTypes = types;
            _connections.PostAdded += _connectedTo_Added;
            _connections.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TInput linkedMultiArg)
            : base(name)
        {
            SyncedArguments.Add(linkedMultiArg);
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            _connections.PostAdded += _connectedTo_Added;
            _connections.PostRemoved += _connectedTo_Removed;
        }
        public FuncValueOutput(string name, TParent parent, TInput linkedMultiArg)
            : base(name, parent)
        {
            SyncedArguments.Add(linkedMultiArg);
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            _connections.PostAdded += _connectedTo_Added;
            _connections.PostRemoved += _connectedTo_Removed;
        }
        
        public bool ConnectTo(TInput other)
        {
            if (!CanConnectTo(other))
                return false;
            _connections.Add(other);
            return true;
        }

        public void CallbackAddConnection(IFuncValueInput other) => CallbackAddConnection(other as TInput);
        public virtual void CallbackAddConnection(TInput other) => _connections.Add(other, false, false);
        public void CallbackRemoveConnection(IFuncValueInput other) => CallbackRemoveConnection(other as TInput);
        public virtual void CallbackRemoveConnection(TInput other) => _connections.Remove(other, false, false);
        public void ClearConnections() => _connections.Clear();
        
        private void _connectedTo_Added(TInput item) => item.Connection = this;
        private void _connectedTo_Removed(TInput item) => item.ClearConnection();

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
            if (_connections == null ||
                argIndex >= _connections.Count ||
                argIndex < 0 ||
                _connections[argIndex] == null)
                return ScreenTranslation;

            return BezierToPoint((_connections[argIndex]).ScreenTranslation, time);
        }
        public override bool CanConnectTo(TInput other)
        {
            if (other == null || Connections.Contains(other))
                return false;

            int otherType = other.CurrentArgumentType;
            int thisType = CurrentArgumentType;

            if (thisType >= 0)
            {
                if (otherType >= 0)
                    return thisType == otherType;
                
                return other.AllowedArgumentTypes.Contains(thisType);
            }
            else //this type is invalid, use allowed arg types
            {
                if (otherType >= 0)
                    return AllowedArgumentTypes.Contains(otherType);
                
                //Returns true if there are any matching allowed types between the two
                return AllowedArgumentTypes.Intersect(other.AllowedArgumentTypes).ToArray().Length != 0;
            }
        }
        public override bool CanConnectTo(BaseFuncArg other)
            => CanConnectTo(other as TInput);
        public override bool TryConnectTo(BaseFuncArg other)
            => ConnectTo(other as TInput);

        IEnumerator<IFuncValueInput> IEnumerable<IFuncValueInput>.GetEnumerator() => _connections.GetEnumerator();

        public bool ConnectionsContains(IFuncValueInput other) => _connections.Contains(other);
    }
}
