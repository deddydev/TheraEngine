using Extensions;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace TheraEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        public override AbstractRenderer Renderer { get; } = new GLRenderer();

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

        public GLWindowContext(IntPtr? handle) : base(handle) { }

        protected internal override ThreadSubContext CreateSubContext(IntPtr? handle, Thread thread)
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
        internal unsafe void HandleDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr messagePtr, IntPtr userParam)
        {
            if (_ignoredMessageIds.IndexOf(id) >= 0)
                return;

            string messageStr = new string((sbyte*)messagePtr);
            //if (severity == DebugSeverity.DebugSeverityNotification || type == DebugType.DebugTypeOther || _printMessageIds.IndexOf(id) >= 0)
                Engine.LogWarning($"OPENGL NOTIF: {source} {type} {id} {severity} {messageStr}", 1, 5);
            //else
            //    throw new Exception(string.Format("OPENGL ERROR: {0} {1} {2} {3} {4}", source, type, id, severity, s));
            //    Engine.PrintLine("OPENGL NOTIF: {0} {1} {2} {3} {4}", source, type, id, severity, s);
        }

        private DebugProc _error;

        public unsafe override void Initialize()
        {
            IsInitialized = true;
            GetCurrentSubContext(true);
            
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
            GetCurrentSubContext(true);

            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, 0.0f);
        }
        public override void EndDraw()
        {

        }
        
        public override void Flush()
        {
            GetCurrentSubContext(true);
            GL.Flush();
        }

        internal override void BeforeRender()
        {

        }

        internal override void AfterRender()
        {

        }

        protected class GLThreadSubContext : ThreadSubContext
        {
            private IGraphicsContext _context;
            private EVSyncMode _vsyncMode = EVSyncMode.Disabled;

            public IWindowInfo WindowInfo { get; private set; }

            public GLThreadSubContext(IntPtr? controlHandle, Thread thread)
                : base(controlHandle, thread) { }

            public class GLSpecs
            {
                public string Vendor { get; set; }
                public string Version { get; set; }
                public string Renderer { get; set; }
                public string ShaderLanguageVersion { get; set; }
                public string[] Extensions { get; set; }
                public int MaxTextureUnits { get; set; }

                public int VersionMax => Version[0] - 0x30;
                public int VersionMin => Version[2] - 0x30;

                public bool HasExtension(string extension)
                    => Array.BinarySearch(Extensions, extension) >= 0;

                public GLSpecs()
                {

                }

                public void Analyze()
                {
                    Vendor = GL.GetString(StringName.Vendor);
                    Version = GL.GetString(StringName.Version);
                    Renderer = GL.GetString(StringName.Renderer);
                    ShaderLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);

                    Extensions = GL.GetString(StringName.Extensions).Split(' ');
                    Array.Sort(Extensions);

                    GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int units);
                    Engine.ComputerInfo.MaxTextureUnits = MaxTextureUnits = units;
                }

                public void Print()
                {
                    string output = string.Empty;
                    output += "VENDOR: " + Vendor + Environment.NewLine;
                    output += "VERSION: " + Version + Environment.NewLine;
                    output += "RENDERER: " + Renderer + Environment.NewLine;
                    output += "GLSL: " + ShaderLanguageVersion + Environment.NewLine;
                    output += "TEXTURE UNITS: " + MaxTextureUnits + Environment.NewLine;
                    string ext = string.Join(Environment.NewLine, Extensions);
                    Engine.Out(EOutputVerbosity.Normal, output);
                    Engine.Out(EOutputVerbosity.Verbose, ext);
                }
            }

            public static GLSpecs Specification { get; private set; }

            public const string VRWindowTitle = "VR Hidden Window";

            private static List<IntPtr> Handles { get; } = new List<IntPtr>();

            public override void Generate()
            {
                GraphicsMode mode = new GraphicsMode(new ColorFormat(32), 24, 8, 4, new ColorFormat(0), 2, false);

                if (_controlHandle is null)
                {
                    Engine.Out("Creating hidden window for OpenGL context.");
                    //_hiddenWindow = new NativeWindow(0, 0, 0, 0, VRWindowTitle, GameWindowFlags.FixedWindow, mode, DisplayDevice.Default) { Visible = false };
                    _controlHandle = Engine.Instance.CreateDummyFormHandle();
                }
                else
                {
                    Engine.Out("Using existing window for OpenGL context.");
                    Handles.Add(_controlHandle.Value);
                }

                Engine.RenderThreadId = Thread.CurrentThread.ManagedThreadId;

                WindowInfo = Utilities.CreateWindowsWindowInfo(_controlHandle.Value);

                GraphicsContextFlags flags = DebugMode ? GraphicsContextFlags.Debug : GraphicsContextFlags.Default;
                _context = new GraphicsContext(mode, WindowInfo, 4, 6, flags) { ErrorChecking = DebugMode };

                _context.MakeCurrent(WindowInfo);
                _context.LoadAll();

                VsyncChanged(_vsyncMode);

                //Retrieve OpenGL information
                bool readSpec = Specification is null;
                if (readSpec)
                {
                    Specification = new GLSpecs();
                    Specification.Analyze();
                }

                Engine.Out(EOutputVerbosity.Normal, $"Generated OpenGL context on thread {_thread.ManagedThreadId}.");

#if DEBUG
                if (readSpec)
                    Specification.Print();
#endif
            }

            internal override void VsyncChanged(EVSyncMode vsyncMode)
            {
                _vsyncMode = vsyncMode;
                if (_context is null)
                    return;
                switch (vsyncMode)
                {
                    case EVSyncMode.Disabled: _context.SwapInterval = 0; break;
                    case EVSyncMode.Enabled: _context.SwapInterval = 1; break;
                    case EVSyncMode.Adaptive: _context.SwapInterval = -1; break;
                }
            }

            public override void Dispose()
            {
                _context?.Dispose();
                _context = null;

                WindowInfo?.Dispose();
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
                base.OnResized(size);
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
