﻿using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TheraEngine.Rendering
{
    public delegate void DelContextsChanged(RenderContext context, bool added);
    /// <summary>
    /// This is what handles tying a renderer to the UI.
    /// </summary>
    public abstract class RenderContext : TObjectSlim, IDisposable
    {
        public static RenderContext WorldPanel { get; set; }
        public static RenderContext Hovered { get; set; }
        public static RenderContext Focused { get; set; }

        public enum EPanelType
        {
            World,
            Hovered,
            Focused,
            Rendering,
        }

        public delegate void ContextChangedEventHandler(bool isCurrent);
        public event ContextChangedEventHandler ContextChanged;
        public event EventHandler ResetOccured;

        public abstract AbstractRenderer Renderer { get; }

        private BaseRenderHandler _renderHandler;
        public BaseRenderHandler Handler
        {
            get => _renderHandler;
            set
            {
                if (_renderHandler != null && _renderHandler.Context == this)
                    _renderHandler.Context = null;
                _renderHandler = value;
                if (_renderHandler != null)
                    _renderHandler.Context = this;
            }
        }

        public static event DelContextsChanged BoundContextsChanged;
        public static List<RenderContext> BoundContexts = new List<RenderContext>();

        private EVSyncMode _vsyncMode = EVSyncMode.Adaptive;

        private static RenderContext _captured;
        public static RenderContext Captured
        {
            get => _captured;
            set
            {
                if (_captured == value)
                {
                    if (_captured != null && _captured.IsCurrent())
                        _captured.SetCurrent(true);
                    return;
                }

                if (value is null && _captured != null && _captured.IsCurrent())
                    _captured.SetCurrent(false);

                _captured = value;

                if (_captured != null)
                {
                    _captured.SetCurrent(true);
                    _captured.ContextChanged?.Invoke(false);
                }
            }
        }

        public List<BaseRenderObject.ContextBind> States { get; } = new List<BaseRenderObject.ContextBind>();

        public EVSyncMode VSyncMode
        {
            get => _vsyncMode;
            set
            {
                _vsyncMode = value;
                foreach (ThreadSubContext c in _subContexts.Values)
                    c.VsyncChanged(_vsyncMode);
            }
        }

        public IntPtr Handle => _handle;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Point ScreenLocation { get; set; }

        public Point PointToClient(Point p)
        {
            p.X -= ScreenLocation.X;
            p.Y -= ScreenLocation.Y;
            p.Y = Handler.Height - p.Y;
            return p;
        }
        public Point PointToScreen(Point p)
        {
            p.Y = Handler.Height - p.Y;
            p.X += ScreenLocation.X;
            p.Y += ScreenLocation.Y;
            return p;
        }

        protected IntPtr _handle;
        protected bool _resetting = false;
        
        protected ConcurrentDictionary<int, ThreadSubContext> _subContexts = new ConcurrentDictionary<int, ThreadSubContext>();
        protected ThreadSubContext _currentSubContext;

        public RenderContext(IntPtr handle)
        {
            _handle = handle;
            //if (_handle != null)
            //    _handle.Resize += OnResized;
            BoundContexts.Add(this);
            BoundContextsChanged?.Invoke(this, true);
        }

        private void OnResized(object sender, EventArgs e)
        {
            //OnResized();
            //_control.Invalidate();
        }

        protected void GetCurrentSubContext()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            if (!_subContexts.ContainsKey(id))
                CreateContextForThread();
            _currentSubContext = _subContexts[id];
            _currentSubContext.SetCurrent(true);
        }
        protected abstract ThreadSubContext CreateSubContext(IntPtr handle, Thread thread);
        internal int CreateContextForThread()
        {
            Thread thread = Thread.CurrentThread;

            if (thread == null || _handle == null)
                return -1;
            
            if (!_subContexts.ContainsKey(thread.ManagedThreadId))
            {
                IntPtr handle = IntPtr.Zero;
                Size size = Size.Empty;
                //if (_handle.InvokeRequired)
                //    _handle.Invoke((Action)(() =>
                //    {
                //        handle = _handle.Handle;
                //        size = _handle.ClientSize;
                //    }));
                //else
                //{
                    handle = _handle;
                    //size = _handle.ClientSize;
                //}
                ThreadSubContext c = CreateSubContext(handle, thread);
                c.OnResized(size);
                c.Generate();
                _subContexts.TryAdd(thread.ManagedThreadId, c);
            }

            return thread.ManagedThreadId;
        }
        internal void DestroyContextForThread(Thread thread)
        {
            if (thread == null)
                return;

            int id = thread.ManagedThreadId;
            if (_subContexts.ContainsKey(id))
            {
                _subContexts.TryRemove(id, out ThreadSubContext value);
                value?.Dispose();
            }
        }

        public void Update() => Handler.Update();
        public void SwapBuffers() => Handler.SwapBuffers();
        public void Render()
        {
            Capture(true);
            PreRender();
            Handler.Render();
            PostRender();
            Swap();
            ErrorCheck();
        }

        public bool IsCurrent()
        {
            GetCurrentSubContext();
            return _currentSubContext.IsCurrent();
        }
        public bool IsContextDisposed()
        {
            GetCurrentSubContext();
            return _currentSubContext?.IsContextDisposed() ?? true;
        }
        protected void OnSwapBuffers()
        {
            GetCurrentSubContext();
            try
            {
                if (!IsContextDisposed())
                    _currentSubContext.OnSwapBuffers();
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
        }
        protected void OnResized(IVec2 size)
        {
            GetCurrentSubContext();
            _currentSubContext.OnResized(size);
        }
        public void SetCurrent(bool current)
        {
            GetCurrentSubContext();
            _currentSubContext.SetCurrent(current);
        }

        public abstract void ErrorCheck();
        public void Capture(bool force = false)
        {
            //try
            //{
                if (force || Captured != this)
                {
                    if (force)
                        Captured = null;
                    Captured = this;
                }
            //}
            //catch { Reset(); }
        }
        public void Release()
        {
            //try
            //{
                if (Captured == this)
                {
                    Captured = null;
                    ContextChanged?.Invoke(false);
                }
            //}
            //catch { Reset(); }
        }
        public void Swap()
        {
            Capture();
            OnSwapBuffers();
        }

        internal abstract void PreRender();
        internal abstract void PostRender();

        //public void Reset()
        //{
        //    if (_resetting) //Prevent a possible infinite loop
        //        return;

        //    _resetting = true;
        //    //_control.Reset();
        //    Dispose();

        //    //_winInfo = Utilities.CreateWindowsWindowInfo(_control.Handle);
        //    //_context = new GraphicsContext(GraphicsMode.Default, WindowInfo);
        //    Capture(true);
        //    //_context.LoadAll();
        //    Update();

        //    ResetOccured?.Invoke(this, EventArgs.Empty);

        //    _resetting = false;
        //}
        //public void Update()
        //{
        //    //if (Captured == this)
        //    //    OnResized();
        //}

        public abstract void Flush();
        public abstract void Initialize();
        public abstract void BeginDraw();
        public abstract void EndDraw();
        //public virtual void Unbind() { }

        public void Resize(int width, int height)
        {
            GetCurrentSubContext();
            _currentSubContext.OnResized(new IVec2(width, height));

            Handler?.Resize(width, height);
        }

        public virtual void LostFocus() => Handler?.LostFocus();
        public virtual void GotFocus() => Handler?.GotFocus();
        public virtual void MouseLeave() => Handler?.MouseLeave();
        public virtual void MouseEnter() => Handler?.MouseEnter();
        
        /// <summary>
        /// Returns true if the render panel needs to be invoked from the calling thread.
        /// If it does, then it calls the method. You should return from the currently executing method if calling this on the currently executing method.
        /// This method is typically used when calling graphics methods off of the render thread (same as the main/UI thread).
        /// </summary>
        public static bool ThreadSafeBlockingInvoke<T>(Delegate method, EPanelType type, out T result, params object[] args)
        {
            RenderContext ctx = Engine.DomainProxy.GetContext(type);
            if (ctx is null)
            {
                result = default;
                return false;
            }
            Control ctrl = Control.FromHandle(ctx.Handle);
            if (ctrl is null)
            {
                result = default;
                return false;
            }
            return ctrl.ThreadSafeBlockingInvoke(method, out result, args);
        }
        /// <summary>
        /// Returns true if the render panel needs to be invoked from the calling thread.
        /// If it does, then it calls the method. You should return from the currently executing method if calling this on the currently executing method.
        /// This method is typically used when calling graphics methods off of the render thread (same as the main/UI thread).
        /// </summary>
        public static bool ThreadSafeBlockingInvoke(Delegate method, EPanelType type, params object[] args)
        {
            RenderContext ctx = Engine.DomainProxy.GetContext(type);
            if (ctx is null)
                return false;
            Control ctrl = Control.FromHandle(ctx.Handle);
            return ctrl?.ThreadSafeBlockingInvoke(method, args) ?? false;
        }

        #region IDisposable Support
        protected bool _disposedValue = false; // To detect redundant calls
        public event Action Disposing;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    //Capture();
                    //Unbind();
                    foreach (BaseRenderObject.ContextBind state in States)
                        state.Destroy();
                    States.Clear();
                    if (BoundContexts.Contains(this))
                    {
                        BoundContexts.Remove(this);
                        BoundContextsChanged?.Invoke(this, false);
                    }
                    Release();
                    //_handle.Resize -= OnResized;
                    //_handle = null;
                }
                Disposing?.Invoke();
            }
        }

        public void RecreateSelf()
        {
            var control = Control.FromHandle(Handle) as IRenderPanel;
            control?.CreateContext();
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion

        protected abstract class ThreadSubContext
        {
            protected Thread _thread;
            protected IntPtr _controlHandle;

            public IVec2 Size { get; private set; }

            public ThreadSubContext(IntPtr controlHandle, Thread thread)
            {
                _controlHandle = controlHandle;
                _thread = thread;
            }
            public abstract void Generate();
            public abstract bool IsCurrent();
            public abstract bool IsContextDisposed();
            public abstract void OnSwapBuffers();
            public virtual void OnResized(IVec2 size) => Size = size;
            public abstract void SetCurrent(bool current);
            public abstract void Dispose();
            internal abstract void VsyncChanged(EVSyncMode vsyncMode);
        }
    }
}
