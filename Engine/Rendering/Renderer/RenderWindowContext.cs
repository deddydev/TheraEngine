using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CustomEngine.Rendering
{
    public abstract class RenderWindowContext : IDisposable
    {
        public delegate void ContextChangedEventHandler(bool isCurrent);
        public event ContextChangedEventHandler ContextChanged;
        public event EventHandler ResetOccured;

        public static List<RenderWindowContext> BoundContexts = new List<RenderWindowContext>();

        private static RenderWindowContext _currentContext;
        public static RenderWindowContext CurrentContext
        {
            get { return _currentContext; }
            set
            {
                if (_currentContext == value)
                {
                    if (_currentContext != null && _currentContext.IsCurrent())
                        _currentContext.SetCurrent(true);
                    return;
                }

                if (value == null && _currentContext != null && _currentContext.IsCurrent())
                    _currentContext.SetCurrent(false);

                _currentContext = value;

                if (_currentContext != null)
                {
                    _currentContext.SetCurrent(true);
                    Engine.Renderer = _currentContext.GetRendererInstance();
                }
            }
        }

        public RenderPanel Control { get { return _control; } }

        protected RenderPanel _control;
        private bool _resetting = false;
        public Dictionary<string, IRenderState> _states = new Dictionary<string, IRenderState>();

        public RenderWindowContext(RenderPanel c)
        {
            _control = c;
            _control.Resize += OnResized;
            BoundContexts.Add(this);
        }

        protected abstract void OnResized(object sender, EventArgs e);
        protected abstract AbstractRenderer GetRendererInstance();
        public abstract void ErrorCheck();
        public abstract bool IsCurrent();
        public abstract bool IsContextDisposed();
        public abstract void SetCurrent(bool current);
        public void Capture(bool force = false)
        {
            try
            {
                if (force || CurrentContext != this)
                {
                    if (force)
                        CurrentContext = null;
                    CurrentContext = this;
                    ContextChanged?.Invoke(false);
                }
            }
            catch { Reset(); }
        }
        public void Release()
        {
            try
            {
                if (CurrentContext == this)
                {
                    CurrentContext = null;
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
            if (CurrentContext == this)
                OnUpdated();
        }
        protected abstract void OnUpdated();
        public virtual void Dispose()
        {
            Release();
            if (BoundContexts.Contains(this))
                BoundContexts.Remove(this);
        }

        public abstract void Initialize();
        public abstract void BeginDraw();
        public abstract void EndDraw();
        public virtual void Unbind() { }
    }
}
