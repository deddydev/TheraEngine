using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace CustomEngine.Rendering
{
    public class GLRenderer : RenderContext
    {
        public override void DrawBoxWireframe(Vector3 min, Vector3 max)
        {
            GL.Begin(PrimitiveType.LineStrip);

            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);

            GL.End();

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, min.Y, max.Z);

            GL.End();
        }
        public override void DrawBoxSolid(Vector3 min, Vector3 max)
        {
            GL.Begin(PrimitiveType.QuadStrip);
            
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);

            GL.End();

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, min.Z);

            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, min.Z);

            GL.End();
        }

        public override void DrawCapsuleWireframe(System.Single radius, System.Single halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void DrawCapsuleSolid(System.Single radius, System.Single halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void SetPointSize(System.Single size)
        {
            throw new NotImplementedException();
        }

        public override void SetLineSize(System.Single size)
        {
            throw new NotImplementedException();
        }

        public override void CompileShader(System.String shader)
        {
            throw new NotImplementedException();
        }
    }
}
