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
using System.Drawing;
using CustomEngine.Input;
using System.Linq;
using CustomEngine.Input.Devices;

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
            SetRenderLibrary();
            AddViewport(new LocalPlayerController());
        }
        
        private RenderContext _context;
        protected int _updateCounter;
        private HudManager _globalHud;
        public List<Viewport> _viewports = new List<Viewport>();
        private ColorF4 _backColor = Color.Black;

        public HudManager GlobalHud
        {
            get { return _globalHud; }
            set { _globalHud = value; }
        }
        public new ColorF4 BackColor { get { return _backColor; } set { _backColor = value; } }
        public void SetRenderLibrary()
        {
            switch (Engine.RenderLibrary)
            {
                case RenderLibrary.OpenGL:
                    if (_context is GLWindowContext)
                        return;
                    else
                        _context?.Dispose();
                    _context = new GLWindowContext(this);
                    break;
                case RenderLibrary.Direct3D11:
                    if (_context is DXWindowContext)
                        return;
                    else
                        _context?.Dispose();
                    _context = new DXWindowContext(this);
                    break;
                default:
                    return;
            }
            _context.Capture(true);
        }
        /// <summary>
        /// Disables rendering until PopUpdate is called, unless there are other update calls on the stack.
        /// </summary>
        public void PushUpdate() { ++_updateCounter; }
        /// <summary>
        /// Ends the last PushUpdate call. Rendering may not resume unless the update stack is empty.
        /// </summary>
        public void PopUpdate() { if ((_updateCounter = Math.Max(_updateCounter - 1, 0)) == 0) Redraw(); }
        /// <summary>
        /// Redraws all viewports in this panel.
        /// </summary>
        public void Redraw() { Invalidate(); }
        public void CaptureContext() { _context?.Capture(); }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updateCounter > 0)
                return;

            PushUpdate();
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
            PopUpdate();
        }
        protected virtual void OnRender(PaintEventArgs e)
        {
            _context.BeginDraw();
            Engine.Renderer.RenderCurrentPanel();
            _context.EndDraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
            Rectangle region = new Rectangle(0, 0, Width, Height);
            Engine.Renderer.PopRenderArea();
            Engine.Renderer.PushRenderArea(region);
            Engine.Renderer.CropRenderArea(region);
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnHandleCreated(EventArgs e)
        {
            if (_context == null)
                SetRenderLibrary();
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
        public void AddViewport(LocalPlayerController owner)
        {
            _viewports.Add(new Viewport(owner, _viewports.Count));
            for (int i = 0; i < _viewports.Count; ++i)
                _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
        }
        public Viewport GetViewport(int index)
        {
            return index >= 0 && index < _viewports.Count ? _viewports[index] : null;
        }
        public void RemoveViewport(LocalPlayerController owner)
        {
            if (_viewports.Contains(owner.Viewport))
            {
                _viewports.Remove(owner.Viewport);
                for (int i = 0; i < _viewports.Count; ++i)
                    _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
            }
        }
    }
}
