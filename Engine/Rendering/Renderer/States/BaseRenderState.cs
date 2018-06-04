using System;
using System.Collections.Generic;
using System.Threading;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Identifies a render object that is handled by the renderer and needs to be generated/destroyed during runtime.
    /// </summary>
    public abstract class BaseRenderState : TObject, IDisposable
    {
        public const int NullBindingId = 0;
        public class ContextBind
        {
            //internal bool _generatedFailSafe = false;
            internal int _bindingId = NullBindingId;
            private BaseRenderState _parentState;
            internal RenderContext _context = null;
            internal ContextBind(RenderContext context, BaseRenderState parentState, int contextIndex)
            {
                _parentState = parentState;
                _context = context;
                ContextIndex = contextIndex;
                context?.States.Add(this);
            }

            public DateTime? GenerationTime { get; internal set; } = null;
            public string GenerationStackTrace { get; internal set; }
            public int ContextIndex { get; internal set; }

            public bool Active => BindingId > NullBindingId/* || _generatedFailSafe*/;
            public int BindingId
            {
                get => _bindingId;
                set
                {
                    if (_bindingId > NullBindingId)
                        throw new Exception("Context binding already has an id!");
                    _bindingId = value;
                    //if (_bindingId == 0)
                    //    _generatedFailSafe = true;
                }
            }

            public BaseRenderState ParentState => _parentState;

            public void Destroy() => _parentState.DestroyContextBind(ContextIndex);
            public override string ToString() => _parentState.ToString();
        }

        internal ContextBind CurrentBind
        {
            get
            {
                //Make sure current bind is up to date
                GetCurrentBind();
                return _currentBind;
            }
        }
        public bool IsActive => CurrentBind.Active;
        public int BindingId
        {
            get
            {
                //Generate if not active already
                if (!IsActive)
                    Generate();

                return CurrentBind.BindingId;
            }
        }

        public EObjectType Type { get; private set; }

        /// <summary>
        /// List of all render contexts this object has been generated on.
        /// </summary>
        private List<ContextBind> _owners = new List<ContextBind>();
        /// <summary>
        /// The last context that this object has been bound to or called the binding id from.
        /// </summary>
        private ContextBind _currentBind = null;

        public event Action Generated;

        public BaseRenderState(EObjectType type) => Type = type;
        public BaseRenderState(EObjectType type, int bindingId)
        {
            Type = type;
            CurrentBind.BindingId = bindingId;
            PostGenerated();
        }

        private void DestroyContextBind(int index)
        {
            if (_currentBind != null && _currentBind.ContextIndex == index)
                _currentBind = null;
            if (index >= 0 && index < _owners.Count)
            {
                _owners.RemoveAt(index);
                for (int i = index; i < _owners.Count; ++i)
                    --_owners[i].ContextIndex;
            }
        }

        private bool GetCurrentBind()
        {
            if (RenderContext.Captured == null)
            {
                _currentBind = new ContextBind(null, this, -1);
                //throw new Exception("No context bound.");
                return false;
            }
            else if (_currentBind == null || _currentBind._context != RenderContext.Captured)
            {
                int index = _owners.FindIndex(x => x._context == RenderContext.Captured);
                if (index >= 0)
                    _currentBind = _owners[index];
                else
                    _owners.Add(_currentBind = new ContextBind(RenderContext.Captured, this, _owners.Count));
            }
            return true;
        }

        /// <summary>
        /// Performs all checks needed and creates this render object on the current render context if need be.
        /// Call after capturing a context.
        /// </summary>
        public int Generate()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Func<int>)Generate, BaseRenderPanel.PanelType.Rendering, out int id))
                return id;

            //Make sure current bind is up to date
            bool hasBind = GetCurrentBind();
            if (!hasBind)
            {
                Engine.LogWarning("Unable to create render object: no captured render context.");
                return NullBindingId;
            }

            if (IsActive)
                return BindingId;
            
            PreGenerated();
            id = CreateObject();
            if (id != 0)
            {
                CurrentBind.BindingId = id;
                CurrentBind.GenerationStackTrace = Engine.GetStackTrace();
                CurrentBind.GenerationTime = DateTime.Now;
                PostGenerated();
                Generated?.Invoke();
            }
            else
                Engine.LogWarning("Unable to create render object.");
            return id;
        }

        /// <summary>
        /// Removes this render object from the current context.
        /// Call after capturing a context.
        /// </summary>
        protected void Delete()
        {
            if (RenderContext.Captured == null)
                return;

            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)Delete, BaseRenderPanel.PanelType.Rendering))
                return;

            //Remove current bind from owners list
            if (_currentBind == null || _currentBind._context != RenderContext.Captured)
            {
                int index = _owners.FindIndex(x => x._context == RenderContext.Captured);
                if (index >= 0)
                {
                    _currentBind = _owners[index];
                    _owners.RemoveAt(index);
                }
                else
                    return; //This state was never generated on this context in the first place
            }
            else
            {
                int index = _currentBind.ContextIndex;
                if (index >= 0 && index < _owners.Count)
                    _owners.RemoveAt(index);
            }

            if (!IsActive)
                return;

            PreDeleted();

            Engine.Renderer.DeleteObject(Type, _currentBind._bindingId);

            _currentBind._bindingId = 0;
            _currentBind._context = null;
            _currentBind.GenerationStackTrace = null;
            _currentBind.GenerationTime = null;
            PostDeleted();
        }
        /// <summary>
        /// Do not call. Override if special generation necessary.
        /// </summary>
        /// <returns>The handle to the object.</returns>
        protected virtual int CreateObject() => Engine.Renderer.CreateObject(Type);
        protected virtual void PreGenerated() { }
        /// <summary>
        /// Called directly after this object is created on the current context.
        /// </summary>
        protected virtual void PostGenerated() { }
        /// <summary>
        /// Called directly before this object is deleted from the current context.
        /// </summary>
        protected virtual void PreDeleted() { }
        /// <summary>
        /// Called directly after this object is deleted from the current context.
        /// </summary>
        protected virtual void PostDeleted() { }
        /// <summary>
        /// Deletes this object from ALL contexts.
        /// </summary>
        public virtual void Destroy()
        {
            ContextBind b;
            int prevCount;
            while ((prevCount = _owners.Count) > 0)
            {
                b = _owners[0];
                if (b._context != null && !b._context.IsContextDisposed())
                {
                    b._context.Capture();
                    Delete();
                    if (prevCount == _owners.Count)
                        _owners.RemoveAt(0);
                }
                else
                    _owners.RemoveAt(0);
            }
        }

        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => Type.ToString() + BindingId;
        public override bool Equals(object obj) => obj != null && ToString().Equals(obj.ToString());
        
        #region IDisposable Support
        protected bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseRenderState() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    /// <summary>
    /// The type of render object that is handled by the renderer.
    /// </summary>
    public enum EObjectType
    {
        Buffer,
        Shader,
        Program,
        VertexArray,
        Query,
        ProgramPipeline,
        TransformFeedback,
        Sampler,
        Texture,
        Renderbuffer,
        Framebuffer,
    }
}
