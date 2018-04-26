using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.DirectX;
using TheraEngine.Rendering.OpenGL;
using TheraEngine.Timers;

namespace TheraEngine
{
    public enum VSyncMode
    {
        Disabled,
        Enabled,
        Adaptive,
    }
    public static class ControlExtension
    {
        [ReflectionPermission(SecurityAction.Demand, MemberAccess = true)]
        public static void Reset(this Control c)
        {
            typeof(Control).InvokeMember("SetState", BindingFlags.NonPublic |
            BindingFlags.InvokeMethod | BindingFlags.Instance, null,
            c, new object[] { 0x400000, false });
        }
    }
    public abstract class BaseRenderPanel : UserControl, IEnumerable<Viewport>
    {
        public virtual int MaxViewports => 4;

        public enum PanelType
        {
            World,
            Hovered,
            Captured,
            Rendering,
        }

        public static Control GetPanel(PanelType type)
        {
            switch (type)
            {
                case PanelType.World: return WorldPanel;
                case PanelType.Hovered: return HoveredPanel;
                case PanelType.Captured: return CapturedPanel;
                case PanelType.Rendering: return RenderingPanel;
            }
            return null;
        }

        /// <summary>
        /// The render panel that hosts the game's current world.
        /// </summary>
        public static BaseRenderPanel WorldPanel;
        /// <summary>
        /// The render panel that the mouse is currently on top of.
        /// </summary>
        public static BaseRenderPanel HoveredPanel;
        /// <summary>
        /// The render panel that the mouse has last clicked in and not clicked out of.
        /// </summary>
        public static BaseRenderPanel CapturedPanel;
        /// <summary>
        /// The render panel that is currently being rendered to.
        /// </summary>
        public static BaseRenderPanel RenderingPanel => RenderContext.Captured?.Control;
        
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

            //_globalHud = new UIManager();
            PointToClientDelegate = new DelPointConvert(PointToClient);
            PointToScreenDelegate = new DelPointConvert(PointToScreen);

            //Create context RIGHT AWAY so render objects can bind to it as they are created
            CreateContext();
            //Add the main viewport - at least one viewport should always be rendering
            //AddViewport();

            //_timer = new Timer
            //{
            //    Interval = 500
            //};
            //_timer.Tick += (sender, e) => EndResize();
        }

        internal delegate Point DelPointConvert(Point p);
        internal DelPointConvert PointToClientDelegate;
        internal DelPointConvert PointToScreenDelegate;

        protected bool _resizing = false;
        protected VSyncMode _vsyncMode = VSyncMode.Adaptive;
        internal RenderContext _context;
        //protected UIManager _globalHud;
        protected List<Viewport> _viewports = new List<Viewport>(4);

        public List<Viewport> Viewports => _viewports;

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public UIManager GlobalHud
        //{
        //    get => _globalHud;
        //    set => _globalHud = value;
        //}

        /// <summary>
        /// Calls the method. Invokes the render panel if necessary.
        /// </summary>
        public static void CheckedInvoke(Action method, PanelType type)
        {
            if (!NeedsInvoke(method, type))
                method();
        }
        /// <summary>
        /// Returns true if the render panel needs to be invoked from the calling thread.
        /// If it does, then it calls the method.
        /// </summary>
        public static bool NeedsInvoke(Action method, PanelType type)
        {
            Control panel = GetPanel(type);
            if (panel != null && panel.InvokeRequired)
            {
                panel.Invoke(method);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Calls the method. Invokes render panel if necessary.
        /// </summary>
        public static T CheckedInvoke<T>(Func<T> method, PanelType type)
        {
            if (!NeedsInvoke(method, out T returnValue, type))
                return (T)method.DynamicInvoke();
            return returnValue;
        }
        /// <summary>
        /// Returns true if the render panel needs to be invoked from the calling thread.
        /// If it does, then it calls the method.
        /// </summary>
        public static bool NeedsInvoke<T2>(Func<T2> method, out T2 returnValue, PanelType type)
        {
            Control panel = GetPanel(type);
            if (panel != null && panel.InvokeRequired)
            {
                returnValue = (T2)panel.Invoke(method);
                return true;
            }
            returnValue = default;
            return false;
        }

        public VSyncMode VsyncMode
        {
            get => _vsyncMode;
            set
            {
                _vsyncMode = value;
                if (_context != null)
                    _context.VSyncMode = _vsyncMode;
            }
        }

        #region Rendering

        /// <summary>
        /// Tells the engine to render this panel at whatever rate the engine is running at.
        /// This panel can also be manually rendered with a custom loop by calling RenderTick(object sender, FrameEventArgs e)
        /// </summary>
        public void RegisterTick() => Engine.RegisterTick(RenderTick, UpdateTick, SwapBuffers);
        /// <summary>
        /// Tells the engine to stop rendering this panel.
        /// </summary>
        public void UnregisterTick() => Engine.UnregisterTick(RenderTick, UpdateTick, SwapBuffers);

        public void UpdateTick(object sender, FrameEventArgs e)
        {
            OnUpdate();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            Invalidate();
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_context == null || _context.IsContextDisposed())
                base.OnPaint(e);
            else// if (Monitor.TryEnter(_context))
            {
                //try
                //{
                    _context.Capture(true);
                    OnRender();
                    _context.Swap();
                    _context.ErrorCheck();
                //}
                //finally { Monitor.Exit(_context); }
            }
        }
        /// <summary>
        /// Overridden by the non-generic derived render panel to render the scene(s).
        /// </summary>
        protected abstract void OnRender();
        protected abstract void OnUpdate();
        public abstract void SwapBuffers();

        #endregion

        #region Resizing
        //protected override void OnParentChanged(EventArgs e)
        //{
        //    base.OnParentChanged(e);

        //    if (!DesignMode)
        //    {
        //        var parent = Parent;
        //        if (parent != null)
        //        {
        //            while (!(parent is Form))
        //                parent = parent.Parent;

        //            var form = parent as Form;

        //            form.ResizeBegin += (s, ea) => BeginResize();
        //            form.ResizeEnd += (s, ea) => EndResize();
        //        }
        //    }
        //}
        //private Timer _timer;
        //protected override void OnSizeChanged(EventArgs e)
        //{
        //    base.OnSizeChanged(e);
        //    _context?.Update();
        //    foreach (Viewport v in _viewports)
        //        v.Resize(Width, Height, false);
        //    _resizing = true;
        //    _timer.Start();
        //}
        private void BeginResize()
        {
            //Visible = false;
            _resizing = true;
        }
        private void EndResize()
        {
            //_timer.Stop();
            //Visible = true;
            _resizing = false;
            foreach (Viewport v in _viewports)
                v.SetInternalResolution(v.Width, v.Height);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _context?.Update();
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height, true);
        }
        #endregion

        #region Mouse
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (HoveredPanel != this)
                HoveredPanel = this;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (HoveredPanel == this)
                HoveredPanel = null;
        }
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    Capture = true;
        //}
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (CapturedPanel != this)
                CapturedPanel = this;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (CapturedPanel == this)
                CapturedPanel = null;
        }
        //protected override void OnMouseCaptureChanged(EventArgs e)
        //{
        //    base.OnMouseCaptureChanged(e);
        //    if (Capture)
        //    {
        //        if (CapturedPanel != this)
        //            CapturedPanel = this;
        //    }
        //    else
        //    {
        //        if (CapturedPanel == this)
        //            CapturedPanel = null;
        //    }
        //}
        public new Point PointToClient(Point p)
        {
            p = base.PointToClient(p);
            p.Y = Height - p.Y;
            return p;
        }
        public new Point PointToScreen(Point p)
        {
            p.Y = Height - p.Y;
            return base.PointToScreen(p);
        }
        #endregion

        #region Context
        public void CaptureContext()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CaptureContext));
                return;
            }
            _context?.Capture();
        }
        protected virtual void OnContextChanged(bool isNowCurrent)
        {
            //Don't update anything if this context has just been released
            if (isNowCurrent)
                OnResize(EventArgs.Empty);

            //_currentPanel = isNowCurrent ? this : null;
        }
        public void CreateContext()
        {
            if (Engine.DesignMode)
                return;

            switch (Engine.RenderLibrary)
            {
                case RenderLibrary.OpenGL:
                    if (_context is GLWindowContext)
                        return;

                    _context?.Dispose();
                    _context = new GLWindowContext(this);

                    break;
                case RenderLibrary.Direct3D12:
                    if (_context is DXWindowContext)
                        return;

                    _context?.Dispose();
                    _context = new DXWindowContext(this);

                    break;
                default:
                    return;
            }
            if (_context != null)
            {
                _context.ContextChanged += OnContextChanged;
                _context.ResetOccured += OnReset;
                _context.Capture(true);
                _context.Initialize();
            }
        }
        protected void DisposeContext()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }
        #endregion

        #region Viewports
        public Viewport GetOrAddViewport(LocalPlayerIndex index)
        {
            //Sometimes this method is accessed by separate threads at the same time.
            //Lock to ensure one viewport is added before the other thread checks for its existence.
            //This method probably won't be called every frame so this shouldn't be a speed issue.
            //lock (_viewports)
            //{
                return GetViewport(index) ?? AddViewport();
            //}
        }
        public Viewport GetViewport(LocalPlayerIndex index)
        {
            int i = (int)index;
            return _viewports.IndexInRange(i) ? _viewports[i] : null;
        }
        public Viewport AddViewport()
        {
            if (_viewports.Count == MaxViewports)
                return null;

            Viewport newViewport = new Viewport(this, _viewports.Count);
            _viewports.Add(newViewport);

            Engine.PrintLine("Added new viewport to {0}: {1}", GetType().GetFriendlyName(), newViewport.Index);

            //Fix the regions of the rest of the viewports
            for (int i = 0; i < _viewports.Count - 1; ++i)
            {
                Viewport p = _viewports[i];
                p.ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
                p.Resize(Width, Height);
            }

            return newViewport;
        }
        public void UnregisterController(LocalPlayerController controller)
        {
            if (IsDisposed)
                return;

            if (controller.Viewport != null && _viewports.Contains(controller.Viewport))
            {
                Viewport v = controller.Viewport;
                v.UnregisterController(controller);

                //if (v.Owners.Count == 0)
                //{
                //    _viewports.Remove(v);
                //    for (int i = 0; i < _viewports.Count; ++i)
                //    {
                //        Viewport p = _viewports[i];
                //        p.ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
                //        p.Resize(Width, Height);
                //    }
                //}
            }
        }
        #endregion

        protected virtual void OnReset(object sender, EventArgs e)
        {
            _context?.Initialize();
        }
        protected override void Dispose(bool disposing)
        {
            DisposeContext();
            base.Dispose(disposing);
        }

        public IEnumerator<Viewport> GetEnumerator() => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
    }
}
