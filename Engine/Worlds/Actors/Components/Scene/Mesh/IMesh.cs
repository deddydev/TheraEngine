using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public interface IMesh : IRenderable
    {
        bool Visible { get; set; }
        bool VisibleInEditorOnly { get; set; }
        bool HiddenFromOwner { get; set; }
        bool VisibleToOwnerOnly { get; set; }
    }
}
