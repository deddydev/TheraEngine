using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering
{
    public enum EFeedbackType
    {
        OutValues,
        PerVertex,
    }
    public class TransformFeedback : BaseRenderState
    {
        public TransformFeedback(EFeedbackType type, int bindingLocation, params string[] names) 
            : base(EObjectType.TransformFeedback)
        {
            _type = type;
            _names = names;
            _bindingLocation = bindingLocation;
        }

        EFeedbackType _type;
        string[] _names;
        int _bindingLocation;

        private DataBuffer _feedbackBuffer;

        protected override int CreateObject()
        {
            //_feedbackBuffer = new VertexBuffer("Feedback", _bindingLocation, BufferTarget.TransformFeedbackBuffer, false);
            return base.CreateObject();
        }

        protected override void PostGenerated()
        {
            
        }

        protected override void PostDeleted()
        {
            _feedbackBuffer.Dispose();
            _feedbackBuffer = null;
        }

        public void Bind(EFramebufferTarget type)
            => Engine.Renderer.BindTransformFeedback(BindingId);
        public void Unbind(EFramebufferTarget type)
            => Engine.Renderer.BindTransformFeedback(NullBindingId);
    }
}
