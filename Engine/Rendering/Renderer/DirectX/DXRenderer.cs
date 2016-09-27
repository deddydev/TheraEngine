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

        public override void MultMatrix(Matrix4 matrix)
        {
            throw new NotImplementedException();
        }

        public override void PopMatrix()
        {
            throw new NotImplementedException();
        }

        public override void PushMatrix()
        {
            throw new NotImplementedException();
        }

        public override void Rotate(Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public override void Rotate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public override void Scale(float x, float y, float z)
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

        public override void Translate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }
    }
}
