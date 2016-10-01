using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class DisplayList : IRenderState
    {
        public int _id = -1;
        public DisplayList() { _id = Engine.Renderer.CreateDisplayList(); }
        public void Begin() { Begin(DisplayListMode.Compile); }
        public void Begin(DisplayListMode mode) { Engine.Renderer.BeginDisplayList(_id, mode); }
        public void End() { Engine.Renderer.EndDisplayList(); }
        public void Call() { Engine.Renderer.CallDisplayList(_id); }
        public void Delete()
        {
            if (_id >= 0)
            {
                Engine.Renderer.DeleteDisplayList(_id);
                _id = 0;
            }
        }
    }
}
