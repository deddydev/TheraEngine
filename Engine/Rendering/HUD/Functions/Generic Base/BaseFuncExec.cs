using TheraEngine.Rendering.HUD;
using System;

namespace TheraEngine.Rendering
{
    public interface IBaseFuncExec
    {
        Vec2 Translation { get; set; }
        void Arrange(int argumentIndex);
        void ClearConnection();
    }
    public abstract class BaseFuncExec<T> : BaseFuncArg where T : HudComponent, IBaseFuncExec
    {
        public T ConnectedTo => _connectedTo;

        protected T _connectedTo;

        public BaseFuncExec(string name) : base(name) { }
        public BaseFuncExec(string name, IFunction parent) : base(name, parent) { }
        
        public virtual void ClearConnection()
        {
            T temp = _connectedTo;
            _connectedTo = null;
            temp?.ClearConnection();
        }
        public void Arrange(int argumentIndex)
        {
            //TranslationX = IsOutput ? MaterialFunction._padding + _connectionBoxDims + 
        }

        /// <summary>
        /// Returns an interpolated point from this argument to the connected argument.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from one point to the other, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToConnectedArgPoint(float time)
        {
            if (_connectedTo == null)
                return Translation;

            return BezierToPointAsPoint(_connectedTo.Translation, time);
        }
        public Vec2[] BezierToConnectedArgPoints(int count)
        {
            if (_connectedTo == null)
                return new Vec2[0];

            return BezierToPointAsPoints(_connectedTo.Translation, count);
        }
    }
}
