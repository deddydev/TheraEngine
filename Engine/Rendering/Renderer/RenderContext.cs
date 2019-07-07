using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace TheraEngine.Rendering
{
    public delegate void DelContextsChanged(RenderContext context, bool added);
    public abstract class RenderContext : IDisposable
    {
        public delegate void ContextChangedEventHandler(bool isCurrent);
        public event ContextChangedEventHandler ContextChanged;
        public event EventHandler ResetOccured;

        public static event DelContextsChanged BoundContextsChanged;
        public static List<RenderContext> BoundContexts = new List<RenderContext>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        private EVSyncMode _vsyncMode;
        private static RenderContext _current;
        public static RenderContext Captured
        {
            get => _current;
            set
            {
                if (_current == value)
                {
                    if (_current != null && _current.IsCurrent())
                        _current.SetCurrent(true);
                    return;
                }

                if (value == null && _current != null && _current.IsCurrent())
                    _current.SetCurrent(false);

                _current = value;

                if (_current != null)
                {
                    _current.SetCurrent(true);
                    Engine.Renderer = _current.GetRendererInstance();
                    _current.ContextChanged?.Invoke(false);
                }
            }
        }
        
        public BaseRenderPanel Control => _control;
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

        protected BaseRenderPanel _control;
        protected bool _resetting = false;
        
        protected abstract class ThreadSubContext
        {
            protected Thread _thread;
            protected IntPtr _controlHandle;

            public IVec2 Size { get; protected set; }

            public ThreadSubContext(IntPtr controlHandle, Thread thread)
            {
                _controlHandle = controlHandle;
                _thread = thread;
            }
            public abstract void Generate();
            public abstract bool IsCurrent();
            public abstract bool IsContextDisposed();
            public abstract void OnSwapBuffers();
            public abstract void OnResized(IVec2 size);
            public abstract void SetCurrent(bool current);
            public abstract void Dispose();
            internal abstract void VsyncChanged(EVSyncMode vsyncMode);
        }

        protected ConcurrentDictionary<int, ThreadSubContext> _subContexts = new ConcurrentDictionary<int, ThreadSubContext>();
        protected ThreadSubContext _currentSubContext;

        public RenderContext(BaseRenderPanel c)
        {
            _control = c;
            if (_control != null)
                _control.Resize += OnResized;
            BoundContexts.Add(this);
            BoundContextsChanged?.Invoke(this, true);
        }

        private void OnResized(object sender, EventArgs e)
        {
            OnResized();
            //_control.Invalidate();
        }

        protected void GetCurrentSubContext()
        {
            Thread thread = Thread.CurrentThread;
            
             CreateContextForThread(thread);
            if (_subContexts.ContainsKey(thread.ManagedThreadId))
            {
                _currentSubContext = _subContexts[thread.ManagedThreadId];
                _currentSubContext.SetCurrent(true);
            }
        }
        protected abstract ThreadSubContext CreateSubContext(IntPtr handle, Thread thread);
        internal void CreateContextForThread(Thread thread)
        {
            if (thread == null || _control == null)
                return;
            
            if (!_subContexts.ContainsKey(thread.ManagedThreadId))
            {
                IntPtr handle = IntPtr.Zero;
                Size size = Size.Empty;
                if (_control.InvokeRequired)
                    _control.Invoke((Action)(() =>
                    {
                        handle = _control.Handle;
                        size = _control.ClientSize;
                    }));
                else
                {
                    handle = _control.Handle;
                    size = _control.ClientSize;
                }
                ThreadSubContext c = CreateSubContext(handle, thread);
                c.OnResized(size);
                c.Generate();
                _subContexts.TryAdd(thread.ManagedThreadId, c);
            }
        }
        internal void DestroyContextForThread(Thread thread)
        {
            if (thread == null)
                return;

            if (_subContexts.ContainsKey(thread.ManagedThreadId))
            {
                _subContexts[thread.ManagedThreadId].Dispose();
                _subContexts.TryRemove(thread.ManagedThreadId, out ThreadSubContext value);
            }
        }

        internal abstract AbstractRenderer GetRendererInstance();

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
        protected void OnResized()
        {
            GetCurrentSubContext();
            _currentSubContext.OnResized(_control.ClientSize);
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
        public void Update()
        {
            if (Captured == this)
                OnResized();
        }

        public abstract void Flush();
        public abstract void Initialize();
        public abstract void BeginDraw();
        public abstract void EndDraw();
        //public virtual void Unbind() { }

        #region IDisposable Support
        protected bool _disposedValue = false; // To detect redundant calls

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
                    _control.Resize -= OnResized;
                    _control = null;
                }
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
