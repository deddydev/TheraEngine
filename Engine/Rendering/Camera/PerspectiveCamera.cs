using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Camera
{
    public class OrthographicCamera : Camera
    {
        public override void Zoom(float amount)
        {
            TranslateRelative(0.0f, 0.0f, amount);
        }
    }
}
