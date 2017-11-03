using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.DirectX;
using TheraEngine.Rendering.OpenGL;
using TheraEngine.Timers;
using System.Diagnostics;
using System.ComponentModel;

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
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// </summary>
    public class RenderPanel : UserControl, IEnumerable<Viewport>
    {
        public const int MaxViewports = 4;

        public enum PanelType
        {
            Game,
            Hovered,
            Captured,
            Rendering,
        }

        public static RenderPanel GetPanel(PanelType type)
        {
            switch (type)
            {
                case PanelType.Game:
                    return GamePanel;
                case PanelType.Hovered:
                    return HoveredPanel;
                case PanelType.Captured:
                    return CapturedPanel;
                case PanelType.Rendering:
                    return RenderingPanel;
            }
            return null;
        }

        /// <summary>
        /// The render panel that houses the actual game and viewports.
        /// </summary>
        public static RenderPanel GamePanel;
        /// <summary>
        /// The render panel that the mouse is currently on top of.
        /// </summary>
        public static RenderPanel HoveredPanel;
        /// <summary>
        /// The render panel that the mouse has last clicked in and not clicked out of.
        /// </summary>
        public static RenderPanel CapturedPanel;
        /// <summary>
        /// The render panel that is currently being rendered to.
        /// </summary>
        public static RenderPanel RenderingPanel => RenderContext.Current?.Control;

        public RenderPanel()
        {
            //Force custom paint
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque,
                true);

            _globalHud = new HudManager();
            PointToClientDelegate = new DelPointConvert(PointToClient);
            PointToScreenDelegate = new DelPointConvert(PointToScreen);

            //Create context RIGHT AWAY so render objects can bind to it as they are created
            CreateContext();
            //Add the main viewport - at least one viewport should always be rendering
            //AddViewport();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }

        internal delegate Point DelPointConvert(Point p);
        internal DelPointConvert PointToClientDelegate;
        internal DelPointConvert PointToScreenDelegate;

        private bool _resizing = false;
        private VSyncMode _vsyncMode = VSyncMode.Adaptive;
        internal RenderContext _context;
        private HudManager _globalHud;
        public List<Viewport> _viewports = new List<Viewport>(MaxViewports);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HudManager GlobalHud
        {
            get => _globalHud;
            set => _globalHud = value;
        }

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
            RenderPanel panel = GetPanel(type);
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
        public static bool NeedsInvoke<T>(Func<T> method, out T returnValue, PanelType type)
        {
            RenderPanel panel = GetPanel(type);
            if (panel != null && panel.InvokeRequired)
            {
                returnValue = (T)panel.Invoke(method);
                return true;
            }
            returnValue = default(T);
            return false;
        }

        public VSyncMode VsyncMode
        {
            get => _vsyncMode;
            set
            {
                _vsyncMode = value;
                _context.VSyncMode = _vsyncMode;
            }
        }

        #region Rendering
        public void RegisterTick() => Engine.RegisterRenderTick(RenderTick);
        public void UnregisterTick() => Engine.UnregisterRenderTick(RenderTick);
        private void RenderTick(object sender, FrameEventArgs e)
        {
            Invalidate();
            //Application.DoEvents();
            //Thread.Sleep(0);
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_context == null || _context.IsContextDisposed())
                base.OnPaint(e);
            else if (Monitor.TryEnter(_context))
            {
                try
                {
                    _context.Capture();
                    OnRender(e);
                    _context.Swap();
                    _context.ErrorCheck();
                }
                finally { Monitor.Exit(_context); }
            }
        }
        protected virtual void OnRender(PaintEventArgs e)
        {
            Engine.Scene?.RenderShadowMaps();

            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render(Engine.Scene);
            _globalHud?.Render();
            _context.EndDraw();
        }
        #endregion

        #region Resizing
        public void BeginResize()
        {
            Visible = false;
            _resizing = true;
        }
        public void EndResize()
        {
            Visible = true;
            _resizing = false;
            foreach (Viewport v in _viewports)
                v.SetInternalResolution(v.Width, v.Height);
        }
        protected override void OnResize(EventArgs e)
        {
            int w = Width.ClampMin(1);
            int h = Height.ClampMin(1);
            //_globalHud?.Resize(new Vec2(w, h));
            foreach (Viewport v in _viewports)
                v.Resize(w, h, !_resizing);
            base.OnResize(e);
            //_context?.Update();
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
            switch (Engine.RenderLibrary)
            {
                case RenderLibrary.OpenGL:
                    if (_context is GLWindowContext)
                        return;
                    
                    _context?.Dispose();
                    _context = new GLWindowContext(this);
                    
                    break;
                case RenderLibrary.Direct3D11:
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
        private void DisposeContext()
        {
            if (_context != null)
            {
                Resize -= _context.OnResized;
                _context.Unbind();
                _context.Dispose();
                _context = null;
            }
        }
        #endregion

        #region Viewports
        public Viewport GetViewport(int index) => index >= 0 && index < _viewports.Count ? _viewports[index] : null;
        public Viewport AddViewport()
        {
            if (_viewports.Count == MaxViewports)
                return null;

            Viewport newViewport = new Viewport(this, _viewports.Count);
            _viewports.Add(newViewport);

            //Fix the regions of the rest of the viewports
            for (int i = 0; i < _viewports.Count - 1; ++i)
            {
                Viewport p = _viewports[i];
                p.ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
                p.Resize(Width, Height);
            }

            return newViewport;
        }
        public void UnregisterController(LocalPlayerController owner)
        {
            if (IsDisposed)
                return;

            if (owner.Viewport != null && _viewports.Contains(owner.Viewport))
            {
                owner.Viewport.UnregisterController(owner);

                if (owner.Viewport.Owners.Count == 0)
                {
                    _viewports.Remove(owner.Viewport);
                    for (int i = 0; i < _viewports.Count; ++i)
                    {
                        Viewport p = _viewports[i];
                        p.ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
                        p.Resize(Width, Height);
                    }
                }
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
