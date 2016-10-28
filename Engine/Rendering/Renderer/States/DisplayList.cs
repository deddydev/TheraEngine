using System;

namespace CustomEngine.Rendering
{
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
        DisplayList,
    }
    public abstract class BaseRenderState
    {
        protected int _bindingId;
        private GenType _type;

        public BaseRenderState(GenType type) { _type = type; }

        public bool IsBound { get { return _bindingId > 0; } }
        public int BindingId { get { return _bindingId; } }
        public GenType Type { get { return _type; } }

        private void Generate()
        {
            if (IsBound)
                return;

            if (RenderContext.Current != null)
            {
                _bindingId = Engine.Renderer.GenObject(_type);
            }
            else
                throw new Exception("No context available.");
        }
        private void Delete()
        {
            if (!IsBound)
                return;

            if (RenderContext.Current != null)
            {
                Engine.Renderer.DeleteObject(_type, _bindingId);
                _bindingId = 0;
            }
            else
                throw new Exception("No context available.");
        }

        internal void Destroy()
        {
            Delete();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return Type.ToString() + BindingId;
        }
        public override bool Equals(object obj)
        {
            return ToString().Equals(obj.ToString());
        }
    }
    public class DisplayList : BaseRenderState
    {
        public DisplayList() : base(GenType.DisplayList) { }

        bool _hasStarted = false;
        bool _hasFinished = false;

        public void Begin()
        {
            if (_hasStarted && !_hasFinished)
                return;

            if (_hasFinished)
                Delete();

            Generate();
            Begin(DisplayListMode.Compile);
            _hasStarted = true;
            _hasFinished = false;
        }
        public void Begin(DisplayListMode mode)
        {
            if (_hasStarted && !_hasFinished)
                return;

            if (_hasFinished)
                Delete();

            Generate();
            Engine.Renderer.BeginDisplayList(_bindingId, mode);
            _hasStarted = true;
            _hasFinished = false;
        }
        public void End()
        {
            if (!_hasStarted)
                return;
            Engine.Renderer.EndDisplayList();
            _hasStarted = false;
            _hasFinished = true;
        }
        public void Call()
        {
            if (!_hasFinished)
                return;

            Engine.Renderer.CallDisplayList(_bindingId);
        }
    }
}
