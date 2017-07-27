using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Diagnostics;
using System.Threading;

namespace TheraEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        private GLRenderer _renderer;

        protected class GLThreadSubContext : ThreadSubContext
        {
            private int _versionMin, _versionMax;
            private IGraphicsContext _context;
            private IWindowInfo _winInfo;
            private VSyncMode _vsyncMode = VSyncMode.Disabled;

            public IWindowInfo WindowInfo => _winInfo;

            public GLThreadSubContext(IntPtr controlHandle, Thread thread) 
                : base(controlHandle, thread) { }
            
            public override void Generate()
            {
                _winInfo = Utilities.CreateWindowsWindowInfo(_controlHandle);
                GraphicsMode mode = new GraphicsMode(new ColorFormat(32), 32, 0, 4, new ColorFormat(0), 2, false);
                _context = new GraphicsContext(mode, _winInfo);
                _context.MakeCurrent(WindowInfo);
                _context.LoadAll();
                VsyncChanged(_vsyncMode);

                //Retrieve OpenGL information
                string vendor = GL.GetString(StringName.Vendor);
                string version = GL.GetString(StringName.Version);
                string renderer = GL.GetString(StringName.Renderer);
                string shaderVersion = GL.GetString(StringName.ShadingLanguageVersion);
                //string extensions = GL.GetString(StringName.Extensions);
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int units);
                Engine.MaxTextureUnits = units;

                Engine.DebugPrint("Generated OpenGL context on " + _thread.Name + " thread.");
                Engine.DebugPrint("OPENGL VENDOR: " + vendor);
                Engine.DebugPrint("OPENGL VERSION: " + version);
                Engine.DebugPrint("OPENGL RENDERER: " + renderer);
                Engine.DebugPrint("OPENGL SHADER LANGUAGE VERSION: " + shaderVersion);
                Engine.DebugPrint("MAX TEXTURE UNITS: " + units);
                //Engine.DebugPrint("OPENGL EXTENSIONS:\n" + string.Join("\n", extensions.Split(' ')));

                _versionMax = version[0] - 0x30;
                _versionMin = version[2] - 0x30;
            }

            internal override void VsyncChanged(VSyncMode vsyncMode)
            {
                _vsyncMode = vsyncMode;
                if (_context == null)
                    return;
                switch (vsyncMode)
                {
                    case VSyncMode.Disabled:
                        _context.SwapInterval = 0;
                        break;
                    case VSyncMode.Enabled:
                        _context.SwapInterval = 1;
                        break;
                    case VSyncMode.Adaptive:
                        _context.SwapInterval = -1;
                        break;
                }
            }

            public override void Dispose()
            {
                if (_context != null)
                    _context.Dispose();
                _context = null;
                if (_winInfo != null)
                    _winInfo.Dispose();
                _winInfo = null;
            }

            public override bool IsContextDisposed()
                => _context == null || _context.IsDisposed;

            public override bool IsCurrent()
            {
                if (!IsContextDisposed())
                    return _context.IsCurrent;
                return false;
            }

            public override void OnSwapBuffers()
            {
                if (!IsContextDisposed())
                    _context.SwapBuffers();
            }

            public override void OnUpdated()
                => _context?.Update(WindowInfo);

            public override void SetCurrent(bool current)
            {
                if (!IsContextDisposed() && IsCurrent() != current)
                    _context.MakeCurrent(current ? WindowInfo : null);
            }
        }
        protected override ThreadSubContext CreateSubContext(Thread thread)
        {
            IntPtr handle;
            if (_control.InvokeRequired)
                handle = (IntPtr)_control.Invoke(new Func<IntPtr>(() => _control.Handle));
            else
                handle = _control.Handle;
            return new GLThreadSubContext(handle, thread);
        }

        public GLWindowContext(RenderPanel c) : base(c)
        {

        }
        public override bool IsCurrent()
        {
            GetCurrentSubContext();
            return _currentSubContext.IsCurrent();
        }
        public override bool IsContextDisposed()
        {
            GetCurrentSubContext();
            return _currentSubContext.IsContextDisposed();
        }
        protected override void OnSwapBuffers()
        {
            GetCurrentSubContext();
            _currentSubContext.OnSwapBuffers();
        }
        protected override void OnUpdated()
        {
            GetCurrentSubContext();
            _currentSubContext.OnSwapBuffers();
        }
        public override void SetCurrent(bool current)
        {
            GetCurrentSubContext();
            _currentSubContext.SetCurrent(current);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (ThreadSubContext c in _subContexts.Values)
                        c.Dispose();
                    _subContexts.Clear();
                    _currentSubContext = null;
                }
                _disposedValue = true;
            }
        }

        internal override AbstractRenderer GetRendererInstance()
            => _renderer ?? (_renderer = new GLRenderer());

        public override void ErrorCheck()
        {
            GetCurrentSubContext();
            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError && _control != null)
                _control.Reset();
        }

        public unsafe override void Initialize()
        {
            GetCurrentSubContext();

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Dither);
            //GL.FrontFace(FrontFaceDirection.Ccw);
            //GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.ClearDepth(1.0f);
            GL.Enable(EnableCap.Multisample);
            //GL.Enable(EnableCap.FramebufferSrgb);

            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            //GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);

            //GL.DepthFunc(DepthFunction.Less);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            GL.UseProgram(0);
        }
        public unsafe override void BeginDraw()
        {
            GetCurrentSubContext();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, 0.0f);
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
            GetCurrentSubContext();
            GL.Flush();
        }
    }
}
