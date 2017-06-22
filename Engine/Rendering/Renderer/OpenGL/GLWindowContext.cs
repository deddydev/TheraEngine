﻿using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Windows.Controls;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace TheraEngine.Rendering.OpenGL
{
    public class GLWindowContext : RenderContext
    {
        private GLRenderer _renderer;

        protected class GLThreadSubContext : ThreadSubContext
        {
            private int _versionMin, _versionMax;
            public IWindowInfo WindowInfo => _winInfo;

            public GLThreadSubContext(IntPtr controlHandle, Thread thread) 
                : base(controlHandle, thread) { }
            
            public override void Generate()
            {
                _winInfo = Utilities.CreateWindowsWindowInfo(_controlHandle);
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

                Debug.WriteLine("Generated OpenGL context on thread " + _thread.ManagedThreadId);
                Debug.WriteLine("OPENGL VENDOR: " + vendor);
                Debug.WriteLine("OPENGL VERSION: " + version);
                Debug.WriteLine("OPENGL RENDERER: " + renderer);
                Debug.WriteLine("OPENGL SHADER LANGUAGE VERSION: " + shaderVersion);
                //Debug.WriteLine("OPENGL EXTENSIONS:\n" + string.Join("\n", extensions.Split(' ')));

                _versionMax = version[0] - 0x30;
                _versionMin = version[2] - 0x30;
            }

            private IGraphicsContext _context;
            private IWindowInfo _winInfo;

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
            //GL.Enable(EnableCap.FramebufferSrgb);

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
            GetCurrentSubContext();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Control.BackColor.R, Control.BackColor.G, Control.BackColor.B, 0.0f);
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
