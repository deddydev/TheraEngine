using CustomEngine.Rendering.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;

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
        IKeyframeTrack Keyframes { get; }
        int FrameCount { get; set; }
    }
    public interface IKeyframeTrack
    {

    }
    public interface IRenderState
    {

    }
    public interface IShape : IRenderable
    {
        bool ContainsPoint(Vec3 point);
    }
}
