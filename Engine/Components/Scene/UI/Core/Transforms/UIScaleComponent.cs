using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public interface IUIScaleComponent : IUIComponent
    {
        Vec2 Scale { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
    }
    public class UIScaleComponent : UIComponent, IUIScaleComponent
    {
        public UIScaleComponent() : base() { }
        
        protected Vec2 _scale = Vec2.One;

        [Category("Transform")]
        public virtual Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Browsable(false)]
        [Category("Transform")]
        public virtual float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Browsable(false)]
        [Category("Transform")]
        public virtual float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateScale(new Vec3(Scale, 1.0f));
            inverseLocalTransform = Matrix4.CreateScale(new Vec3(1.0f / Scale, 1.0f));
        }
    }
}
