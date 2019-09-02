using System.Collections.Generic;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    public interface ITextProjector
    {
        UIString3D[] Text { get; }
    }
    public class UITextProjectionComponent : UITextComponent
    {
        public UITextProjectionComponent() : base() { }

        public void SubmitProjectors(IEnumerable<ITextProjector> projectors)
        {
            //TextDrawer.Intersect(projectors.SelectMany(x => x.Text));
        }
    }
}
