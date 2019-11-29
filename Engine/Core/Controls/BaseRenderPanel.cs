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
        public RenderPanel()
        {
            _renderHandlerCache = new Lazy<T>(() => MarshalRenderHandler(true), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private bool IsHandleCreated { get; set; } = false;

        protected override void OnHandleCreated(EventArgs e)
        {
            if (IsHandleCreated)
                return;

            IsHandleCreated = true;

            base.OnHandleCreated(e);

            Engine.Instance.DomainProxyCreated += Instance_DomainProxySet;
            Engine.Instance.DomainProxyDestroying += Instance_DomainProxyUnset;
            Instance_DomainProxySet(Engine.DomainProxy);
        }
        protected override void DestroyHandle()
        {
            if (!IsHandleCreated)
                return;

            IsHandleCreated = false;

            Engine.Instance.DomainProxyCreated -= Instance_DomainProxySet;
            Engine.Instance.DomainProxyDestroying -= Instance_DomainProxyUnset;

            base.DestroyHandle();
        }

        /// <summary>
        /// Arguments to pass into the constructor of the render handler.
        /// All arguments must be serializable.
        /// </summary>
        public object[] HandlerArgs { get; set; } = new object[0];
        public override void CreateContext()
        {
            Instance_DomainProxyUnset(Engine.DomainProxy);
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

        /// <summary>
        /// Handles rendering for this panel in the game's AppDomain.
        /// </summary>
        public T RenderHandler => _renderHandlerCache.Value;
        private Lazy<T> _renderHandlerCache;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_renderHandlerCache.IsValueCreated)
                _renderHandlerCache.Value?.Sponsor?.Release();

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Marshals the render handler over from the game's AppDomain.
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public T MarshalRenderHandler(bool cache)
        {
            if (InvokeRequired)
                return (T)Invoke((Func<bool, T>)MarshalRenderHandler, cache);
            
            T handler = (T)Engine.DomainProxy.MarshalRenderHandler(Handle);
            if (cache)
                AppDomainHelper.Sponsor(handler);
            
            return handler;
        }
    }
}
