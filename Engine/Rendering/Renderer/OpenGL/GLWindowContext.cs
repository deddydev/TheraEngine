using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Controls;
using System;

namespace CustomEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        private IGraphicsContext _context;
        private IWindowInfo _winInfo = null;

        public IWindowInfo WindowInfo { get { return _winInfo; } }

        public GLWindowContext(RenderPanel c) : base(c)
        {
            _winInfo = Utilities.CreateWindowsWindowInfo(_control.Handle);
            _context = new GraphicsContext(GraphicsMode.Default, _winInfo);
            _context.MakeCurrent(WindowInfo);
            _context.LoadAll();
        }
        public override bool IsCurrent()
        {
            if (!IsContextDisposed())
                return _context.IsCurrent;
            return false;
        }
        public override bool IsContextDisposed()
        {
            return _context == null || _context.IsDisposed;
        }
        protected override void OnSwapBuffers()
        {
            if (!IsContextDisposed())
                _context.SwapBuffers();
        }
        protected override void OnUpdated() { _context.Update(WindowInfo); }
        public override void SetCurrent(bool current)
        {
            if (!IsContextDisposed())
                _context.MakeCurrent(current ? WindowInfo : null);
        }
        public override void Dispose()
        {
            base.Dispose();
            if (_context != null)
                _context.Dispose();
            _context = null;
            if (_winInfo != null)
                _winInfo.Dispose();
            _winInfo = null;
        }

        protected override AbstractRenderer GetRendererInstance()
        {
            return GLRenderer.Instance ?? (GLRenderer.Instance = new GLRenderer());
        }

        public override void ErrorCheck()
        {
            //ErrorCode code = GL.GetError();
            //if (code != ErrorCode.NoError && _control != null)
            //    _control.Reset();
        }

        public override void Initialize()
        {
            GL.ClearDepth(1.0);

            GL.ShadeModel(ShadingModel.Smooth);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);

            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Dither);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Lighting); //Lighting will by done by shaders
            GL.Disable(EnableCap.PointSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
            GL.Disable(EnableCap.LineSmooth);

            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
            GL.CullFace(CullFaceMode.Back);
        }
        public override void BeginDraw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, Control.BackColor.A);
        }
        public override void EndDraw()
        {

        }
        protected override void OnResized(object sender, EventArgs e)
        {
            OnUpdated();
            _control.Redraw();
        }
    }
}
