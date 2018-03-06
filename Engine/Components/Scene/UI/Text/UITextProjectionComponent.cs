using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.UI
{
    public interface ITextProjector
    {
        string[] GetProjectedText();
        Vec3[] GetTextPositions();
    }
    public class UITextProjectionComponent : UITextComponent
    {
        public UITextProjectionComponent() : base() { }

        private HashSet<ITextProjector> _projectors = new HashSet<ITextProjector>();
    }
}
