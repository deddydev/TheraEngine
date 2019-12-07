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
    public interface IUITranslationComponent : IUIComponent
    {
        Vec2 ScreenTranslation { get; }

        Vec2 LocalTranslation { get; set; }
        float LocalTranslationX { get; set; }
        float LocalTranslationY { get; set; }
    }
    public class UITranslationComponent : UIComponent, IUITranslationComponent
    {
        public UITranslationComponent() : base() { }
        
        protected Vec2 _translation = Vec2.Zero;

        [Browsable(false)]
        [Category("Transform")]
        public Vec2 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, GetComponentTransform()).Xy;
            set => LocalTranslation = Vec3.TransformPosition(value, GetInvComponentTransform()).Xy;
        }

        [Category("Transform")]
        public virtual Vec2 LocalTranslation
        {
            get => _translation;
            set
            {
                _translation = value;
                //RecalcLocalTransform();
                Resize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationX
        {
            get => _translation.X;
            set
            {
                _translation.X = value;
                //RecalcLocalTransform();
                Resize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationY
        {
            get => _translation.Y;
            set
            {
                _translation.Y = value;
                //RecalcLocalTransform();
                Resize();
            }
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(LocalTranslation);
            inverseLocalTransform = Matrix4.CreateTranslation(-LocalTranslation);
        }
    }
}
