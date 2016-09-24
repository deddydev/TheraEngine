using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CustomEngine.Rendering
{
    public class Viewport : IPanel
    {
        private HudManager _hud;
        private int _viewportNumber = 0;
        private RectangleF _region;

        public HudManager HUD { get { return _hud; } }
        public int ViewportNumber { get { return _viewportNumber; } }
        CustomGameForm Form { get { return CustomGameForm.Instance; } }

        public float Height { get { return _region.Height; } set { } }
        public float Width { get { return _region.Width; } set { } }
        public float X { get { return _region.X; } set { } }
        public float Y { get { return _region.Y; } set { } }
        public RectangleF Region { get { return _region; } set { } }

        public Viewport() { _hud = new HudManager(this); }

        public void OnResized() { }

        public void RenderTick(double deltaTime)
        {
            Form._currentWorld.RenderTick(deltaTime);
        }
    }
}
