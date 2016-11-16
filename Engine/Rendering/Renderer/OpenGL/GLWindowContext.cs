using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Controls;
using System;
using System.Drawing;

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
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);

            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Normalize);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Dither);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.AlphaTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.ScissorTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PolygonSmooth);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.LineSmooth);

            //GL.DepthFunc(DepthFunction.Less);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
            //GL.CullFace(CullFaceMode.Back);
        }
        Matrix4 mtx = Matrix4.Identity;
        Vec3[] pos = new Vec3[6]
{
            new Vec3(-5, -5, 20), new Vec3(5, 0, 20), new Vec3(-5, 5, 20),
            new Vec3(-5, 5, 20), new Vec3(5, 0, 20), new Vec3(5, 5, 20),
};
        public unsafe override void BeginDraw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, Control.BackColor.A);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 m2 = mtx;
            GL.LoadMatrix((float*)&m2);
            //GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);
            //Matrix4 mtx = Matrix4.LookAt(Vec3.Zero, new Vec3(0, 0, 20), Vec3.Up);
            //Matrix4 m1 = mtx;
            //GL.LoadMatrix((float*)&m1);
            GL.LoadIdentity();

            GL.Color4(Color.Red);
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.VertexPointer(3, VertexPointerType.Float, 0, pos);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.DisableClientState(ArrayCap.VertexArray);

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
