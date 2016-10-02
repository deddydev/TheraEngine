using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering
{
    public delegate T GLCreateHandler<T>() where T : IRenderState;
    /// <summary>
    /// This class is meant to be overridden with an implementation such as OpenTK or DirectX
    /// </summary>
    public abstract class AbstractRenderer : IDisposable
    {
        private static List<DisplayList> _displayLists = new List<DisplayList>();

        public static T FindOrCreate<T>(string name, GLCreateHandler<T> handler) where T : IRenderState
        {
            if (RenderWindowContext.CurrentContext == null)
                return default(T);

            if (RenderWindowContext.CurrentContext._states.ContainsKey(name))
                return (T)RenderWindowContext.CurrentContext._states[name];
            T obj = handler();
            RenderWindowContext.CurrentContext._states[name] = obj;
            return obj;
        }

        #region Shapes
        public void DrawBoxWireframe(Box box) { DrawBoxWireframe(box.Minimum, box.Maximum); }
        public void DrawBoxSolid(Box box) { DrawBoxSolid(box.Minimum, box.Maximum); }
        public abstract void DrawBoxWireframe(Vector3 min, Vector3 max);
        public abstract void DrawBoxSolid(Vector3 min, Vector3 max);

        public void DrawCapsuleWireframe(Capsule capsule) { DrawCapsuleWireframe(capsule.Radius, capsule.HalfHeight); }
        public void DrawCapsuleSolid(Capsule capsule) { DrawCapsuleSolid(capsule.Radius, capsule.HalfHeight); }
        public abstract void DrawCapsuleWireframe(float radius, float halfHeight);
        public abstract void DrawCapsuleSolid(float radius, float halfHeight);

        public void DrawSphereWireframe(Sphere sphere) { DrawSphereWireframe(sphere.Radius); }
        public void DrawSphereSolid(Sphere sphere) { DrawSphereSolid(sphere.Radius); }
        public abstract void DrawSphereWireframe(float radius);
        public abstract void DrawSphereSolid(float radius);
        #endregion

        #region Matrices
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
        #endregion

        #region Display Lists
        public abstract int CreateDisplayList();
        public abstract void BeginDisplayList(int id, DisplayListMode mode);
        public abstract void EndDisplayList();
        public abstract void CallDisplayList(int id);
        public abstract void DeleteDisplayList(int id);
        #endregion

        #region Drawing

        public abstract void Begin(EPrimitive type);
        public abstract void Vertex3(Vector3 value);
        public abstract void Vertex2(Vector2 value);
        public abstract void Normal3(Vector3 value);
        public abstract void TexCoord2(Vector2 value);
        public abstract void MultiTexCoord2(int unit, Vector2 value);
        public abstract void Color4(ColorF4 value);
        public abstract void Color3(ColorF3 value);
        public abstract void End();

        #endregion

        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);

        public abstract void Clear(BufferClear clearBufferMask);
        public abstract float GetDepth(float x, float y);
        
        public abstract void CompileShader(string shaderHandle);
        public abstract void AttachShader(int programHandle, int shaderHandle);
        public abstract void LinkProgram(int programHandle);

        public void Dispose()
        {
            _displayLists = null;
        }
    }

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
    public enum DisplayListMode
    {
        //Means this displaylist should not be applied until it is called.
        Compile,
        //Means this displaylist should be applied while compiling.
        CompileAndExecute,
    }
    public enum EPrimitive
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
    }
}
