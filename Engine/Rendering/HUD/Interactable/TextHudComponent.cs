using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering.HUD
{
    public class TextHudComponent : TextureHudComponent
    {
        public TextHudComponent() { }

        public string _text;
        public bool _wordWrap;
        Font _font;
    }
}
