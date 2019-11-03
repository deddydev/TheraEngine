using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Reflection;
using TheraEngine.Rendering;

namespace TheraEngine
{
    public enum EVSyncMode
    {
        Disabled,
        Enabled,
        Adaptive,
    }
    public interface IRenderPanel
    {
        void CreateContext();
    }
    public abstract class BaseRenderPanel : UserControl, IRenderPanel
    {
        public abstract void CreateContext();

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle = cp.ExStyle | 0x2000000;
        //        return cp;
        //    }
        //}
        
        public BaseRenderPanel()
        {
            ResizeRedraw = true;

            //Force custom paint
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque,
                true);

            UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e) { }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Engine.DomainProxy.RenderPanelResized(Handle, Width, Height);
            Engine.DomainProxy.UpdateScreenLocation(Handle, PointToScreen(Point.Empty));
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            Engine.DomainProxy.RenderPanelResized(Handle, Width, Height);
            Engine.DomainProxy.UpdateScreenLocation(Handle, PointToScreen(Point.Empty));
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Engine.DomainProxy.RenderPanelResized(Handle, Width, Height);
            Engine.DomainProxy.UpdateScreenLocation(Handle, PointToScreen(Point.Empty));
        }
        private void UpdateScreenLocation(object sender, EventArgs e)
            => Engine.DomainProxy.UpdateScreenLocation(Handle, PointToScreen(Point.Empty));
        protected override void OnParentChanged(EventArgs e)
        {
            Control parent = this;
            while (parent != null)
            {
                parent.Move -= UpdateScreenLocation;
                parent = parent.Parent;
            }
            Form form = ParentForm;
            if (form != null)
                form.Move -= UpdateScreenLocation;

            base.OnParentChanged(e);

            parent = this;
            while (parent != null)
            {
                parent.Move += UpdateScreenLocation;
                parent = parent.Parent;
            }
            form = ParentForm;
            if (form != null)
                form.Move += UpdateScreenLocation;
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            UpdateScreenLocation(this, e);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            //Create context RIGHT AWAY so render objects can bind to it as they are created
            CreateContext();
        }
        protected override void DestroyHandle()
        {
            Engine.DomainProxy.UnregisterRenderPanel(Handle);
            base.DestroyHandle();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Engine.DomainProxy.MouseEnter(Handle);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Engine.DomainProxy.MouseLeave(Handle);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Engine.DomainProxy.GotFocus(Handle);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Engine.DomainProxy.LostFocus(Handle);
        }
        public void LinkToWorldManager(int worldManagerId)
            => Engine.DomainProxy.LinkRenderPanelToWorldManager(Handle, worldManagerId);
        public void UnlinkFromWorldManager()
            => Engine.DomainProxy.UnlinkRenderPanelFromWorldManager(Handle);
    }
    public class RenderPanel<T> : BaseRenderPanel where T : class, IRenderHandler
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Engine.Instance.DomainProxySet += Instance_DomainProxySet;
            Engine.Instance.DomainProxyUnset += Instance_DomainProxyUnset;
            Instance_DomainProxySet(Engine.DomainProxy);
        }
        protected override void DestroyHandle()
        {
            Engine.Instance.DomainProxySet -= Instance_DomainProxySet;
            Engine.Instance.DomainProxyUnset -= Instance_DomainProxyUnset;

            base.DestroyHandle();
        }

        /// <summary>
        /// Arguments to pass into the constructor of the render handler.
        /// All arguments must be serializable.
        /// </summary>
        public object[] HandlerArgs { get; set; } = new object[0];
        public override void CreateContext()
        {
            Instance_DomainProxySet(Engine.DomainProxy);
        }

        private void Instance_DomainProxyUnset(Core.EngineDomainProxy proxy)
        {
            proxy?.UnregisterRenderPanel(Handle);
        }
        private void Instance_DomainProxySet(Core.EngineDomainProxy proxy)
        {
            proxy?.RegisterRenderPanel<T>(Handle, HandlerArgs);
        }

        private T _renderHandlerCache = null;
        public T RenderHandler  => _renderHandlerCache ?? MarshalRenderHandler(true);
        
        protected override void OnHandleDestroyed(EventArgs e)
        {
            _renderHandlerCache?.Sponsor?.Release();
            base.OnHandleDestroyed(e);
        }
        public T MarshalRenderHandler(bool cache)
        {
            T handler = (T)Engine.DomainProxy.MarshalRenderHandler(Handle);
            if (cache)
            {
                AppDomainHelper.Sponsor(handler);
                _renderHandlerCache = handler;
            }
            return handler;
        }
    }
}
