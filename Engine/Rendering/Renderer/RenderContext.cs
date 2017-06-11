using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace TheraEngine.Rendering
{
    public abstract class RenderContext : IDisposable
    {
        public delegate void ContextChangedEventHandler(bool isCurrent);
        public event ContextChangedEventHandler ContextChanged;
        public event EventHandler ResetOccured;

        public static List<RenderContext> BoundContexts = new List<RenderContext>();
        
        private static RenderContext _current;
        public static RenderContext Current
        {
            get { return _current; }
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

        public RenderPanel Control { get { return _control; } }

        protected RenderPanel _control;
        private bool _resetting = false;
        
        public Dictionary<string, BaseRenderState> _states = new Dictionary<string, BaseRenderState>();

        public RenderContext(RenderPanel c)
        {
            _control = c;
            if (_control != null)
                _control.Resize += OnResized;
            BoundContexts.Add(this);
        }

        internal abstract void OnResized(object sender, EventArgs e);
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
        public virtual void Dispose()
        {
            Capture();
            foreach (BaseRenderState state in _states.Values)
                state.Destroy();
            if (BoundContexts.Contains(this))
                BoundContexts.Remove(this);
            Release();
        }

        public abstract void Flush();
        public abstract void Initialize();
        public abstract void BeginDraw();
        public abstract void EndDraw();
        public virtual void Unbind() { }
    }
}
