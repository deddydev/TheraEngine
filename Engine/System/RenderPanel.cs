using System.Collections.Generic;
using System.Windows.Forms;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Rendering.DirectX;
using System.Security.Permissions;
using System.Reflection;
using CustomEngine.Rendering.OpenGL;
using System.Threading;
using System;

namespace CustomEngine
{
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
    /// Used for rendering using OpenGL or DirectX.
    /// </summary>
    public partial class RenderPanel : Control
    {
        public RenderPanel()
        {
            SetStyle(
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.Opaque |
               ControlStyles.ResizeRedraw,
               true);
            SetRenderLibrary(RenderLibrary.OpenGL);
        }

        private RenderLibrary _currentRenderer;
        private RenderWindowContext _context;
        protected int _updateCounter;

        public HudManager _overallHud;
        public List<Viewport> _viewports = new List<Viewport>();

        public new ColorF4 BackColor { get { return _backColor; } set { _backColor = value; } }
        private ColorF4 _backColor = System.Drawing.Color.Lavender;

        public void SetRenderLibrary(RenderLibrary library)
        {
            _currentRenderer = library;
            switch (library)
            {
                case RenderLibrary.OpenGL:
                    _context = new GLWindowContext(this);
                    break;
                case RenderLibrary.DirectX:
                    _context = new DXWindowContext(this);
                    break;
            }
        }
        public void BeginUpdate() { ++_updateCounter; }
        public void EndUpdate() { if ((_updateCounter = Math.Max(_updateCounter - 1, 0)) == 0) Redraw(); }
        public void Redraw() { Invalidate(); }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
        }
        public void SetCurrent()
        {
            RenderWindowContext.CurrentContext = _context;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updateCounter > 0)
                return;

            BeginUpdate();
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
            EndUpdate();
        }
        protected virtual void OnRender(PaintEventArgs e)
        {
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render();
            _context.EndDraw();
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnHandleCreated(EventArgs e)
        {
            if (_context == null)
                SetRenderLibrary(RenderLibrary.OpenGL);
            _context.ContextChanged += OnContextChanged;
            _context.ResetOccured += OnReset;
            _context.Initialize();
        }
        protected override void DestroyHandle()
        {
            DisposeContext();
            base.DestroyHandle();
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            DisposeContext();
            base.OnHandleDestroyed(e);
        }
        protected virtual void OnReset(object sender, EventArgs e)
        {
            _context.Initialize();
        }
        protected virtual void OnContextChanged(bool isNowCurrent)
        {
            //Don't update anything if this context has just been released
            if (isNowCurrent)
                OnResize(EventArgs.Empty);

            //_currentPanel = isNowCurrent ? this : null;
        }
        protected override void Dispose(bool disposing)
        {
            DisposeContext();
            base.Dispose(disposing);
        }
        private void DisposeContext()
        {
            if (_context != null)
            {
                _context.Unbind();
                _context.Dispose();
                _context = null;
            }
        }
        public Viewport GetViewport(int viewport)
        {
            return _viewports[viewport.Clamp(0, _viewports.Count - 1)];
        }
    }
}
