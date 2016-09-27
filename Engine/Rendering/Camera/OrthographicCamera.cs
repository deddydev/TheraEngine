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
            float scale = amount >= 0 ? amount : 1.0f / -amount;
            Scale(scale, scale, scale);
        }
    }
}
