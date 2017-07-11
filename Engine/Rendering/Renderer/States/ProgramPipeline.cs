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
        //private List<RenderProgram> _includedStages = new List<RenderProgram>();

        public ProgramPipeline() : base(EObjectType.ProgramPipeline)
        {

        }
        
        public void Bind()
        {
            Engine.Renderer.BindPipeline(BindingId);
        }

        public void Add(EProgramStageMask mask, RenderProgram program)
        {
            //if (!_includedStages.Contains(program))
            //    _includedStages.Add(program);
            Engine.Renderer.UsePipeline(BindingId, mask, program.BindingId); 
        }
        public void Clear(EProgramStageMask mask)
        {
            Engine.Renderer.UsePipeline(BindingId, mask, 0);
        }
    }
}
