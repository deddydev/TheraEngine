using System;
using OpenTK;

namespace CustomEngine.Rendering
{
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

        public abstract void CompileShader(string shader);
    }
}
