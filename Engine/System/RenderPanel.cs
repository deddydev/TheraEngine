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
using System.Drawing.Imaging;
using TheraEngine.Players;

namespace TheraEngine
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
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// </summary>
    public class RenderPanel : UserControl, IEnumerable<Viewport>
    {
        public static RenderPanel HoveredPanel;
        public static RenderPanel CapturedPanel;
        public RenderPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw,
                true);

            PointToClientDelegate = new DelPointConvert(PointToClient);
            PointToScreenDelegate = new DelPointConvert(PointToScreen);
            CreateContext();
            AddViewport(new LocalPlayerController());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            DisposeContext();
            base.Dispose(disposing);
        }

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
        
        internal delegate Point DelPointConvert(Point p);
        internal DelPointConvert PointToClientDelegate;
        internal DelPointConvert PointToScreenDelegate;
        
        internal RenderContext _context;
        protected int _updateCounter;
        private HudManager _globalHud;
        public List<Viewport> _viewports = new List<Viewport>();
        private ColorF4 _backColor = Color.Lavender;
        private bool _attachedToEngine = false;

        public HudManager GlobalHud
        {
            get => _globalHud;
            set => _globalHud = value;
        }
        public new ColorF4 BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }
        
        /// <summary>
        /// Disables rendering until PopUpdate is called, unless there are other update calls on the stack.
        /// </summary>
        public void PushUpdate()
        {
            ++_updateCounter;
        }
        /// <summary>
        /// Ends the last PushUpdate call. Rendering may not resume unless the update stack is empty.
        /// </summary>
        public void PopUpdate()
        {
            if ((_updateCounter = Math.Max(_updateCounter - 1, 0)) == 0)
                Invalidate();
        }
        public void CaptureContext()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CaptureContext));
                return;
            }
            _context?.Capture();
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updateCounter > 0)
                return;

            if (_context == null || _context.IsContextDisposed())
                base.OnPaint(e);
            else if (Monitor.TryEnter(_context))
            {
                try
                {
                    _context.Capture();
                    OnRender(e);
                    _context.Swap();
                    //_context.ErrorCheck();
                    //_context.Flush();
                }
                finally { Monitor.Exit(_context); }
            }
        }
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
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //Cursor.Hide();
            Capture = true;
        }
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (Capture)
            {
                if (CapturedPanel != this)
                    CapturedPanel = this;
            }
            else
            {
                if (CapturedPanel == this)
                    CapturedPanel = null;
            }
        }
        protected virtual void OnRender(PaintEventArgs e)
        {
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render(Engine.Renderer.Scene);
            //_globalHud?.Render();
            _context.EndDraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _globalHud?.Resize(new Vec2(Width, Height));
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
            //Rectangle region = new Rectangle(0, 0, Width, Height);
            //Engine.Renderer.PopRenderArea();
            //Engine.Renderer.PushRenderArea(region);
            //Engine.Renderer.CropRenderArea(region);
        }
        protected virtual void OnReset(object sender, EventArgs e)
        {
            _context?.Initialize();
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
        public void AddViewport(LocalPlayerController owner)
        {
            _viewports.Add(new Viewport(owner, this, _viewports.Count));
            //Fix the regions of the rest of the viewports
            for (int i = 0; i < _viewports.Count - 1; ++i)
                _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
        }
        public Viewport GetViewport(int index)
            => index >= 0 && index < _viewports.Count ? _viewports[index] : null;
        public void RemoveViewport(LocalPlayerController owner)
        {
            if (_viewports.Contains(owner.Viewport))
            {
                _viewports.Remove(owner.Viewport);
                for (int i = 0; i < _viewports.Count; ++i)
                    _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
            }
        }

        public void RegisterTick()
        {
            _attachedToEngine = true;
            Engine.RegisterRenderTick(RenderTick);
        }
        public void UnregisterTick()
        {
            Engine.UnregisterRenderTick(RenderTick);
            _attachedToEngine = false;
        }
        public void RenderTick(object sender, FrameEventArgs e)
        {
            Invalidate();
        }

        public IEnumerator<Viewport> GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
    }
}
