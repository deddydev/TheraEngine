using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering.HUD
{
    public class TextHudComponent : TextureHudComponent, I2DRenderable
    {
        public TextHudComponent() { }

        public string _text;
        public bool _wordWrap = true;
        Font _font;

        public bool HasTransparency => true;
    }
}
