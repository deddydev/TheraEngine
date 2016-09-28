using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering
{
    public enum MtxMode
    {
        Modelview,
        Projection,
        Texture,
        Color
    }
    [Flags]
    public enum BufferClear
    {
        Color,
        Depth,
        Stencil
    }
    public abstract class RenderContext
    {
        public void DrawBoxWireframe(Box box) { DrawBoxWireframe(box.Minimum, box.Maximum); }
        public abstract void DrawBoxWireframe(Vector3 min, Vector3 max);
        public void DrawBoxSolid(Box box) { DrawBoxSolid(box.Minimum, box.Maximum); }
        public abstract void DrawBoxSolid(Vector3 min, Vector3 max);

        public void DrawCapsuleWireframe(Capsule capsule) { DrawCapsuleWireframe(capsule.Radius, capsule.HalfHeight); }
        public abstract void DrawCapsuleWireframe(float radius, float halfHeight);
        public void DrawCapsuleSolid(Capsule capsule) { DrawCapsuleSolid(capsule.Radius, capsule.HalfHeight); }
        public abstract void DrawCapsuleSolid(float radius, float halfHeight);

        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);

        public abstract void Clear(BufferClear clearBufferMask);
        public abstract void CompileShader(string shader);

        public void Translate(Vector3 translation) { Translate(translation.X, translation.Y, translation.Z); }
        public abstract void Translate(float x, float y, float z);

        public void Scale(Vector3 scale) { Scale(scale.X, scale.Y, scale.Z); }
        public abstract void Scale(float x, float y, float z);

        public void Rotate(Vector3 rotation) { Rotate(rotation.X, rotation.Y, rotation.Z); }
        public abstract void Rotate(float x, float y, float z);
        public abstract void Rotate(Quaternion rotation);

        public abstract void PushMatrix();
        public abstract void PopMatrix();
        public abstract void MultMatrix(Matrix4 matrix);
        public abstract void MatrixMode(MtxMode modelview);
        public void LoadIdentity() { LoadMatrix(Matrix4.Identity); }
        public abstract void LoadMatrix(Matrix4 matrix);
    }
}
