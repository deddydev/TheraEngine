using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
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
            RenderInfo = new RenderInfo3D(ERenderPass.OpaqueDeferredLit);
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
            Vertex[] vertexNormals = new Vertex[6];
            int triCount = 0;
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
                    
                    Vec3 topLeftNorm = triNorm1 + triNorm2;
                    Vec3 bottomLeftNorm = triNorm1;
                    Vec3 bottomRightNorm = triNorm1 + triNorm2;
                    Vec3 topRightNorm = triNorm1;

                    if (triCount - 2 >= 0)
                    {
                        VertexTriangle left1 = list[triCount - 2];
                        VertexTriangle left2 = list[triCount - 1];
                        
                        Vec3 prevBottomRightNorm = left1.Vertex2._normal + left2.Vertex1._normal;
                        Vec3 prevTopRightNorm = left2.Vertex2._normal;

                        topLeftNorm += prevTopRightNorm;
                        bottomLeftNorm += prevBottomRightNorm;
                    }
                    if (triCount - xTriStride >= 0)
                    {
                        VertexTriangle top1 = list[triCount - xTriStride];
                        VertexTriangle top2 = list[triCount - xTriStride + 1];
                        
                        Vec3 upperBottomLeftNorm = top1.Vertex1._normal;
                        Vec3 upperBottomRightNorm = top1.Vertex2._normal + top2.Vertex1._normal;

                        topLeftNorm += upperBottomLeftNorm;
                        topRightNorm += upperBottomRightNorm;
                    }
                    if (triCount - xTriStride - 2 >= 0)
                    {
                        VertexTriangle topLeft1 = list[triCount - xTriStride - 2];
                        VertexTriangle topLeft2 = list[triCount - xTriStride - 1];
                        
                        Vec3 upperLeftBottomRightNorm = topLeft1.Vertex2._normal + topLeft2.Vertex1._normal;

                        topLeftNorm += upperLeftBottomRightNorm;
                    }
                    if (triCount - xTriStride + 2 >= 0)
                    {
                        VertexTriangle topRight1 = list[triCount - xTriStride + 2];
                        //VertexTriangle topRight2 = list[triCount - xTriStride + 3];
                        
                        Vec3 upperRightBottomLeftNorm = topRight1.Vertex1._normal;

                        topRightNorm += upperRightBottomLeftNorm;
                    }

                    topLeftNorm = topLeftNorm.Normalized();
                    bottomLeftNorm = bottomLeftNorm.Normalized();
                    bottomRightNorm = bottomRightNorm.Normalized();
                    topRightNorm = topRightNorm.Normalized();

                    //Update previous normals
                    if (triCount - 2 >= 0)
                    {
                        VertexTriangle left1 = list[triCount - 2];
                        VertexTriangle left2 = list[triCount - 1];
                        
                        left2.Vertex2._normal = topLeftNorm;
                        left1.Vertex2._normal = left2.Vertex1._normal = bottomLeftNorm;
                    }
                    if (triCount - xTriStride >= 0)
                    {
                        VertexTriangle top1 = list[triCount - xTriStride];
                        VertexTriangle top2 = list[triCount - xTriStride + 1];
                        
                        top1.Vertex1._normal = topLeftNorm;
                        top1.Vertex2._normal = top2.Vertex1._normal = topRightNorm;
                    }
                    if (triCount - xTriStride - 2 >= 0)
                    {
                        VertexTriangle topLeft1 = list[triCount - xTriStride - 2];
                        VertexTriangle topLeft2 = list[triCount - xTriStride - 1];

                        topLeft1.Vertex2._normal = topLeft2.Vertex1._normal = topLeftNorm;
                    }

                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, topLeftNorm, topLeftUV),
                        new Vertex(bottomLeft, bottomLeftNorm, bottomLeftUV),
                        new Vertex(bottomRight, bottomRightNorm, bottomRightUV)));
                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, topLeftNorm, topLeftUV),
                        new Vertex(bottomRight, bottomRightNorm, bottomRightUV),
                        new Vertex(topRight, topRightNorm, topRightUV)));
                    triCount += 2;
                    
                    x += xInc;
                }

                y += yInc;
            }

            PrimitiveData data = PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), list);
            material.RenderParams.CullMode = ECulling.Back;
            _mesh = new PrimitiveManager(data, material);
        }
        
        public override Shape CullingVolume => null;
        protected override TCollisionShape GetCollisionShape() => _heightFieldShape;

        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            Editor.EditorState.RegisterHighlightedMaterial(_mesh.Material, highlighted, OwningScene);
        }

        private RenderCommandMesh3D _rc = new RenderCommandMesh3D();
        public override void AddRenderables(RenderPasses passes, Camera camera)
        {
            _rc.Primitives = _mesh;
            _rc.WorldMatrix = WorldMatrix;
            _rc.NormalMatrix = WorldMatrix.Transposed().Inverted().GetRotationMatrix3();
            passes.Add(_rc, RenderInfo.RenderPass);
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }
    }
}
