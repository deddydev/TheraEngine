using CustomEngine.Rendering.Models;
using System.Drawing;

namespace CustomEngine
{
    public interface IRenderable
    {
        void Render();
    }
    public interface ITransformable
    {
        FrameState Transform { get; set; }
    }
    public interface IPanel
    {
        RectangleF Region { get; set; }
        void OnResized();
    }
    public interface IPropertyAnimation
    {

    }
}
