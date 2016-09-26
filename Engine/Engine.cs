using CustomEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering;

namespace CustomEngine
{
    public static class Engine
    {
        private static RenderContext _renderContext;
        
        public static CustomGameForm Form { get { return CustomGameForm.Instance; } }
        public static WorldBase World { get { return Form._currentWorld; } }
        public static double RenderDelta { get { return Form.RenderTime; } }
        public static double UpdateDelta { get { return Form.UpdateTime; } }
        public static RenderContext Renderer { get { return _renderContext; } }
        public static void ShowMessage(string message, int viewport = -1)
        {
            
        }
    }
}
