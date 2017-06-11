using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Controls;
using System;
using System.Drawing;
using System.Diagnostics;

namespace CustomEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        private int _versionMin, _versionMax;

        private IGraphicsContext _context;
        private IWindowInfo _winInfo = null;

        public IWindowInfo WindowInfo => _winInfo;

        public GLWindowContext(RenderPanel c) : base(c)
        {
            _winInfo = Utilities.CreateWindowsWindowInfo(_control.Handle);
            _context = new GraphicsContext(GraphicsMode.Default, _winInfo);
            _context.MakeCurrent(WindowInfo);
            _context.LoadAll();
            _context.SwapInterval = 1;

            //Retrieve OpenGL information
            string vendor = GL.GetString(StringName.Vendor);
            string version = GL.GetString(StringName.Version);
            string renderer = GL.GetString(StringName.Renderer);
            string shaderVersion = GL.GetString(StringName.ShadingLanguageVersion);
            //string extensions = GL.GetString(StringName.Extensions);

            Debug.WriteLine("OPENGL VENDOR: " + vendor);
            Debug.WriteLine("OPENGL VERSION: " + version);
            Debug.WriteLine("OPENGL RENDERER: " + renderer);
            Debug.WriteLine("OPENGL SHADER LANGUAGE VERSION: " + shaderVersion);
            //Debug.WriteLine("OPENGL EXTENSIONS:\n" + string.Join("\n", extensions.Split(' ')));

            _versionMax = version[0] - 0x30;
            _versionMin = version[2] - 0x30;
        }
        public override bool IsCurrent()
        {
            if (!IsContextDisposed())
                return _context.IsCurrent;
            return false;
        }
        public override bool IsContextDisposed()
            => _context == null || _context.IsDisposed;
        protected override void OnSwapBuffers()
        {
            if (!IsContextDisposed())
                _context.SwapBuffers();
        }
        protected override void OnUpdated()
            => _context?.Update(WindowInfo);
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

        internal override AbstractRenderer GetRendererInstance()
            => new GLRenderer();

        public override void ErrorCheck()
        {
            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError && _control != null)
                _control.Reset();
        }

        public unsafe override void Initialize()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Dither);
            //GL.FrontFace(FrontFaceDirection.Ccw);
            //GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.ClearDepth(1.0f);
            GL.Enable(EnableCap.FramebufferSrgb);

            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(Matrix4.Identity.Data);

            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.AlphaTest);
            //GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.ScissorTest);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PointSmooth);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.PolygonSmooth);
            //GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.LineSmooth);

            //GL.DepthFunc(DepthFunction.Less);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
        }
        public unsafe override void BeginDraw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, Control.BackColor.A);
        }
        public override void EndDraw()
        {

        }
        internal override void OnResized(object sender, EventArgs e)
        {
            OnUpdated();
            _control.Invalidate();
        }

        public override void Flush()
        {
            GL.Flush();
        }
    }
}
