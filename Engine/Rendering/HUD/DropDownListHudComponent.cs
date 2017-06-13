using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.HUD.Functions
{
    public class DropDownListHudComponent : DockableHudComponent
    {
        public DropDownListHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
