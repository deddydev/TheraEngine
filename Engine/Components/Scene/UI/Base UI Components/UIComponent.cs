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
    public class UIComponent : OriginRebasableComponent, IUIComponent, I2DRenderable, IEnumerable<UIComponent>
    {
        public UIComponent() : base() { _rc = new RenderCommandMethod2D(ERenderPass.OnTopForward, Render); }

        protected Vec2 _translation = Vec2.Zero;
        protected Vec2 _scale = Vec2.One;
        protected bool _visible = true;

        [Category("Rendering")]
        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);
        [Category("Rendering")]
        public virtual bool IsVisible
        {
            get => _visible;
            set
            {
                //if (_visible == value)
                //    return;
                _visible = value;
                RenderInfo.Visible = value;
                foreach (UIComponent c in _children)
                    c.IsVisible = value;
            }
        }

        [Category("Scene Component")]
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
                PerformResize();
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
                PerformResize();
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
                PerformResize();
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
        #endregion
        
        [Browsable(false)]
        public new IUserInterface OwningActor
        {
            get => (IUserInterface)base.OwningActor;
            set
            {
                if (IsSpawned && this is I2DRenderable r)
                {
                    OwningActor?.RemoveRenderableComponent(r);
                    value?.AddRenderableComponent(r);
                }
                base.OwningActor = value as BaseActor;

                //if (ParentSocket == null)
                    PerformResize();
            }
        }
        /// <summary>
        /// Recursively registers (or unregisters) inputs on this and all child UI components.
        /// </summary>
        /// <param name="input"></param>
        internal protected virtual void RegisterInputs(InputInterface input)
        {
            foreach (SceneComponent comp in ChildComponents)
                if (comp is UIComponent uiComp)
                    uiComp.RegisterInputs(input);
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            if (this is I2DRenderable r)
                OwningActor.AddRenderableComponent(r);
        }
        public override void OnDespawned()
        {
            if (this is I2DRenderable r)
                OwningActor.RemoveRenderableComponent(r);
            base.OnDespawned();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.TransformMatrix(
                new Vec3(Scale, 1.0f),
                Matrix4.Identity,
                LocalTranslation,
                TransformOrder.TRS);

            inverseLocalTransform = Matrix4.TransformMatrix(
                new Vec3(1.0f / Scale, 1.0f),
                Matrix4.Identity,
                -LocalTranslation,
                TransformOrder.SRT);
        }
        [Browsable(false)]
        public bool IgnoreResizes { get; set; } = false;
        [Browsable(false)]
        public Vec2 ParentBounds { get; protected set; }
        [TSerialize]
        public bool RenderTransformation { get; set; } = true;

        public virtual Vec2 Resize(Vec2 parentBounds)
        {
            if (IgnoreResizes)
                return parentBounds;

            IgnoreResizes = true;
            ParentBounds = parentBounds;
            foreach (SceneComponent c in _children)
                if (c is UIComponent uiComp)
                    uiComp.Resize(parentBounds);
            RecalcLocalTransform();
            IgnoreResizes = false;

            return parentBounds;
        }
        /// <summary>
        /// Resizes self depending on the parent component.
        /// </summary>
        public virtual void PerformResize()
        {
            if (IgnoreResizes)
                return;

            if (ParentSocket is UIBoundableComponent comp)
                Resize(comp.Size);
            else if (OwningActor != null)
                Resize(OwningActor.Bounds);
            //else
            //    Resize(Vec2.Zero);
        }
        public virtual UIComponent FindDeepestComponent(Vec2 cursorPointWorld, bool includeThis)
        {
            foreach (SceneComponent c in _children)
            {
                if (c is UIComponent uiComp)
                {
                    UIComponent comp = uiComp.FindDeepestComponent(cursorPointWorld, true);
                    if (comp != null)
                        return comp;
                }
            }

            if (includeThis && cursorPointWorld.DistanceTo(WorldPoint.Xy) < 20)
                return this;

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
            {
                c.RenderInfo.LayerIndex = RenderInfo.LayerIndex;
                c.RenderInfo.IndexWithinLayer = RenderInfo.IndexWithinLayer + 1;
                c.PerformResize();
            }
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin) { }

        public IEnumerator<UIComponent> GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UIComponent>)_children).GetEnumerator();

        /// <summary>
        /// Converts a local-space coordinate of a parent UI component 
        /// to a local-space coordinate of a child UI component.
        /// </summary>
        /// <param name="coordinate">The coordinate relative to the parent UI component.</param>
        /// <param name="parentUIComp">The parent UI component whose space the coordinate is already in.</param>
        /// <param name="targetChildUIComp">The UI component whose space you wish to convert the coordinate to.</param>
        /// <returns></returns>
        public static Vec2 ConvertUICoordinate(Vec2 coordinate, UIComponent parentUIComp, UIComponent targetChildUIComp, bool delta = false)
        {
            Matrix4 mtx = targetChildUIComp.InverseWorldMatrix * parentUIComp.WorldMatrix;
            if (delta)
                mtx = mtx.ClearTranslation();
            return (coordinate * mtx).Xy;
        }
        /// <summary>
        /// Converts a screen-space coordinate
        /// to a local-space coordinate of a UI component.
        /// </summary>
        /// <param name="coordinate">The coordinate relative to the screen / origin of the root UI component.</param>
        /// <param name="uiComp">The UI component whose space you wish to convert the coordinate to.</param>
        /// <param name="delta">If true, the coordinate and returned value are treated like a vector offset instead of an absolute point.</param>
        /// <returns></returns>
        public Vec2 ScreenToLocal(Vec2 coordinate, bool delta = false)
        {
            Matrix4 mtx = GetInvComponentTransform();
            if (delta)
                mtx = mtx.ClearTranslation();
            return (coordinate * mtx).Xy;
        }
        public RenderCommandMethod2D _rc;
        public virtual void AddRenderables(RenderPasses passes, Camera camera)
        {
            if (!RenderTransformation || !Engine.EditorState.InEditMode)
                return;

            passes.Add(_rc);
        }
        private void Render()
        {
            Vec3 startPoint = ParentSocket?.WorldMatrix.Translation ?? Vec3.Zero;
            Vec3 endPoint = WorldMatrix.Translation;

            Engine.Renderer.RenderLine(startPoint, endPoint, ColorF4.White);
            Engine.Renderer.RenderPoint(endPoint, ColorF4.White);

            Vec3 scale = WorldMatrix.Scale;
            Engine.Renderer.RenderLine(endPoint, endPoint + WorldMatrix.UpVec.NormalizedFast() * 50.0f, Color.Green);
            Engine.Renderer.RenderLine(endPoint, endPoint + WorldMatrix.RightVec.NormalizedFast() * 50.0f, Color.Red);
        }
    }
}
