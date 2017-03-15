using System.Windows.Forms;
using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.OpenGL;
using CustomEngine.Rendering.DirectX;
using System.Collections.Generic;
using CustomEngine.Rendering.HUD;
using System.Collections;
using CustomEngine.Input;

namespace System
{
    public partial class Test : UserControl, IEnumerable<Viewport>
    {
        internal RenderContext _context;
        protected int _updateCounter = 0;
        private HudManager _globalHud;
        private List<Viewport> _viewports = new List<Viewport>();

        public Test()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw,
                true);

            AddViewport(new LocalPlayerController());
        }
        public void SetRenderLibrary()
        {
            switch (Engine.RenderLibrary)
            {
                case RenderLibrary.OpenGL:
                    if (_context is GLWindowContext)
                        return;
                    _context?.Dispose();
                    _context = new GLWindowContext(null);
                    break;
                case RenderLibrary.Direct3D11:
                    if (_context is DXWindowContext)
                        return;
                    _context?.Dispose();
                    _context = new DXWindowContext(null);
                    break;
                default:
                    return;
            }
            _context.Capture(true);
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_updateCounter > 0)
                return;

            if (_context != null &&
                !_context.IsContextDisposed()// &&
                /*Monitor.TryEnter(_context)*/)
            {
                PushUpdate();
                try
                {
                    _context.Capture();
                    OnRender(e);
                    _context.Swap();
                    //_context.ErrorCheck();
                    //_context.Flush();
                }
                finally { /*Monitor.Exit(_context);*/ }
                PopUpdate();
            }
            else
                base.OnPaint(e);
        }
        protected virtual void OnRender(PaintEventArgs e)
        {
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render(Engine.Renderer.Scene);
            _globalHud?.Render();
            _context.EndDraw();
        }
        public void PushUpdate() { ++_updateCounter; }
        public void PopUpdate() { if ((_updateCounter = Math.Max(_updateCounter - 1, 0)) == 0) Invalidate(); }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Viewport v in _viewports)
                v.Resize(Width, Height);
        }
        protected override void OnLoad(EventArgs e)
        {
            if (_context == null)
                SetRenderLibrary();
            //_context.ContextChanged += OnContextChanged;
            //_context.ResetOccured += OnReset;
            base.OnLoad(e);
        }
        public void AddViewport(LocalPlayerController owner)
        {
            _viewports.Add(new Viewport(owner, null, _viewports.Count));
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
        public IEnumerator<Viewport> GetEnumerator()
        {
            return ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Viewport>)_viewports).GetEnumerator();
        }
    }
}
