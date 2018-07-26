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
            int heightIndex = x + y * _dimensions.X;
            if (heightIndex < 0 || heightIndex >= _heightData.Length / sizeof(float))
                throw new IndexOutOfRangeException();
            return heightPtr[heightIndex];
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
            int nextX, nextY, prevX, prevY;
            int xTriStride = _dimensions.X / xInc * 2;
            int triCount = 0;

            float* heightPtr = (float*)_heightData.Address;
            float halfX = xDim * 0.5f, halfY = yDim * 0.5f;
            float yOffset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;
            Vec3 GetPosition(int x, int y) => new Vec3(x - halfX, GetHeight(x, y) - yOffset, y - halfY);
            bool GetQuad(int x, int y, out Vec3 topLeft, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight)
            {
                if (x < 0 || y < 0 || x + xInc >= _dimensions.X || y + yInc >= _dimensions.Y)
                {
                    topLeft = Vec3.Zero;
                    topRight = Vec3.Zero;
                    bottomLeft = Vec3.Zero;
                    bottomRight = Vec3.Zero;
                    return false;
                }
                else
                {
                    topLeft = GetPosition(x, y);
                    topRight = GetPosition(x + xInc, y);
                    bottomLeft = GetPosition(x, y + yInc);
                    bottomRight = GetPosition(x + xInc, y + yInc);
                    return true;
                }
            }
            Vec3? GetNormal(int quadX, int quadY, bool tri2)
            {
                if (!GetQuad(quadX, quadY, out Vec3 topLeft, out Vec3 bottomLeft, out Vec3 bottomRight, out Vec3 topRight))
                    return null;
                return tri2 ? 
                    Vec3.CalculateNormal(topLeft, bottomRight, topRight) : 
                    Vec3.CalculateNormal(topLeft, bottomLeft, bottomRight);
            }
            Vec3 GetSmoothedNormal(int x, int y)
            {
                int xPrev = x - xInc;
                int yPrev = y - yInc;

                /*
                 ________
                |\2|\ |\ |
                |1\|_\|_\|
                |\ |\ |\ |
                |_\|_\|_\|
                |\ |\ |\ |
                |_\|_\|_\|

                */

                Vec3? leftQuadTri2 = GetNormal(xPrev, y, true);
                Vec3? topQuadTri1 = GetNormal(x, yPrev, false);

                Vec3? thisQuadTri1 = GetNormal(x, y, false);
                Vec3? thisQuadTri2 = GetNormal(x, y, true);

                Vec3? topLeftQuadTri1 = GetNormal(xPrev, yPrev, false);
                Vec3? topLeftQuadTri2 = GetNormal(xPrev, yPrev, true);

                Vec3 normal = Vec3.Zero;

                if (thisQuadTri1.HasValue)
                    normal += thisQuadTri1.Value;
                if (thisQuadTri2.HasValue)
                    normal += thisQuadTri2.Value;
                if (leftQuadTri2.HasValue)
                    normal += leftQuadTri2.Value;
                if (topQuadTri1.HasValue)
                    normal += topQuadTri1.Value;
                if (topLeftQuadTri1.HasValue)
                    normal += topLeftQuadTri1.Value;
                if (topLeftQuadTri2.HasValue)
                    normal += topLeftQuadTri2.Value;

                return normal.Normalized();
            }
            for (int thisY = 0; thisY < yDim; thisY += yInc)
            {
                nextY = thisY + yInc;
                prevY = thisY - yInc;

                for (int thisX = 0; thisX < xDim; thisX += xInc)
                {
                    nextX = thisX + xInc;
                    prevX = thisX - xInc;

                    Vec3 topLeftPos = GetPosition(thisX, thisY);
                    Vec3 topRightPos = GetPosition(nextX, thisY);
                    Vec3 bottomLeftPos = GetPosition(thisX, nextY);
                    Vec3 bottomRightPos = GetPosition(nextX, nextY);

                    Vec3 topLeftNorm = GetSmoothedNormal(thisX, thisY);
                    Vec3 bottomLeftNorm = GetSmoothedNormal(thisX, nextY);
                    Vec3 bottomRightNorm = GetSmoothedNormal(nextX, nextY);
                    Vec3 topRightNorm = GetSmoothedNormal(nextX, thisY);

                    Vec2 topLeftUV      = new Vec2(thisX * uInc, thisY * vInc);
                    Vec2 topRightUV     = new Vec2(nextX * uInc, thisY * vInc);
                    Vec2 bottomLeftUV   = new Vec2(thisX * uInc, nextY * vInc);
                    Vec2 bottomRightUV  = new Vec2(nextX * uInc, nextY * vInc);

                    list.Add(new VertexTriangle(
                        new Vertex(topLeftPos, topLeftNorm, topLeftUV),
                        new Vertex(bottomLeftPos, bottomLeftNorm, bottomLeftUV),
                        new Vertex(bottomRightPos, bottomRightNorm, bottomRightUV)));
                    list.Add(new VertexTriangle(
                        new Vertex(topLeftPos, topLeftNorm, topLeftUV),
                        new Vertex(bottomRightPos, bottomRightNorm, bottomRightUV),
                        new Vertex(topRightPos, topRightNorm, topRightUV)));
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
