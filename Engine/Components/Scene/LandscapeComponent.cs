using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Actors.Types
{
    [FileDef("Landscape Component")]
    public class LandscapeComponent : ShapeComponent
    {
        private IVec2 _dimensions = new IVec2(100, 100);
        private Vec2 _minMaxHeight = new Vec2(0.0f, 1.0f);
        private TCollisionHeightField _heightFieldShape;
        private PrimitiveManager _mesh;
        private DataSource _heightData;
        private TCollisionHeightField.EHeightValueType _heightValueType = TCollisionHeightField.EHeightValueType.Single;
        //private Matrix4 _heightOffsetTransform, _heightOffsetTransformInv;

        public TMaterial Material
        {
            get => _mesh?.Material;
            set
            {
                if (_mesh != null)
                    _mesh.Material = value;
            }
        }

        public LandscapeComponent()
        {
            RenderInfo = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null, true, true);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
               r = _rotation.GetMatrix(),
               ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();

            localTransform = t * r;
            inverseLocalTransform = ir * it;
        }

        public void GenerateHeightFieldCollision(
            VoidPtr heightDataPtr, int dataLength,
            int width, int length,
            float minHeight, float maxHeight,
            TCollisionHeightField.EHeightValueType valueType,
            TRigidBodyConstructionInfo bodyInfo)
            => GenerateHeightFieldCollision(new DataSource(heightDataPtr, dataLength, true),
                width, length, minHeight, maxHeight, valueType, bodyInfo);
        
        public void GenerateHeightFieldCollision(
            DataSource heightData,
            int width, int length,
            float minHeight, float maxHeight,
            TCollisionHeightField.EHeightValueType valueType,
            TRigidBodyConstructionInfo bodyInfo)
        {
            _dimensions = new IVec2(width, length);
            _minMaxHeight = new Vec2(minHeight, maxHeight);
            _heightValueType = valueType;

            _heightData = heightData;
            UnmanagedMemoryStream stream = _heightData.AsStream();

            _heightFieldShape = TCollisionHeightField.New(
                _dimensions.X, _dimensions.Y, stream, 1.0f, _minMaxHeight.X, _minMaxHeight.Y, 1, _heightValueType, false);

            //BoundingBox box = _heightFieldShape.GetAabb(Matrix4.Identity);
            //float offset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;
            //_heightOffsetTransform = Matrix4.CreateTranslation(0.0f, offset, 0.0f);
            //_heightOffsetTransformInv = Matrix4.CreateTranslation(0.0f, -offset, 0.0f);

            if (bodyInfo != null)
            {
                bodyInfo.Mass = 0;
                bodyInfo.LocalInertia = Vec3.Zero;
                bodyInfo.CollisionShape = _heightFieldShape;
                bodyInfo.UseMotionState = false;
                bodyInfo.SleepingEnabled = false;
                InitPhysicsShape(bodyInfo);
            }
        }
        public unsafe float GetHeight(int x, int y)
        {
            float* heightPtr = (float*)_heightData.Address;
            return heightPtr[x + y * _dimensions.X];
        }
        public unsafe void GenerateHeightFieldMesh(TMaterial material, int stride = 1)
        {
            if (_heightFieldShape == null)
                throw new InvalidOperationException();

            float offset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;

            List<VertexTriangle> list = new List<VertexTriangle>();
            Vec3 topLeft, topRight, bottomLeft, bottomRight;
            Vec2 topLeftUV, topRightUV, bottomLeftUV, bottomRightUV;
            float* heightPtr = (float*)_heightData.Address;
            int xInc = stride, yInc = stride;
            int xDim = _dimensions.X - 1, yDim = _dimensions.Y - 1;
            float halfX = xDim * 0.5f, halfY = yDim * 0.5f;
            float uInc = 1.0f / xDim, vInc = 1.0f / yDim;
            int nextX = 0, nextY = 0;
            int xTriStride = _dimensions.X / xInc * 2;
            int triIndex;
            Vertex[] vertexNormals = new Vertex[6];
            Vec3 normal;
            void AverageNormals(int x, int y)
            {
                //topleftleft
                triIndex = (x - 1) * 2 + 0 + (y - 1) * xTriStride;
                vertexNormals[0] = list.IndexInRange(triIndex) ? list[triIndex].Vertex2 : null;
                //topleftright
                triIndex = (x - 1) * 2 + 1 + (y - 1) * xTriStride;
                vertexNormals[1] = list.IndexInRange(triIndex) ? list[triIndex].Vertex1 : null;
                //toprightleft
                triIndex = (x - 0) * 2 + 0 + (y - 1) * xTriStride;
                vertexNormals[2] = list.IndexInRange(triIndex) ? list[triIndex].Vertex1 : null;

                //toprightright
                //triIndex = (x1 - 0) * 2 + 1 + (y1 - 1) * xTriStride;
                //vertexNormals[0] = list.IndexInRange(triIndex) ? list[triIndex] : null;
                //bottomleftleft
                //triIndex = (x1 - 1) * 2 + 0 + (y1 - 0) * xTriStride;
                //vertexNormals[0] = list.IndexInRange(triIndex) ? list[triIndex] : null;

                //bottomleftright
                triIndex = (x - 1) * 2 + 1 + (y - 0) * xTriStride;
                vertexNormals[3] = list.IndexInRange(triIndex) ? list[triIndex].Vertex2 : null;
                //bottomrightleft
                triIndex = (x - 0) * 2 + 0 + (y - 0) * xTriStride;
                vertexNormals[4] = list.IndexInRange(triIndex) ? list[triIndex].Vertex0 : null;
                //bottomrightright
                triIndex = (x - 0) * 2 + 1 + (y - 0) * xTriStride;
                vertexNormals[5] = list.IndexInRange(triIndex) ? list[triIndex].Vertex0 : null;

                normal = Vec3.Zero;
                vertexNormals.ForEach(vtx =>
                {
                    if (vtx != null)
                        normal += vtx._normal;
                });
                normal.Normalize();
                vertexNormals.ForEach(vtx =>
                {
                    if (vtx != null)
                        vtx._normal = normal;
                });
            }
            for (int y = 0; y < yDim; )
            {
                nextY = y + yInc;
                for (int x = 0; x < xDim; )
                {
                    nextX = x + xInc;

                    topLeft     = new Vec3(x - halfX,        heightPtr[x + y * _dimensions.X] - offset,            y - halfY);
                    topRight    = new Vec3(nextX - halfX,    heightPtr[nextX + y * _dimensions.X] - offset,        y - halfY);
                    bottomLeft  = new Vec3(x - halfX,        heightPtr[x + nextY * _dimensions.X] - offset,        nextY - halfY);
                    bottomRight = new Vec3(nextX - halfX,    heightPtr[nextX + nextY * _dimensions.X] - offset,    nextY - halfY);

                    topLeftUV       = new Vec2(x * uInc, y * vInc);
                    topRightUV      = new Vec2(nextX * uInc, y * vInc);
                    bottomLeftUV    = new Vec2(x * uInc, nextY * vInc);
                    bottomRightUV   = new Vec2(nextX * uInc, nextY * vInc);

                    Vec3 triNorm1 = Vec3.CalculateNormal(topLeft, bottomLeft, bottomRight);
                    Vec3 triNorm2 = Vec3.CalculateNormal(topLeft, bottomRight, topRight);

                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, triNorm1, topLeftUV),
                        new Vertex(bottomLeft, triNorm1, bottomLeftUV),
                        new Vertex(bottomRight, triNorm1, bottomRightUV)));
                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, triNorm2, topLeftUV),
                        new Vertex(bottomRight, triNorm2, bottomRightUV),
                        new Vertex(topRight, triNorm2, topRightUV)));
                    
                    AverageNormals(x / xInc, y / yInc);

                    x += xInc;
                }
                AverageNormals(nextX / xInc, y / yInc);

                y += yInc;
            }
            AverageNormals(nextX / xInc, nextY / yInc);

            PrimitiveData data = PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), list);
            material.RenderParams.CullMode = Culling.Back;
            _mesh = new PrimitiveManager(data, material);
        }
        
        public override Shape CullingVolume => null;
        public override void Render() => _mesh?.Render(WorldMatrix, WorldMatrix.GetRotationMatrix3());
        protected override TCollisionShape GetCollisionShape() => _heightFieldShape;

        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            Editor.EditorState.RegisterHighlightedMaterial(_mesh.Material, highlighted, OwningScene);
        }
    }
}
