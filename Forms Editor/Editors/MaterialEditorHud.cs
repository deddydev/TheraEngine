using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Input.Devices;

namespace TheraEditor.Editors
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Viewport v) : base(v) { }
        public MaterialEditorHud(RenderPanel p) : base(p) { }

        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
        }
    }
}
