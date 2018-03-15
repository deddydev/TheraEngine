using System;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public abstract class BaseFuncArg : UIMaterialRectangleComponent
    {
        public static Vec4 RegularColor { get; set; } = new Vec4(0.4f, 0.4f, 0.4f, 1.0f);
        public static Vec4 HighlightedColor { get; set; } = new Vec4(0.4f, 0.6f, 0.6f, 1.0f);
        public static Vec4 ConnectableColor { get; set; } = new Vec4(0.8f, 0.2f, 0.2f, 1.0f);

        public const int ConnectionBoxDims = 10;
        public const int ConnectionBoxMargin = 3;

        public BaseFuncArg(string name, ColorF4 color) : base(MakeArgMaterial(color))
        {
            _name = name;
            Size = new Vec2(ConnectionBoxDims);
        }
        public BaseFuncArg(string name, IFunction parent, ColorF4 color) : base(MakeArgMaterial(color))
        {
            _name = name;
            _parent = parent;
            Size = new Vec2(ConnectionBoxDims);
        }

        public abstract bool IsOutput { get; }

        public override string ToString() => Name;

        public abstract bool CanConnectTo(BaseFuncArg other);
        public abstract bool TryConnectTo(BaseFuncArg other);
        
        private static TMaterial MakeArgMaterial(ColorF4 color)
        {
            return TMaterial.CreateUnlitColorMaterialForward(color);
        }
        
        /// <summary>
        /// Returns an interpolated point from this argument to the given point.
        /// Used for rendering the material editor graph.
        /// </summary>
        /// <param name="t">Time from this argument's point to the given point, 0.0f to 1.0f continuous.</param>
        /// <returns>The interpolated point.</returns>
        public Vec2 BezierToPointAsPoint(Vec2 point, float time)
        {
            Vec2 p0 = ScreenTranslation;

            Vec2 p1 = p0;
            p1.X += IsOutput ? 10.0f : -10.0f;

            Vec2 p3 = point;

            Vec2 p2 = p3;
            p2.X += ScreenTranslation.X < point.X ? -10.0f : 10.0f;

            return Interp.CubicBezier(p0, p1, p2, p3, time);
        }
        public Vec2[] BezierToPointAsPoints(Vec2 point, int count)
        {
            Vec2 p0 = ScreenTranslation;

            Vec2 p1 = p0;
            p1.X += IsOutput ? 10.0f : -10.0f;

            Vec2 p3 = point;

            Vec2 p2 = p3;
            p2.X += ScreenTranslation.X < point.X ? -10.0f : 10.0f;

            return Interp.GetBezierPoints(p0, p1, p2, p3, count);
        }
    }
}
