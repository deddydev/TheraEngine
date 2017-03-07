using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public abstract class BaseRenderState : ObjectBase
    {
        public class ContextBind
        {
            public int _bindingId = 0;
            public RenderContext _context = null;
            public ContextBind(RenderContext c) { _context = c; }
        }

        public const int NullBindingId = 0;
        public bool IsActive { get { return _currentBind._bindingId > NullBindingId; } }
        public int BindingId
        {
            get
            {
                if (!IsActive)
                    Generate();
                return _currentBind._bindingId;
            }
        }
        public GenType Type { get { return _type; } }

        private GenType _type;
        private List<ContextBind> _owners = new List<ContextBind>();
        private ContextBind _currentBind = new ContextBind(null);
        
        public BaseRenderState(GenType type) { _type = type; }
        public BaseRenderState(GenType type, int bindingId)
        {
            _type = type;

            //Make sure current bind is up to date
            if (_currentBind._context != RenderContext.Current)
            {
                int index = _owners.Select(x => x._context).ToList().IndexOf(RenderContext.Current);
                if (index >= 0)
                    _currentBind = _owners[index];
                else
                    _owners.Add(_currentBind = new ContextBind(RenderContext.Current));
            }

            _currentBind._bindingId = bindingId;
            OnGenerated();
        }
        ~BaseRenderState()
        {
            foreach (ContextBind b in _owners)
                if (b._context != null && !b._context.IsContextDisposed())
                {
                    b._context.Capture();
                    Delete();
                }
        }
        
        /// <summary>
        /// Performs all checks needed and creates this render object on the current render context if need be.
        /// Call after capturing a context.
        /// </summary>
        public int Generate()
        {
            if (RenderContext.Current == null)
                return _currentBind._bindingId;

            //Make sure current bind is up to date
            if (_currentBind._context != RenderContext.Current)
            {
                int index = _owners.Select(x => x._context).ToList().IndexOf(RenderContext.Current);
                if (index >= 0)
                    _currentBind = _owners[index];
                else
                    _owners.Add(_currentBind = new ContextBind(RenderContext.Current));
            }

            if (IsActive)
                return _currentBind._bindingId;

            _currentBind._bindingId = CreateObject();
            OnGenerated();

            //Debug.WriteLine(this.ToString());

            return _currentBind._bindingId;
        }
        /// <summary>
        /// Removes this render object from the current context.
        /// Call after capturing a context.
        /// </summary>
        protected void Delete()
        {
            if (RenderContext.Current == null)
                return;

            //Make sure current bind is up to date
            //Remove current bind from owners list
            if (_currentBind._context != RenderContext.Current)
            {
                int index = _owners.Select(x => x._context).ToList().IndexOf(RenderContext.Current);
                if (index >= 0)
                {
                    _currentBind = _owners[index];
                    _owners.RemoveAt(index);
                }
                else
                {
                    //This state was never generated on this context in the first place
                    return;
                }
            }
            else
            {
                int index = _owners.IndexOf(_currentBind);
                if (index >= 0)
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
        internal void Destroy() { Delete(); }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return Type.ToString() + _currentBind._bindingId;
        }
        public override bool Equals(object obj)
        {
            return obj != null && ToString().Equals(obj.ToString());
        }
    }
    public enum GenType
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
        //DisplayList,
    }
}
