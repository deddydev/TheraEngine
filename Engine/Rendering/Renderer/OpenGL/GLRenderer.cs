using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace CustomEngine.Rendering
{
    public class GLRenderer : RenderContext
    {
        #region Shapes
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
        #endregion

        public override void SetPointSize(System.Single size)
        {
            GL.PointSize(size);
        }
        public override void SetLineSize(System.Single size)
        {
            GL.LineWidth(size);
        }
        public override void CompileShader(System.String shader)
        {
            throw new NotImplementedException();
        }

        #region Matrices
        public override void PushMatrix()
        {
            GL.PushMatrix();
        }
        public override void PopMatrix()
        {
            GL.PopAttrib();
        }
        public override void MultMatrix(Matrix4 matrix)
        {
            GL.MultMatrix(ref matrix);
        }
        public override void Translate(float x, float y, float z)
        {
            GL.Translate(x, y, z);
        }
        public override void Scale(float x, float y, float z)
        {
            GL.Scale(x, y, z);
        }
        public override void Rotate(float roll, float pitch, float yaw)
        {
            //TODO: which method is faster?
            //Method 1:
            Matrix4 rotX = Matrix4.CreateRotationX(roll);
            Matrix4 rotY = Matrix4.CreateRotationY(pitch);
            Matrix4 rotZ = Matrix4.CreateRotationZ(yaw);
            MultMatrix(rotX * rotY * rotZ);

            //Method 2:
            //Rotate(Quaternion.FromEulerAngles(pitch, yaw, roll));
        }
        public override void Rotate(Quaternion rotation)
        {
            MultMatrix(Matrix4.CreateFromQuaternion(rotation));
        }
        #endregion
    }
}
