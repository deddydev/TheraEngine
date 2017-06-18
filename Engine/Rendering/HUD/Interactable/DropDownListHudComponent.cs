using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.HUD.Functions
{
    public class DropDownListHudComponent : DockableHudComponent
    {
        [Category("Events")]
        public event DelScrolling Scrolled;
        public virtual void OnScrolled(bool up)
        {
            if (_scrollable)
                Scrolled?.Invoke(up);
        }
        public DropDownListHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
