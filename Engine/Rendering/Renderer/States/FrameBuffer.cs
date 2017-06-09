﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering
{
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(GenType.Framebuffer) { }

        public void Bind(EFramebufferType type) { Engine.Renderer.BindFrameBuffer(type, BindingId); }
        public void Unbind(EFramebufferType type) { Engine.Renderer.BindFrameBuffer(type, 0); }
    }
}
