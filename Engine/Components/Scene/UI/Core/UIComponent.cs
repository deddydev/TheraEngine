using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.ComponentModel;
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
        public IUIComponent ParentUIComponent { get; internal set; }
        public IUIComponent UIComponent { get; internal set; }
    }
    public enum EVisibility
    {
        Visible,
        Hidden,
        Collapsed,
    }
    public interface IUIComponent : IOriginRebasableComponent, ISocket, IEnumerable<IUIComponent>
    {
        bool IsVisible { get; set; }
        EVisibility Visibility { get; set; }
        bool IsEnabled { get; set; }
        UIParentAttachmentInfo ParentInfo { get; set; }

        void RegisterInputs(InputInterface input);
        void ResizeLayout(BoundingRectangleF parentRegion);
        Vec2 ScreenToLocal(Vec2 coordinate, bool delta = false);
        void InvalidateLayout();

        float CalcAutoWidth();
        float CalcAutoHeight();
    }
    public abstract class UIComponent : OriginRebasableComponent, IUIComponent, I2DRenderable, I3DRenderable
    {
        public UIComponent() : base() { _rc = new RenderCommandMethod2D(ERenderPass.OnTopForward, RenderVisualGuides); }

        [TSerialize(nameof(Visibility))]
        protected EVisibility _visibility = EVisibility.Collapsed;
        [TSerialize(nameof(IsEnabled))]
        protected bool _isEnabled = true;

        IRenderInfo2D I2DRenderable.RenderInfo => RenderInfo2D;
        IRenderInfo3D I3DRenderable.RenderInfo => RenderInfo3D;

        [TSerialize]
        [Category("Rendering")]
        public IRenderInfo2D RenderInfo2D { get; private set; } = new RenderInfo2D(0, 0);
        [TSerialize]
        [Category("Rendering")]
        public IRenderInfo3D RenderInfo3D { get; private set; } = new RenderInfo3D(false, true);

        [Browsable(false)]
        public bool IsVisible
        {
            get => Visibility == EVisibility.Visible;
            set
            {
                if (value)
                    Visibility = EVisibility.Visible;
                else if (CollapseOnHide)
                    Visibility = EVisibility.Collapsed;
                else
                    Visibility = EVisibility.Hidden;
            }
        }

        [Browsable(false)]
        [TSerialize]
        public bool CollapseOnHide { get; set; } = true;

        [Category("Rendering")]
        public virtual EVisibility Visibility
        {
            get => _visibility;
            set => Set(ref _visibility, value, null, VisibilityChanged);
        }

        private void VisibilityChanged()
        {
            RenderInfo2D.IsVisible = IsVisible;
            RenderInfo3D.IsVisible = IsVisible;

            try
            {
                //_childLocker.EnterReadLock();

                foreach (ISceneComponent c in _childSockets)
                    if (c is UIComponent uic)
                        uic.Visibility = Visibility;
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                //_childLocker.ExitReadLock();
            }

            OnPropertyChanged(nameof(IsVisible));
        }

        [Category("Rendering")]
        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set => Set(ref _isEnabled, value, null, IsEnabledChanged);
        }

        private void IsEnabledChanged()
        {
            try
            {
                //_childLocker.EnterReadLock();

                foreach (ISceneComponent c in _childSockets)
                    if (c is UIComponent uic)
                        uic.IsEnabled = IsEnabled;
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                //_childLocker.ExitReadLock();
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
        public IUserInterfacePawn OwningUserInterface => OwningActor as IUserInterfacePawn;

        [Browsable(false)]
        public override IActor OwningActor
        {
            get => base.OwningActor;
            set
            {
                if (IsSpawned)
                    OnDespawned();
                
                base.OwningActor = value as IActor;

                if (IsSpawned)
                    OnSpawned();

                InvalidateLayout();
            }
        }

        public void InvalidateLayout()
            => OwningUserInterface?.InvalidateLayout();

        [Category("Rendering")]
        public bool RenderTransformation 
        {
            get => _renderTransformation;
            set => Set(ref _renderTransformation, value); 
        }

        [Category("Transform")]
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
            //try
            //{
            //    foreach (ISceneComponent comp in ChildComponents)
            //        if (comp is IUIComponent uiComp)
            //            uiComp.RegisterInputs(input);
            //}
            //catch (Exception ex) 
            //{
            //    Engine.LogException(ex);
            //}
        }
        protected override void OnSpawned()
        {
            base.OnSpawned();
            if (this is I2DRenderable r)
                OwningUserInterface?.AddRenderableComponent(r);
        }
        protected override void OnDespawned()
        {
            if (this is I2DRenderable r)
                OwningUserInterface?.RemoveRenderableComponent(r);
            base.OnDespawned();
        }
        protected override bool AllowRecalcLocalTransform()
        {
            if (OwningUserInterface?.IsResizing ?? true)
                return true;

            //InvalidateLayout();
            return false;
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.Identity;
            inverseLocalTransform = Matrix4.Identity;
        }

        protected abstract void OnResizeLayout(BoundingRectangleF parentRegion);
        protected virtual void OnResizeChildComponents(BoundingRectangleF parentRegion)
        {
            try
            {
                //_childLocker.EnterReadLock();

                foreach (ISceneComponent c in _childSockets)
                    if (c is IUIComponent uiComp)
                        uiComp.ResizeLayout(parentRegion);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                //_childLocker.ExitReadLock();
            }
        }
        /// <summary>
        /// Resizes self depending on the parent component.
        /// </summary>
        void IUIComponent.ResizeLayout(BoundingRectangleF parentRegion)
            => ResizeLayout(parentRegion);
        internal void ResizeLayout(BoundingRectangleF parentRegion)
            => OnResizeLayout(parentRegion);
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

        protected override void OnChildAdded(ISocket item)
        {
            base.OnChildAdded(item);

            if (item is I2DRenderable c)
            {
                c.RenderInfo.LayerIndex = RenderInfo2D.LayerIndex;
                c.RenderInfo.IndexWithinLayer = RenderInfo2D.IndexWithinLayer + 1;
            }

            if (item is IUIComponent uic)
                uic.InvalidateLayout();
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin) { }

        public IEnumerator<IUIComponent> GetEnumerator() => ((IEnumerable<IUIComponent>)_childSockets).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<IUIComponent>)_childSockets).GetEnumerator();

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
        public Vec3 ScreenToLocal(Vec3 coordinate, bool delta = false)
        {
            Matrix4 mtx = InverseActorRelativeMatrix;
            if (delta)
                mtx = mtx.ClearTranslation();
            return coordinate * mtx;
        }
        public Vec3 LocalToScreen(Vec3 coordinate, bool delta = false)
        {
            Matrix4 mtx = ActorRelativeMatrix;
            if (delta)
                mtx = mtx.ClearTranslation();
            return coordinate * mtx;
        }
        public Vec2 LocalToScreen(Vec2 coordinate, bool delta = false)
        {
            Matrix4 mtx = ActorRelativeMatrix;
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
            Vec3 up = WorldMatrix.UpVec * 50.0f;
            Vec3 right = WorldMatrix.RightVec * 50.0f;

            Engine.Renderer.RenderLine(endPoint, endPoint + up, Color.Green);
            Engine.Renderer.RenderLine(endPoint, endPoint + right, Color.Red);
        }
        
        public virtual float CalcAutoWidth() => 0.0f;
        public virtual float CalcAutoHeight() => 0.0f;

        public virtual bool Contains(Vec2 worldPoint) => worldPoint.DistanceTo(WorldMatrix.Translation.Xy) < 0.0001f;
        public virtual Vec2 ClosestPoint(Vec2 worldPoint) => WorldMatrix.Translation.Xy;
    }
}
