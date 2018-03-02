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

        public void Update()
        {

        }

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 rect = base.Resize(parentBounds);
            TextTexture.Resize((int)(Width * TexScale.X), (int)(Height * TexScale.Y));
            Redraw();
            return rect;
        }
    }
}
