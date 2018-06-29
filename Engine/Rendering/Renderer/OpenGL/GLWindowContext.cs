using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Text;
using System.Threading;
using TheraEngine.Core.Extensions;

namespace TheraEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        static GLWindowContext()
        {
            GraphicsContext.ShareContexts = true;
            GraphicsContext.DirectRendering = true;
        }

        public GLWindowContext(BaseRenderPanel c) : base(c) { }

        private static Lazy<GLRenderer> _renderer = new Lazy<GLRenderer>(() => new GLRenderer());

        protected override ThreadSubContext CreateSubContext(Thread thread)
        {
            IntPtr handle;
            if (_control.InvokeRequired)
                handle = (IntPtr)_control.Invoke(new Func<IntPtr>(() => _control.Handle));
            else
                handle = _control.Handle;
            return new GLThreadSubContext(handle, thread);
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

        internal override AbstractRenderer GetRendererInstance() => _renderer.Value;

        public override void ErrorCheck()
        {
            GetCurrentSubContext();
            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError)
            {
                Engine.LogWarning(code.ToString());
                _control?.Reset();
            }
        }

        private int[] _ignoredMessageIds =
        {
            131185,
            131204,
            131169,
            //131216,
            //131218,
            //131076,
            //1282,
        };
        private int[] _printMessageIds =
        {
            //1280, //Invalid texture format and type combination
            //1281, //Invalid texture format
            //1282,
        };
        internal unsafe void HandleDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            if (_ignoredMessageIds.IndexOf(id) >= 0)
                return;

            string s = new string((sbyte*)message);
            if (severity == DebugSeverity.DebugSeverityNotification || type == DebugType.DebugTypeOther || _printMessageIds.IndexOf(id) >= 0)
                Engine.PrintLine("OPENGL NOIF: {0} {1} {2} {3} {4}", source, type, id, severity, s);
            else
                throw new Exception(string.Format("OPENGL ERROR: {0} {1} {2} {3} {4}", source, type, id, severity, s));
        }

        private DebugProc _error;

        public unsafe override void Initialize()
        {
            GetCurrentSubContext();

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.Disable(EnableCap.CullFace);
            //GL.Enable(EnableCap.Dither);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.TextureCubeMapSeamless);
            GL.FrontFace(FrontFaceDirection.Ccw);
            //GL.ClearDepth(1.0f);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            //Throws an error if HandleDebugMessage is passed in directly
            _error = HandleDebugMessage;
            GL.DebugMessageCallback(_error, IntPtr.Zero);

            int[] ids = { };
            GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, ids, true);

            //Modify depth range so there is no loss of precision with scale and bias conversion
            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.NegativeOneToOne);
            //GL.DepthRange(0.0, 1.0);
            //GL.Enable(EnableCap.FramebufferSrgb);
            //GL.Enable(EnableCap.StencilTest);
            //GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            //GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
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

            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, 0.0f);
        }
        public override void EndDraw()
        {

        }
        
        public override void Flush()
        {
            GetCurrentSubContext();
            GL.Flush();
        }

        protected class GLThreadSubContext : ThreadSubContext
        {
            private int _versionMin, _versionMax;
            private IGraphicsContext _context;
            private VSyncMode _vsyncMode = VSyncMode.Adaptive;
#if DEBUG
            private static bool _hasPrintedInfo = false;
#endif

            public IWindowInfo WindowInfo { get; private set; }

            public GLThreadSubContext(IntPtr controlHandle, Thread thread)
                : base(controlHandle, thread) { }

            public override void Generate()
            {
                Engine.RenderThreadId = Thread.CurrentThread.ManagedThreadId;
                WindowInfo = Utilities.CreateWindowsWindowInfo(_controlHandle);
                GraphicsMode mode = new GraphicsMode(new ColorFormat(32), 24, 8, 8, new ColorFormat(0), 2, false);
                _context = new GraphicsContext(mode, WindowInfo, 4, 6, GraphicsContextFlags.Debug)
                {
                    ErrorChecking = true
                };
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
                Engine.ComputerInfo.MaxTextureUnits = units;

                Engine.PrintLine("Generated OpenGL context on thread " + _thread.ManagedThreadId + ".");

#if DEBUG
                if (!_hasPrintedInfo)
                {
                    _hasPrintedInfo = true;
                    string s = "";
                    s += "VENDOR: " + vendor + Environment.NewLine;
                    s += "VERSION: " + version + Environment.NewLine;
                    s += "RENDERER: " + renderer + Environment.NewLine;
                    s += "GLSL VER: " + shaderVersion + Environment.NewLine;
                    s += "TOTAL TEX UNITS: " + units + Environment.NewLine;
                    //s += "EXTENSIONS:" + Environment.NewLine + string.Join(Environment.NewLine, extensions.Split(' ')) + Environment.NewLine;
                    s.Print();
                }
#endif

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
                    case VSyncMode.Disabled: _context.SwapInterval = 0; break;
                    case VSyncMode.Enabled: _context.SwapInterval = 1; break;
                    case VSyncMode.Adaptive: _context.SwapInterval = -1; break;
                }
            }

            public override void Dispose()
            {
                if (_context != null)
                    _context.Dispose();
                _context = null;
                if (WindowInfo != null)
                    WindowInfo.Dispose();
                WindowInfo = null;
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
                try
                {
                    if (!IsContextDisposed())
                        _context.SwapBuffers();
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
            }

            public override void OnResized(Vec2 size)
                => _context?.Update(WindowInfo);

            public override void SetCurrent(bool current)
            {
                try
                {
                    //Engine.PrintLine(Thread.CurrentThread.ManagedThreadId.ToString());
                    if (!IsContextDisposed() && IsCurrent() != current)
                        _context.MakeCurrent(current ? WindowInfo : null);
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
            }
        }
    }
}
