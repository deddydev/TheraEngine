using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TheraEngine.Rendering
{

    public class ProgramPipeline : BaseRenderState
    {
        public ProgramPipeline() : base(EObjectType.ProgramPipeline) { }
        public void Bind()
        {
            Engine.Renderer.BindPipeline(BindingId);
        }
        public void Set(EProgramStageMask mask, int programBindingId)
        {
            Engine.Renderer.SetPipelineStage(BindingId, mask, programBindingId); 
        }
        public void Clear(EProgramStageMask mask)
        {
            Engine.Renderer.SetPipelineStage(BindingId, mask, 0);
        }
        public void SetActive(int programBindingId)
        {
            Engine.Renderer.ActiveShaderProgram(BindingId, programBindingId);
        }
    }
}
