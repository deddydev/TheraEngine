using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering
{
    public enum FeedbackType
    {
        OutValues,
        PerVertex,
    }
    public class TransformFeedback : BaseRenderState
    {
        public TransformFeedback(FeedbackType type, int bindingLocation, params string[] names) 
            : base(GenType.TransformFeedback)
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
            _feedbackBuffer = new VertexBuffer("Feedback", _bindingLocation, BufferTarget.TransformFeedbackBuffer);
            return base.CreateObject();
        }

        protected override void OnGenerated()
        {
            
        }

        protected override void OnDeleted()
        {
            _feedbackBuffer.Dispose();
            _feedbackBuffer = null;
        }

        public void Bind(FramebufferType type) { Engine.Renderer.BindTransformFeedback(BindingId); }
        public void Unbind(FramebufferType type) { Engine.Renderer.BindTransformFeedback(NullBindingId); }
    }
}
