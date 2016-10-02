using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomEngine.Rendering;
using CustomEngine.Rendering.OpenGL;
using CustomEngine.Rendering.DirectX;
using CustomEngine.Rendering.HUD;
using System.Threading;
using CustomEngine;
using System.Windows.Media;
using System.Windows.Interop;

namespace System.Windows.Controls
{
    public class RenderPanel : Control
    {
        private RenderLibrary _currentRenderer;
        private RenderWindowContext _context;
        protected int _updateCounter;

        public HudManager _overallHud;
        public List<Viewport> _viewports = new List<Viewport>();

        public IntPtr Handle
        {
            get
            {
                HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
                return source.Handle;
            }
        }

        public ColorF4 BackColor { get { return _backColor; } set { _backColor = value; } }
        private ColorF4 _backColor = Drawing.Color.Lavender;

        public RenderPanel()
        {
            //SetStyle(
            //    ControlStyles.UserPaint |
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.Opaque |
            //    ControlStyles.ResizeRedraw,
            //    true);
            SetRenderLibrary(RenderLibrary.OpenGL);
        }
        public void SetRenderLibrary(RenderLibrary library)
        {
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
        public void Redraw() { InvalidateVisual(); }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_updateCounter > 0)
                return;

            BeginUpdate();
            if (_context == null || _context.IsContextDisposed())
                base.OnRender(drawingContext);
            else if (Monitor.TryEnter(_context))
            {
                try
                {
                    _context.Capture();
                    _context.BeginDraw();

                    foreach (Viewport v in _viewports)
                        v.Render();

                    _context.EndDraw();
                    _context.Swap();
                    _context.ErrorCheck();
                }
                finally { Monitor.Exit(_context); }
            }
            EndUpdate();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            foreach (Viewport v in _viewports)
                v.Resize((float)Width, (float)Height);
        }
        public override void BeginInit() { base.BeginInit(); }
        protected override void OnInitialized(EventArgs e)
        {
            if (_context != null)
                _context.Initialize();
        }
        public override void EndInit()
        {
            if (_context != null)
            {
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
