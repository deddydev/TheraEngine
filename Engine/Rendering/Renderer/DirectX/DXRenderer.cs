using System;
using CustomEngine.Rendering.DirectX;
using System.Collections.Generic;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;

namespace CustomEngine.Rendering.DirectX
{
    public unsafe class DXRenderer : AbstractRenderer
    {
        public static DXRenderer Instance;
        public DXRenderer()
        {

        }

        public override RenderLibrary RenderLibrary { get { return RenderLibrary.Direct3D11; } }

        private class DLCompileInfo
        {
            public int _id;
            public bool _executeAfterCompiling;
            public bool _temporary;

            public DLCompileInfo(int id, bool execute, bool temporary)
            {
                _id = id;
                _executeAfterCompiling = execute;
                _temporary = temporary;
            }
        }

        private Dictionary<int, DXDisplayList> _displayLists = new Dictionary<int, DXDisplayList>();
        private Stack<DLCompileInfo> _compilingDisplayLists = new Stack<DLCompileInfo>();
        public DXDisplayList CurrentList { get { return _compilingDisplayLists.Count > 0 && _displayLists.Count > 0 ? _displayLists[_compilingDisplayLists.Peek()._id] : null; } }

        public override void Clear(BufferClear clearBufferMask)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxSolid(System.Vec3 min, System.Vec3 max)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxWireframe(System.Vec3 min, System.Vec3 max)
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

        public override void DrawSphereSolid(System.Single radius)
        {
            throw new NotImplementedException();
        }

        public override void DrawSphereWireframe(System.Single radius)
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

        #region Conversion
        private SlimDX.Matrix DXMat4(System.Matrix4 matrix4)
        {
            //OpenTK.Matrix4 m = new OpenTK.Matrix4();
            //float* sPtr = (float*)&m;
            //float* dPtr = (float*)&matrix4;
            //for (int i = 0; i < 16; ++i)
            //    *dPtr++ = *sPtr++;
            //return m;
            return *(SlimDX.Matrix*)&matrix4;
        }
        private SlimDX.Vector4 DXVec4(System.Vec4 vec4)
        {
            return *(SlimDX.Vector4*)&vec4;
        }
        private SlimDX.Vector3 DXVec3(System.Vec3 vec3)
        {
            return *(SlimDX.Vector3*)&vec3;
        }
        private SlimDX.Vector2 DXVec2(System.Vec2 vec2)
        {
            return *(SlimDX.Vector2*)&vec2;
        }
        private SlimDX.Quaternion DXQuat(System.Quaternion quat)
        {
            return *(SlimDX.Quaternion*)&quat;
        }
        #endregion

        public override float GetDepth(float x, float y)
        {
            throw new NotImplementedException();
        }

        #region Display Lists
        public override int CreateDisplayList()
        {
            int id = _displayLists.Count;
            _displayLists.Add(id, new DXDisplayList());
            return id;
        }
        public override void BeginDisplayList(int id, DisplayListMode mode)
        {
            _compilingDisplayLists.Push(new DLCompileInfo(id, mode == DisplayListMode.CompileAndExecute ? true : false, false));
        }
        private void BeginTempDisplayList(int id)
        {
            _compilingDisplayLists.Push(new DLCompileInfo(id, false, true));
        }
        public override void EndDisplayList()
        {
            if (_compilingDisplayLists.Count == 0)
                return;
            DLCompileInfo info = _compilingDisplayLists.Pop();
            if (info._executeAfterCompiling || info._temporary && info._id < _displayLists.Count)
                _displayLists[info._id].Render();
        }
        public override void CallDisplayList(int id)
        {
            if (_displayLists.ContainsKey(id))
                _displayLists[id].Render();
        }
        public override void DeleteDisplayList(int id)
        {
            if (_displayLists.ContainsKey(id))
                _displayLists.Remove(id);
        }
        #endregion

        public override void Begin(EPrimitive type)
        {
            if (CurrentList == null)
                BeginTempDisplayList(CreateDisplayList());
        }
        public override void Vertex3(System.Vec3 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void Vertex2(System.Vec2 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void Normal3(System.Vec3 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void TexCoord2(System.Vec2 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void MultiTexCoord2(int unit, System.Vec2 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void Color4(ColorF4 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void Color3(ColorF3 value)
        {
            if (CurrentList == null)
                return;
        }
        public override void End()
        {
            if (_compilingDisplayLists.Count > 0 && _compilingDisplayLists.Peek()._temporary)
                EndDisplayList();
        }

        public override int GenerateShader(string source)
        {
            throw new NotImplementedException();
        }

        public override int GenerateProgram(params int[] shaderHandles)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable4Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(string name, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        protected override void SetRenderArea(Rectangle region)
        {
            throw new NotImplementedException();
        }

        public override void CropRenderArea(Rectangle region)
        {
            throw new NotImplementedException();
        }

        public override int GenObject(GenType type)
        {
            throw new NotImplementedException();
        }

        public override void DeleteObject(GenType type, int bindingId)
        {
            throw new NotImplementedException();
        }

        public override void SetShaderMode(ShaderMode type)
        {
            throw new NotImplementedException();
        }

        public override int[] GenObjects(GenType type, int count)
        {
            throw new NotImplementedException();
        }

        public override void DeleteObjects(GenType type, int[] bindingIds)
        {
            throw new NotImplementedException();
        }
    }
}
