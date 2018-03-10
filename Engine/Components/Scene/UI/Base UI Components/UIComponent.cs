using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public interface IUIComponent : ISceneComponent
    {
        Vec2 ScreenTranslation { get; }
        Vec2 LocalTranslation { get; set; }
        float LocalTranslationX { get; set; }
        float LocalTranslationY { get; set; }
        Vec2 Scale { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
    }
    public class UIComponent : OriginRebasableComponent, IUIComponent, IEnumerable<UIComponent>
    {
        public UIComponent() : base() { }

        protected Vec2 _translation = Vec2.Zero;
        protected Vec2 _scale = Vec2.One;
        protected Vec2 _localOriginPercentage = Vec2.Zero;

        public virtual int LayerIndex { get; set; }
        public virtual int IndexWithinLayer { get; set; }

        [Browsable(false)]
        public override ISocket ParentSocket
        {
            get => base.ParentSocket;
            set
            {
                base.ParentSocket = value;
                PerformResize();
            }
        }
        
        #region Translation

        [Browsable(false)]
        [Category("Transform")]
        public Vec2 ScreenTranslation => Vec3.TransformPosition(WorldPoint, GetInvActorTransform()).Xy;

        [Category("Transform")]
        public virtual Vec2 LocalTranslation
        {
            get => _translation;
            set
            {
                _translation = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationX
        {
            get => _translation.X;
            set
            {
                _translation.X = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        [Category("Transform")]
        public virtual float LocalTranslationY
        {
            get => _translation.Y;
            set
            {
                _translation.Y = value;
                RecalcLocalTransform();
                //PerformResize();
            }
        }
        #endregion

        #region Scale
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
        #endregion
        
        [Browsable(false)]
        public new IUIManager OwningActor
        {
            get => (IUIManager)base.OwningActor;
            set
            {
                if (IsSpawned && this is I2DRenderable r)
                {
                    OwningActor?.RemoveRenderableComponent(r);
                    value?.AddRenderableComponent(r);
                }
                base.OwningActor = value;

                //if (ParentSocket == null)
                    PerformResize();
            }
        }

        public override void OnSpawned()
        {
            if (this is I2DRenderable r)
                OwningActor.AddRenderableComponent(r);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (this is I2DRenderable r)
                OwningActor.RemoveRenderableComponent(r);
            base.OnDespawned();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(new Vec3(Scale, 1.0f), Matrix4.Identity, LocalTranslation, TransformOrder.TRS);
            inverseLocalTransform = Matrix4.TransformMatrix(new Vec3(1.0f / Scale, 1.0f), Matrix4.Identity, -LocalTranslation, TransformOrder.SRT);
        }
        public virtual Vec2 Resize(Vec2 parentBounds)
        {
            foreach (UIComponent c in _children)
                c.Resize(parentBounds);
            RecalcLocalTransform();
            return parentBounds;
        }
        protected virtual void PerformResize()
        {
            if (ParentSocket is UIBoundableComponent comp)
                Resize(comp.Size);
            else if (OwningActor != null)
                Resize(OwningActor.Bounds);
            else
                Resize(Vec2.Zero);
        }
        public virtual UIComponent FindDeepestComponent(Vec2 viewportPoint)
        {
            foreach (UIComponent c in _children)
            {
                UIComponent comp = c.FindDeepestComponent(viewportPoint);
                if (comp != null)
                    return comp;
            }
            return null;
        }

        public void Zoom(float amount, Vec2 worldScreenPoint, Vec2? minScale, Vec2? maxScale)
        {
            if (amount == 0.0f)
                return;

            Vec2 multiplier = Vec2.One / _scale * amount;
            Vec2 newScale = _scale - amount;

            bool xClamped = false;
            bool yClamped = false;
            if (minScale != null)
            {
                if (newScale.X < minScale.Value.X)
                {
                    newScale.X = minScale.Value.X;
                    xClamped = true;
                }
                if (newScale.Y < minScale.Value.Y)
                {
                    newScale.Y = minScale.Value.Y;
                    yClamped = true;
                }
            }
            if (maxScale != null)
            {
                if (newScale.X > maxScale.Value.X)
                {
                    newScale.X = maxScale.Value.X;
                    xClamped = true;
                }
                if (newScale.Y > maxScale.Value.Y)
                {
                    newScale.Y = maxScale.Value.Y;
                    yClamped = true;
                }
            }

            if (!xClamped || !yClamped)
            {
                _translation = (_translation + (worldScreenPoint - WorldPoint.Xy) * multiplier);
                _scale = newScale;
            }

            PerformResize();

            //_cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * Matrix4.CreateScale(scale);
            //_worldToCameraSpaceMatrix = Matrix4.CreateScale(-scale) * _worldToCameraSpaceMatrix;
            //UpdateTransformedFrustum();
            //OnTransformChanged();
        }

        protected override void HandleSingleChildAdded(SceneComponent item)
        {
            base.HandleSingleChildAdded(item);
            if (item is UIComponent c)
                c.LayerIndex = LayerIndex;
        }

        protected internal override void OriginRebased(Vec3 newOrigin) { }

        public IEnumerator<UIComponent> GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
    }
}
