using System;

namespace TheraEngine.Rendering.UI.Functions
{
    public abstract class BaseFuncArg : UIComponent
    {
        public BaseFuncArg(string name)
        {
            _name = name;
        }
        public BaseFuncArg(string name, IFunction parent)
        {
            _name = name;
            _parent = (UIComponent)parent;
        }
        public abstract bool IsOutput { get; }
        public override string ToString() => Name;

        /// <summary>
        /// Returns an interpolated point from this argument to the given point.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from this argument's point to the given point, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToPointAsPoint(Vec2 point, float time)
        {
            Vec2 p0 = Translation;

            Vec2 p1 = p0;
            p1.X += IsOutput ? 10.0f : -10.0f;

            Vec2 p3 = point;

            Vec2 p2 = p3;
            p2.X += Translation.X < point.X ? -10.0f : 10.0f;

            return Interp.CubicBezier(p0, p1, p2, p3, time);
        }
        public Vec2[] BezierToPointAsPoints(Vec2 point, int count)
        {
            Vec2 p0 = Translation;

            Vec2 p1 = p0;
            p1.X += IsOutput ? 10.0f : -10.0f;

            Vec2 p3 = point;

            Vec2 p2 = p3;
            p2.X += Translation.X < point.X ? -10.0f : 10.0f;

            return Interp.GetBezierPoints(p0, p1, p2, p3, count);
        }
    }
}
