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

        public static RenderPanel HoveredPanel;
        public static RenderPanel CapturedPanel;
        public RenderPanel()
        {
            //Force custom paint
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw,
                true);

            _globalHud = new HudManager();
            PointToClientDelegate = new DelPointConvert(PointToClient);
            PointToScreenDelegate = new DelPointConvert(PointToScreen);

            //Create context RIGHT AWAY so render objects can bind to it as they are created
            CreateContext();

            //Add the main viewport - at least one viewport should always be rendering
            AddViewport();
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
        
        private VSyncMode _vsyncMode = VSyncMode.Adaptive;
        internal RenderContext _context;
        protected int _updateCounter;
        private HudManager _globalHud;
        public List<Viewport> _viewports = new List<Viewport>(MaxViewports);
        private ColorF4 _backColor = Color.Lavender;

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

        public VSyncMode VsyncMode
        {
            get => _vsyncMode;
            set
            {
                _vsyncMode = value;
                _context.VSyncMode = _vsyncMode;
            }
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
                    _context.ErrorCheck();
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
                v.Render(Engine.Scene);
            //_globalHud?.Render();
            _context.EndDraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int w = Width.ClampMin(1);
            int h = Height.ClampMin(1);
            //_globalHud?.Resize(new Vec2(w, h));
            foreach (Viewport v in _viewports)
                v.Resize(w, h);
            _context?.Update();
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
        public Viewport AddViewport()
        {
            if (_viewports.Count == MaxViewports)
                return null;

            Viewport newViewport = new Viewport(this, _viewports.Count);
            _viewports.Add(newViewport);

            //Fix the regions of the rest of the viewports
            for (int i = 0; i < _viewports.Count - 1; ++i)
                _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);

            return newViewport;
        }
        public Viewport GetViewport(int index)
            => index >= 0 && index < _viewports.Count ? _viewports[index] : null;
        public void RemoveViewport(LocalPlayerController owner)
        {
            if (_viewports.Contains(owner.Viewport))
            {
                _viewports.Remove(owner.Viewport);
                for (int i = 0; i < _viewports.Count; ++i)
                    _viewports[i].ViewportCountChanged(i, _viewports.Count, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);
            }
        }

        public void RegisterTick() => Engine.RegisterRenderTick(RenderTick);
        public void UnregisterTick() => Engine.UnregisterRenderTick(RenderTick);
        private void RenderTick(object sender, FrameEventArgs e)
        {
            Invalidate();
            //Application.DoEvents();
            //Thread.Sleep(0);
        }

        public IEnumerator<Viewport> GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Viewport>)_viewports).GetEnumerator();
    }
}
