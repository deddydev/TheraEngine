﻿using System;
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
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public abstract class UIParentAttachmentInfo : TObject
    {
        public IUIComponent UIComponent { get; set; }
    }
    public interface IUIComponent : IOriginRebasableComponent, I2DRenderable, IEnumerable<IUIComponent>
    {
        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }
        UIParentAttachmentInfo ParentInfo { get; set; }

        void RegisterInputs(InputInterface input);
        internal void ResizeLayout(BoundingRectangleF parentRegion);
        Vec2 ScreenToLocal(Vec2 coordinate, bool delta = false);
        void InvalidateLayout();
    }
    public abstract class UIComponent : OriginRebasableComponent, IUIComponent
    {
        public UIComponent() : base() { _rc = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderVisualGuides); }

        protected bool _visible = true;
        protected bool _enabled = true;

        [Category("Rendering")]
        public IRenderInfo2D RenderInfo { get; } = new RenderInfo2D(0, 0);

        [Category("Rendering")]
        public virtual bool IsVisible
        {
            get => _visible;
            set
            {
                if (!Set(ref _visible, value))
                    return;

                RenderInfo.Visible = _visible;

                foreach (ISceneComponent c in _children)
                    if (c is UIComponent uic)
                        uic.IsVisible = _visible;
            }
        }
        [Category("Rendering")]
        public virtual bool IsEnabled
        {
            get => _enabled;
            set
            {
                if (Set(ref _enabled, value))
                    foreach (ISceneComponent c in _children)
                        if (c is UIComponent uic)
                            uic.IsEnabled = _enabled;
            }
        }

        [Category("Scene Component")]
        public override ISocket ParentSocket
        {
            get => base.ParentSocket;
            set
            {
                base.ParentSocket = value; 
                InvalidateLayout();
            }
        }

        [Browsable(false)]
        public new IUserInterfacePawn OwningActor
        {
            get => (IUserInterfacePawn)base.OwningActor;
            set
            {
                if (IsSpawned && this is I2DRenderable r)
                {
                    OwningActor?.RemoveRenderableComponent(r);
                    value?.AddRenderableComponent(r);
                }
                base.OwningActor = value as BaseActor;

                //if (ParentSocket is null)
                InvalidateLayout();
            }
        }

        public void InvalidateLayout()
        {
            OwningActor?.InvalidateLayout();
        }

        [Category("Rendering")]
        public bool RenderTransformation 
        {
            get => _renderTransformation;
            set => Set(ref _renderTransformation, value); 
        }

        [Category("Rendering")]
        public UIParentAttachmentInfo ParentInfo
        {
            get => _parentInfo;
            set => Set(ref _parentInfo, value,
                () => _parentInfo.UIComponent = null,
                () => _parentInfo.UIComponent = this,
                false);
        }

        void IUIComponent.RegisterInputs(InputInterface input) => RegisterInputs(input);
        /// <summary>
        /// Recursively registers (or unregisters) inputs on this and all child UI components.
        /// </summary>
        /// <param name="input"></param>
        internal protected virtual void RegisterInputs(InputInterface input)
        {
            foreach (ISceneComponent comp in ChildComponents)
                if (comp is IUIComponent uiComp)
                    uiComp.RegisterInputs(input);
        }
        public override void OnSpawned()
        {
            base.OnSpawned();
            if (this is I2DRenderable r)
                OwningActor?.AddRenderableComponent(r);
        }
        public override void OnDespawned()
        {
            if (this is I2DRenderable r)
                OwningActor?.RemoveRenderableComponent(r);
            base.OnDespawned();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.Identity;
            inverseLocalTransform = Matrix4.Identity;
        }
        protected override void OnWorldTransformChanged()
        {
            InvalidateLayout();
            base.OnWorldTransformChanged();
        }

        protected virtual void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            //if (IgnoreResizes)
            //    return;

            //IgnoreResizes = true;

            RecalcLocalTransform();

            foreach (ISceneComponent c in _children)
                if (c is IUIComponent uiComp)
                    uiComp.ResizeLayout(parentRegion);

            //IgnoreResizes = false;
        }
        /// <summary>
        /// Resizes self depending on the parent component.
        /// </summary>
        void IUIComponent.ResizeLayout(BoundingRectangleF parentRegion)
            => ResizeLayout(parentRegion);
        internal void ResizeLayout(BoundingRectangleF parentRegion)
        {
            OnResizeLayout(parentRegion);
        }
        //public virtual IUIComponent FindDeepestComponent(Vec2 cursorPointWorld, bool includeThis)
        //{
        //    foreach (ISceneComponent c in _children)
        //        if (c is IUIComponent uiComp)
        //        {
        //            IUIComponent comp = uiComp.FindDeepestComponent(cursorPointWorld, true);
        //            if (comp != null)
        //                return comp;
        //        }

        //    if (includeThis && cursorPointWorld.DistanceTo(WorldPoint.Xy) < 20)
        //        return this;

        //    return null;
        //}

        protected override void OnChildAdded(ISceneComponent item)
        {
            base.OnChildAdded(item);
            if (item is IUIComponent c)
            {
                c.RenderInfo.LayerIndex = RenderInfo.LayerIndex;
                c.RenderInfo.IndexWithinLayer = RenderInfo.IndexWithinLayer + 1;
                c.InvalidateLayout();
            }
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin) { }

        public IEnumerator<IUIComponent> GetEnumerator() => ((IEnumerable<IUIComponent>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<IUIComponent>)_children).GetEnumerator();

        /// <summary>
        /// Converts a local-space coordinate of a parent UI component 
        /// to a local-space coordinate of a child UI component.
        /// </summary>
        /// <param name="coordinate">The coordinate relative to the parent UI component.</param>
        /// <param name="parentUIComp">The parent UI component whose space the coordinate is already in.</param>
        /// <param name="targetChildUIComp">The UI component whose space you wish to convert the coordinate to.</param>
        /// <returns></returns>
        public static Vec2 ConvertUICoordinate(Vec2 coordinate, IUIComponent parentUIComp, IUIComponent targetChildUIComp, bool delta = false)
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
            Matrix4 mtx = InverseActorRelativeMatrix;
            if (delta)
                mtx = mtx.ClearTranslation();
            return (coordinate * mtx).Xy;
        }

        public RenderCommandMethod2D _rc;
        private UIParentAttachmentInfo _parentInfo;
        private bool _renderTransformation = true;

        public virtual void AddRenderables(RenderPasses passes, ICamera camera)
        {
            //#if EDITOR
            //if (!Engine.EditorState.InEditMode)
            //    return;
            //#endif

            if (!RenderTransformation)
                return;

            passes.Add(_rc);
        }

        /// <summary>
        /// Helper method for rendering transforms, bounds, rotations, etc in the editor.
        /// </summary>
        protected virtual void RenderVisualGuides()
        {
            Vec3 startPoint = (ParentSocket?.WorldMatrix.Translation ?? Vec3.Zero) + AbstractRenderer.UIPositionBias;
            Vec3 endPoint = WorldMatrix.Translation + AbstractRenderer.UIPositionBias;

            Engine.Renderer.RenderLine(startPoint, endPoint, ColorF4.White);
            Engine.Renderer.RenderPoint(endPoint, ColorF4.White);

            //Vec3 scale = WorldMatrix.Scale;
            Vec3 up = WorldMatrix.UpVec.NormalizedFast() * 50.0f;
            Vec3 right = WorldMatrix.RightVec.NormalizedFast() * 50.0f;

            Engine.Renderer.RenderLine(endPoint, endPoint + up, Color.Green);
            Engine.Renderer.RenderLine(endPoint, endPoint + right, Color.Red);
        }
    }
}
