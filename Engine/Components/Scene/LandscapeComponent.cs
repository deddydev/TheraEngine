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

            List<VertexTriangle> list = new List<VertexTriangle>();
            Vertex[] vertexNormals = new Vertex[6];
            
            int xInc = stride, yInc = stride;
            int xDim = _dimensions.X - 1, yDim = _dimensions.Y - 1;
            float uInc = 1.0f / xDim, vInc = 1.0f / yDim;
            int nextX, nextY, nextX2, nextY2, prevX, prevY;
            int xTriStride = _dimensions.X / xInc * 2;
            int triCount = 0;

            float* heightPtr = (float*)_heightData.Address;
            float halfX = xDim * 0.5f, halfY = yDim * 0.5f;
            float yOffset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;
            Vec3 GetHeight(int x, int y) => new Vec3(x - halfX, heightPtr[x + y * _dimensions.X] - yOffset, y - halfY);
            for (int thisY = 0; thisY < yDim; thisY += yInc)
            {
                nextY = thisY + yInc;
                nextY2 = nextY + yInc;
                prevY = thisY - yInc;

                for (int thisX = 0; thisX < xDim; thisX += xInc)
                {
                    nextX = thisX + xInc;
                    prevX = thisX - xInc;
                    nextX2 = nextX + xInc;

                    Vec3 ptl = GetHeight(thisX, thisY);
                    Vec3 ptr = GetHeight(nextX, thisY);
                    Vec3 pbl = GetHeight(thisX, nextY);
                    Vec3 pbr = GetHeight(nextX, nextY);

                    Vec3 p01 = GetHeight(prevX, prevY);
                    Vec3 p02 = GetHeight(thisX, prevY);
                    Vec3 p03 = GetHeight(nextX, prevY);
                    Vec3 p04 = GetHeight(nextX2, prevY);
                    Vec3 p05 = GetHeight(nextX2, thisY);
                    Vec3 p06 = GetHeight(nextX2, nextY);
                    Vec3 p07 = GetHeight(nextX2, nextY2);
                    Vec3 p08 = GetHeight(nextX, nextY2);
                    Vec3 p09 = GetHeight(thisX, nextY2);
                    Vec3 p10 = GetHeight(prevX, nextY2);
                    Vec3 p11 = GetHeight(prevX, nextY);
                    Vec3 p12 = GetHeight(prevX, thisY);

                    /*
                    1________4
                    |\ |\ |\ |
                    |_\|_\|_\|
                    |\ |\ |\ |
                    |_\|_\|_\|
                    |\ |\ |\ |
                    |_\|_\|_\|
                    10       7
                    */

                    //top row triangle normals left to right
                    Vec3 n01 = Vec3.CalculateNormal(p01, p12, ptl);
                    Vec3 n02 = Vec3.CalculateNormal(p01, ptl, p02);
                    Vec3 n03 = Vec3.CalculateNormal(p02, ptl, ptr);
                    Vec3 n04 = Vec3.CalculateNormal(p02, ptr, p03);
                    Vec3 n05 = Vec3.CalculateNormal(p03, ptr, ptl);
                    Vec3 n06 = Vec3.CalculateNormal(p03, ptl, p04);

                    //middle row triangle normals left to right
                    Vec3 n07 = Vec3.CalculateNormal(p12, p11, pbl);
                    Vec3 n08 = Vec3.CalculateNormal(p12, ptl, p02);
                    Vec3 n09 = Vec3.CalculateNormal(ptl, pbl, pbr);
                    Vec3 n10 = Vec3.CalculateNormal(ptl, pbr, ptr);
                    Vec3 n11 = Vec3.CalculateNormal(ptr, p12, ptl);
                    Vec3 n12 = Vec3.CalculateNormal(ptr, ptl, p02);

                    //bottom row triangle normals left to right
                    Vec3 n13 = Vec3.CalculateNormal(p01, p12, ptl);
                    Vec3 n14 = Vec3.CalculateNormal(p01, ptl, p02);
                    Vec3 n15 = Vec3.CalculateNormal(p01, p12, ptl);
                    Vec3 n16 = Vec3.CalculateNormal(p01, ptl, p02);
                    Vec3 n17 = Vec3.CalculateNormal(p01, p12, ptl);
                    Vec3 n18 = Vec3.CalculateNormal(p01, ptl, p02);

                    Vec3 topLeftNorm = (n09 + n10).Normalized();
                    Vec3 bottomLeftNorm = n09;
                    Vec3 bottomRightNorm = (n09 + n10).Normalized();
                    Vec3 topRightNorm = n10;

                    Vec2 topLeftUV      = new Vec2(thisX * uInc, thisY * vInc);
                    Vec2 topRightUV     = new Vec2(nextX * uInc, thisY * vInc);
                    Vec2 bottomLeftUV   = new Vec2(thisX * uInc, nextY * vInc);
                    Vec2 bottomRightUV  = new Vec2(nextX * uInc, nextY * vInc);

                    list.Add(new VertexTriangle(
                        new Vertex(ptl, topLeftNorm, topLeftUV),
                        new Vertex(pbl, bottomLeftNorm, bottomLeftUV),
                        new Vertex(pbr, bottomRightNorm, bottomRightUV)));
                    list.Add(new VertexTriangle(
                        new Vertex(ptl, topLeftNorm, topLeftUV),
                        new Vertex(pbr, bottomRightNorm, bottomRightUV),
                        new Vertex(ptr, topRightNorm, topRightUV)));
                    triCount += 2;
                }
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
            _rc.Mesh = _mesh;
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
