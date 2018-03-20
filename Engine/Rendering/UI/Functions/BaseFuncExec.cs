using System;
using TheraEngine.Components;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncExec : IUIComponent
    {
        void ClearConnection();
    }
    public abstract class BaseFuncExec : BaseFuncArg
    {
        public static ColorF4 DefaultColor { get; } = new ColorF4(0.7f, 1.0f);

        public BaseFuncExec(string name) : base(name, DefaultColor) { }
        public BaseFuncExec(string name, IFunction parent) : base(name, parent, DefaultColor) { }
    }
    public abstract class BaseFuncExec<TInput> : BaseFuncExec where TInput : UIComponent, IBaseFuncExec
    {
        public TInput ConnectedTo => _connectedTo;

        protected TInput _connectedTo;

        public BaseFuncExec(string name) : base(name) { }
        public BaseFuncExec(string name, IFunction parent) : base(name, parent) { }
        
        public virtual void ClearConnection()
        {
            TInput temp = _connectedTo;
            _connectedTo = null;
            temp?.ClearConnection();
        }

        /// <summary>
        /// Returns an interpolated point from this argument to the connected argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        //public Vec2 BezierToConnectedArgPoint(float time)
        //{
        //    if (_connectedTo == null)
        //        return ScreenTranslation;

        //    return BezierToPointAsPoint(_connectedTo.ScreenTranslation, time);
        //}
        //public Vec2[] BezierToConnectedArgPoints(int count)
        //{
        //    if (_connectedTo == null)
        //        return new Vec2[0];

        //    return BezierToPointAsPoints(_connectedTo.ScreenTranslation, count);
        //}
    }
}
