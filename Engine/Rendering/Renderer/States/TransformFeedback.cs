using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering
{
    public enum FeedbackType
    {
        OutValues,
        PerVertex,
    }
    public class TransformFeedback : BaseRenderState
    {
        public TransformFeedback(FeedbackType type, int bindingLocation, params string[] names) 
            : base(EObjectType.TransformFeedback)
        {
            _type = type;
            _names = names;
            _bindingLocation = bindingLocation;
        }

        FeedbackType _type;
        string[] _names;
        int _bindingLocation;

        private VertexBuffer _feedbackBuffer;

        protected override int CreateObject()
        {
            //_feedbackBuffer = new VertexBuffer("Feedback", _bindingLocation, BufferTarget.TransformFeedbackBuffer, false);
            return base.CreateObject();
        }

        protected override void OnGenerated()
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
