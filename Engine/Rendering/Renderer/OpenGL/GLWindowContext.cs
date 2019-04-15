using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Threading;

namespace TheraEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        public const bool DebugMode =
#if DEBUG
            true;
#else
            false;
#endif

        static GLWindowContext()
        {
            GraphicsContext.ShareContexts = true;
            GraphicsContext.DirectRendering = true;
        }

        public GLWindowContext(BaseRenderPanel c) : base(c) { }

        private static Lazy<GLRenderer> _renderer = new Lazy<GLRenderer>(() => new GLRenderer());

        protected override ThreadSubContext CreateSubContext(IntPtr handle, Thread thread)
            => new GLThreadSubContext(handle, thread);
        
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
//#if DEBUG
//            GetCurrentSubContext();
//            ErrorCode code = GL.GetError();
//            if (code != ErrorCode.NoError)
//            {
//                Engine.LogWarning(code.ToString());
//                _control?.Reset();
//            }
//#endif
        }

        private int[] _ignoredMessageIds =
        {
            131185, //buffer will use video memory
            131204, //no base level, no mipmaps, etc
            131169, //allocated memory for render buffer
            //131216,
            131218,
            //131076,
            //1282,
            //0,
            //9,
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
            //if (severity == DebugSeverity.DebugSeverityNotification || type == DebugType.DebugTypeOther || _printMessageIds.IndexOf(id) >= 0)
                Engine.PrintLine("OPENGL NOTIF: {0} {1} {2} {3} {4}", source, type, id, severity, s);
            //else
            //    throw new Exception(string.Format("OPENGL ERROR: {0} {1} {2} {3} {4}", source, type, id, severity, s));
            //    Engine.PrintLine("OPENGL NOTIF: {0} {1} {2} {3} {4}", source, type, id, severity, s);
        }

        private DebugProc _error;

        public unsafe override void Initialize()
        {
            GetCurrentSubContext();
            
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.TextureCubeMapSeamless);
            GL.FrontFace(FrontFaceDirection.Ccw);

            //TODO: Modify depth range so there is no loss of precision with scale and bias conversion
            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.NegativeOneToOne);

            //Fix gamma manually inside of the post process shader
            //GL.Enable(EnableCap.FramebufferSrgb);

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.UseProgram(0);

            if (DebugMode)
            {
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);

                //Throws an error if HandleDebugMessage is passed in directly
                _error = HandleDebugMessage;
                GL.DebugMessageCallback(_error, IntPtr.Zero);

                int[] ids = { };
                GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, ids, true);
            }
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

        internal override void PreRender()
        {

        }

        internal override void PostRender()
        {

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
                _context = new GraphicsContext(mode, WindowInfo, 4, 6, DebugMode ? GraphicsContextFlags.Debug : GraphicsContextFlags.Default)
                {
                    ErrorChecking = DebugMode
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
                    s.PrintLine();
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
                => _context?.IsDisposed ?? true;
            public override bool IsCurrent()
                => !IsContextDisposed() && _context.IsCurrent;
            public override void OnSwapBuffers()
                => _context.SwapBuffers();
            
            public override void OnResized(IVec2 size)
            {
                Size = size;
                _context?.Update(WindowInfo);
            }

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
