﻿using System;
using System.Drawing;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public abstract class BaseFuncArg : UIMaterialRectangleComponent
    {
        internal const int ConnectionBoxDims = 10;
        internal const int ConnectionBoxMargin = 3;

        public BaseFuncArg(string name) : base(MakeArgMaterial())
        {
            _name = name;
            DockStyle = HudDockStyle.None;
            Size = new Vec2(ConnectionBoxDims);
        }
        public BaseFuncArg(string name, IFunction parent) : base(MakeArgMaterial())
        {
            _name = name;
            _parent = (UIComponent)parent;
            DockStyle = HudDockStyle.None;
            Size = new Vec2(ConnectionBoxDims);
        }

        public abstract bool IsOutput { get; }
        public override string ToString() => Name;

        private static TMaterial MakeArgMaterial()
        {
            return TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
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
