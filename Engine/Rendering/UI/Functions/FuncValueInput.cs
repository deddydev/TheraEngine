using System;
using System.Linq;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IFuncValueInput : IBaseFuncValue
    {
        IFuncValueOutput Connection { get; set; }
        void ClearConnection();
    }
    public class FuncValueInput<TOutput, TParent> : BaseFuncValue<TOutput>, IFuncValueInput
        where TOutput : UIComponent, IFuncValueOutput where TParent : UIComponent, IFunction
    {
        public delegate void DelConnected(TOutput other);
        public event DelConnected Connected, Disconnected;

        public override bool IsOutput => false;
        public new TParent OwningActor  => (TParent)base.OwningActor;
        public TOutput Connection
        {
            get => _connection;
            set => ConnectTo(value);
        }
        IFuncValueOutput IFuncValueInput.Connection
        {
            get => Connection;
            set => ConnectTo(value as TOutput);
        }
        public override bool HasConnection => Connection != null;

        protected TOutput _connection;

        public FuncValueInput(string name, params int[] types)
            : base(name)
        {
            AllowedArgumentTypes = types;
        }
        public FuncValueInput(string name, TParent parent, params int[] types)
            : base(name, parent)
        {
            AllowedArgumentTypes = types;
        }
        public FuncValueInput(string name, IBaseFuncValue linkedMultiArg)
            : base(name)
        {
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            SyncedArguments.UnionWith(linkedMultiArg.SyncedArguments);
            SyncedArguments.Add(linkedMultiArg);
            linkedMultiArg.SyncedArguments.Add(this);
        }
        public FuncValueInput(string name, TParent parent, IBaseFuncValue linkedMultiArg)
            : base(name, parent)
        {
            AllowedArgumentTypes = linkedMultiArg.AllowedArgumentTypes;
            SyncedArguments.UnionWith(linkedMultiArg.SyncedArguments);
            SyncedArguments.Add(linkedMultiArg);
            linkedMultiArg.SyncedArguments.Add(this);
        }

        public bool ConnectTo(TOutput other)
        {
            if (!CanConnectTo(other))
                return false;
            SetConnection(other);
            return true;
        }
        protected virtual void SetConnection(TOutput other)
        {
            if (_connection != null)
            {
                _connection.SecondaryRemoveConnection(this);
                Disconnected?.Invoke(_connection);
            }
            _connection = other;
            if (_connection != null)
            {
                _connection.CallbackAddConnection(this);
                DetermineBestArgType(_connection);
                Connected?.Invoke(_connection);
            }
            else
                DetermineBestArgType(null);
        }

        public virtual void ClearConnection()
        {
            if (_connection != null)
            {
                _connection.SecondaryRemoveConnection(this);
                DetermineBestArgType(null);
                Disconnected?.Invoke(_connection);
            }
            _connection = null;
        }

        protected override void OnCurrentArgTypeChanged()
        {
            _connection.CurrentArgumentType = CurrentArgumentType;
        }

        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        //public Vec2 BezierFromPoint(Vec2 otherPoint, float time)
        //{
        //    Vec2 p0 = otherPoint;

        //    Vec2 p1 = p0;
        //    p1.X += 10.0f;

        //    Vec2 p3 = ScreenTranslation;

        //    Vec2 p2 = p3;
        //    p2.X -= 10.0f;

        //    return Interp.CubicBezier(p0, p1, p2, p3, time);
        //}
        /// <summary>
        /// Returns interpolated point from the connected output argument to this argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        //public Vec2 BezierFromOutputArg(float time)
        //{
        //    if (_connectedTo == null)
        //        return ScreenTranslation;

        //    return BezierFromPoint(_connectedTo.ScreenTranslation, time);
        //}
        //public Vec2[] BezierPointsFromPoint(Vec2 otherPoint, int count)
        //{
        //    Vec2 p0 = otherPoint;

        //    Vec2 p1 = p0;
        //    p1.X += 10.0f;

        //    Vec2 p3 = ScreenTranslation;

        //    Vec2 p2 = p3;
        //    p2.X -= 10.0f;

        //    return Interp.GetBezierPoints(p0, p1, p2, p3, count);
        //}
        //public Vec2[] BezierPointsFromOutputArg(Vec2 otherPoint, int count)
        //{
        //    if (_connectedTo == null)
        //        return null;

        //    return BezierPointsFromPoint(_connectedTo.ScreenTranslation, count);
        //}
        public override bool CanConnectTo(TOutput other)
        {
            if (other == null || Connection == other)
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
            => CanConnectTo(other as TOutput);
        public override bool ConnectTo(BaseFuncArg other)
            => ConnectTo(other as TOutput);
    }
}
