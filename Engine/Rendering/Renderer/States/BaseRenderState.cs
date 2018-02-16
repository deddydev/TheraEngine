using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    public abstract class BaseRenderState : TObject, IDisposable
    {
        internal const int NullBindingId = 0;
        internal class ContextBind
        {
            //internal bool _generatedFailSafe = false;
            internal int _bindingId = NullBindingId;
            internal BaseRenderState _parent;
            internal RenderContext _context = null;
            internal int _index;
            internal ContextBind(RenderContext c, BaseRenderState parent, int index)
            {
                _parent = parent;
                _context = c;
                _index = index;
                c?.States.Add(this);
            }

            internal int Index
            {
                get => _index;
                set => _index = value;
            }

            internal bool Active => BindingId > NullBindingId/* || _generatedFailSafe*/;

            internal int BindingId
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

            internal void Destroy()
            {
                _parent.DestroyContextBind(_index);
            }

            public override string ToString()
            {
                return _parent.ToString();
            }
        }

        private ContextBind CurrentBind
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

        public EObjectType Type => _type;

        private EObjectType _type;
        /// <summary>
        /// List of all render contexts this object has been generated on.
        /// </summary>
        private List<ContextBind> _owners = new List<ContextBind>();
        /// <summary>
        /// The last context that this object has been bound to or called the binding id from.
        /// </summary>
        private ContextBind _currentBind = null;

        public event Action Generated;

        public BaseRenderState(EObjectType type) { _type = type; }
        public BaseRenderState(EObjectType type, int bindingId)
        {
            _type = type;
            CurrentBind.BindingId = bindingId;
            OnGenerated();
        }

        private void DestroyContextBind(int index)
        {
            if (_currentBind != null && _currentBind._index == index)
                _currentBind = null;
            if (index >= 0 && index < _owners.Count)
            {
                _owners.RemoveAt(index);
                for (int i = index; i < _owners.Count; ++i)
                    --_owners[i].Index;
            }
        }

        private void GetCurrentBind()
        {
            if (RenderContext.Captured == null)
            {
                _currentBind = new ContextBind(null, this, -1);
                //throw new Exception("No context bound.");
            }
            else if (_currentBind == null || _currentBind._context != RenderContext.Captured)
            {
                int index = _owners.FindIndex(x => x._context == RenderContext.Captured);
                if (index >= 0)
                    _currentBind = _owners[index];
                else
                    _owners.Add(_currentBind = new ContextBind(RenderContext.Captured, this, _owners.Count));
            }
        }

        /// <summary>
        /// Performs all checks needed and creates this render object on the current render context if need be.
        /// Call after capturing a context.
        /// </summary>
        public int Generate()
        {
            if (BaseRenderPanel.NeedsInvoke(Generate, out int value, BaseRenderPanel.PanelType.Rendering))
                return value;

            //Make sure current bind is up to date
            GetCurrentBind();

            if (IsActive)
                return BindingId;

            Engine.Renderer.CheckErrors();
            int id = CreateObject();
            if (id == 0)
                throw new Exception("Unable to create render object.");
            Engine.Renderer.CheckErrors();

            CurrentBind.BindingId = id;
            OnGenerated();
            Engine.Renderer.CheckErrors();
            Generated?.Invoke();
            Engine.Renderer.CheckErrors();
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

            if (BaseRenderPanel.NeedsInvoke(Delete, BaseRenderPanel.PanelType.Rendering))
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
                int index = _currentBind.Index;
                if (index >= 0 && index < _owners.Count)
                    _owners.RemoveAt(index);
            }

            if (!IsActive)
                return;

            PreDeleted();

            Engine.Renderer.DeleteObject(_type, _currentBind._bindingId);

            _currentBind._bindingId = 0;
            _currentBind._context = null;
            PostDeleted();
        }
        /// <summary>
        /// Do not call. Override if special generation necessary.
        /// </summary>
        /// <returns></returns>
        protected virtual int CreateObject() { return Engine.Renderer.CreateObjects(_type, 1)[0]; }
        /// <summary>
        /// Called directly after this object is created on the current context.
        /// </summary>
        protected virtual void OnGenerated() { }
        /// <summary>
        /// Called directly before this object is deleted from the current context.
        /// </summary>
        protected virtual void PreDeleted() { }
        /// <summary>
        /// Called directly after this object is deleted from the current context.
        /// </summary>
        protected virtual void PostDeleted() { }
        /// <summary>
        /// Called by a context when it is being destroyed.
        /// </summary>
        internal void Destroy()
        {
            //if (currentContextOnly)
            //    Delete();
            //else
            //{
                foreach (ContextBind b in _owners)
                    if (b._context != null && !b._context.IsContextDisposed())
                    {
                        b._context.Capture();
                        Delete();
                    }
            //}
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return Type.ToString() + CurrentBind._bindingId;
        }
        public override bool Equals(object obj)
        {
            return obj != null && ToString().Equals(obj.ToString());
        }

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
    /// The type of render object that is handled by the renderer and needs to be generated/destroyed.
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
