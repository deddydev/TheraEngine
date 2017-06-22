﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace TheraEngine.Rendering
{
    public abstract class RenderContext : IDisposable
    {
        public delegate void ContextChangedEventHandler(bool isCurrent);
        public event ContextChangedEventHandler ContextChanged;
        public event EventHandler ResetOccured;

        public static List<RenderContext> BoundContexts = new List<RenderContext>();

        private VSyncMode _vsyncMode;
        private static RenderContext _current;
        public static RenderContext Current
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

        public RenderPanel Control => _control;
        internal List<BaseRenderState.ContextBind> States => _states;
        public VSyncMode VSyncMode
        {
            get => _vsyncMode;
            set
            {
                _vsyncMode = value;
                foreach (ThreadSubContext c in _subContexts.Values)
                    c.VsyncChanged(_vsyncMode);
            }
        }

        protected RenderPanel _control;
        protected bool _resetting = false;
        
        protected abstract class ThreadSubContext
        {
            protected Thread _thread;
            protected IntPtr _controlHandle;

            public ThreadSubContext(IntPtr controlHandle, Thread thread)
            {
                _controlHandle = controlHandle;
                _thread = thread;
            }
            public abstract void Generate();
            public abstract bool IsCurrent();
            public abstract bool IsContextDisposed();
            public abstract void OnSwapBuffers();
            public abstract void OnUpdated();
            public abstract void SetCurrent(bool current);
            public abstract void Dispose();
            internal abstract void VsyncChanged(VSyncMode vsyncMode);
        }

        private List<BaseRenderState.ContextBind> _states = new List<BaseRenderState.ContextBind>();
        protected Dictionary<int, ThreadSubContext> _subContexts = new Dictionary<int, ThreadSubContext>();
        protected ThreadSubContext _currentSubContext;

        public RenderContext(RenderPanel c)
        {
            _control = c;
            if (_control != null)
                _control.Resize += OnResized;
            BoundContexts.Add(this);
        }

        internal abstract void OnResized(object sender, EventArgs e);

        protected void GetCurrentSubContext()
        {
            Thread thread = Thread.CurrentThread;
            if (!_subContexts.ContainsKey(thread.ManagedThreadId))
                CreateContextForThread(thread);
            _currentSubContext = _subContexts[thread.ManagedThreadId];
            _currentSubContext.SetCurrent(true);
        }
        protected abstract ThreadSubContext CreateSubContext(Thread thread);
        internal void CreateContextForThread(Thread thread)
        {
            if (thread == null)
                return;

            ThreadSubContext c = CreateSubContext(thread);
            _subContexts.Add(thread.ManagedThreadId, c);
            c.Generate();
        }
        internal void DestroyContextForThread(Thread thread)
        {
            if (thread == null)
                return;

            if (_subContexts.ContainsKey(thread.ManagedThreadId))
            {
                _subContexts[thread.ManagedThreadId].Dispose();
                _subContexts.Remove(thread.ManagedThreadId);
            }
        }

        internal abstract AbstractRenderer GetRendererInstance();

        public abstract void ErrorCheck();
        public abstract bool IsCurrent();
        public abstract bool IsContextDisposed();
        public abstract void SetCurrent(bool current);
        public void Capture(bool force = false)
        {
            try
            {
                if (force || Current != this)
                {
                    if (force)
                        Current = null;
                    Current = this;
                }
            }
            catch { Reset(); }
        }
        public void Release()
        {
            try
            {
                if (Current == this)
                {
                    Current = null;
                    ContextChanged?.Invoke(false);
                }
            }
            catch { Reset(); }
        }
        public void Swap()
        {
            Capture();
            OnSwapBuffers();
        }
        protected abstract void OnSwapBuffers();
        public void Reset()
        {
            if (_resetting) //Prevent a possible infinite loop
                return;

            _resetting = true;
            //_control.Reset();
            Dispose();

            //_winInfo = Utilities.CreateWindowsWindowInfo(_control.Handle);
            //_context = new GraphicsContext(GraphicsMode.Default, WindowInfo);
            Capture(true);
            //_context.LoadAll();
            Update();

            ResetOccured?.Invoke(this, EventArgs.Empty);

            _resetting = false;
        }
        public void Update()
        {
            if (Current == this)
                OnUpdated();
        }
        protected abstract void OnUpdated();

        public abstract void Flush();
        public abstract void Initialize();
        public abstract void BeginDraw();
        public abstract void EndDraw();
        public virtual void Unbind() { }

        #region IDisposable Support
        protected bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Capture();
                    foreach (BaseRenderState.ContextBind state in States)
                        state.Destroy();
                    States.Clear();
                    if (BoundContexts.Contains(this))
                        BoundContexts.Remove(this);
                    Release();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RenderContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
