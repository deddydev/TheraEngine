using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using OpenTK;

namespace CustomEngine.Rendering
{
    public class DXRenderer : RenderContext
    {
        public override void CompileShader(string shader)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxSolid(Vector3 min, Vector3 max)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxWireframe(Vector3 min, Vector3 max)
        {
            throw new NotImplementedException();
        }

        public override void DrawCapsuleSolid(float radius, float halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void DrawCapsuleWireframe(float radius, float halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void SetLineSize(float size)
        {
            throw new NotImplementedException();
        }

        public override void SetPointSize(float size)
        {
            throw new NotImplementedException();
        }
    }
}
