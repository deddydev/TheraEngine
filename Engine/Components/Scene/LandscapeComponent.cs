using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
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
        private Matrix4 _heightOffsetTransform, _heightOffsetTransformInv;

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
            
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
               r = _rotation.GetMatrix(),
               ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();

            localTransform = t * r * _heightOffsetTransform;
            inverseLocalTransform = _heightOffsetTransformInv * ir * it;
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

            BoundingBox box = _heightFieldShape.GetAabb(Matrix4.Identity);
            float offset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;
            _heightOffsetTransform = Matrix4.CreateTranslation(0.0f, offset, 0.0f);
            _heightOffsetTransformInv = Matrix4.CreateTranslation(0.0f, -offset, 0.0f);

            if (bodyInfo != null)
            {
                bodyInfo.Mass = 0;
                bodyInfo.LocalInertia = Vec3.Zero;
                bodyInfo.CollisionShape = _heightFieldShape;
                bodyInfo.UseMotionState = false;
                bodyInfo.SleepingEnabled = false;
                InitPhysicsShape(bodyInfo);
            }

            GenerateHeightFieldMesh(TMaterial.CreateLitColorMaterial(Color.Orange), 0.5f);
        }
        public unsafe float GetHeight(int x, int y)
        {
            float* heightPtr = (float*)_heightData.Address;
            return heightPtr[x + y * _dimensions.X];
        }
        public unsafe void GenerateHeightFieldMesh(TMaterial material, float precision = 1.0f)
        {
            float offset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;

            List<VertexTriangle> list = new List<VertexTriangle>();
            Vec3 topLeft, topRight, bottomLeft, bottomRight;
            float xInc = 1.0f;
            float yInc = 1.0f;
            float* heightPtr = (float*)_heightData.Address;
            float halfX = (_dimensions.X - 1) * 0.5f;
            float halfY = (_dimensions.Y - 1) * 0.5f;
            for (int x = 0; x < _dimensions.X - 1; ++x)
            {
                int nextX = x + 1;
                for (int y = 0; y < _dimensions.Y - 1; ++y)
                {
                    int nextY = y + 1;

                    topLeft     = new Vec3(x * xInc - halfX,        heightPtr[x + y * _dimensions.X] - offset,            y * yInc - halfY);
                    topRight    = new Vec3(nextX * xInc - halfX,    heightPtr[nextX + y * _dimensions.X] - offset,        y * yInc - halfY);
                    bottomLeft  = new Vec3(x * xInc - halfX,        heightPtr[x + nextY * _dimensions.X] - offset,        nextY * yInc - halfY);
                    bottomRight = new Vec3(nextX * xInc - halfX,    heightPtr[nextX + nextY * _dimensions.X] - offset,    nextY * yInc - halfY);

                    Vec3 triNorm1 = -Vec3.CalculateNormal(topLeft, bottomLeft, bottomRight);
                    Vec3 triNorm2 = -Vec3.CalculateNormal(topLeft, bottomRight, topRight);

                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, triNorm1, Vec2.Zero),
                        new Vertex(bottomLeft, triNorm1, Vec2.Zero),
                        new Vertex(bottomRight, triNorm1, Vec2.Zero)));
                    list.Add(new VertexTriangle(
                        new Vertex(topLeft, triNorm2, Vec2.Zero),
                        new Vertex(bottomRight, triNorm2, Vec2.Zero),
                        new Vertex(topRight, triNorm2, Vec2.Zero)));
                }
            }

            PrimitiveData data = PrimitiveData.FromTriangleList(Culling.None, VertexShaderDesc.PosNormTex(), list);
            material.RenderParams.CullMode = Culling.None;
            _mesh = new PrimitiveManager(data, material);
        }

        public override void OnSpawned()
        {
            OwningScene.Add(this);
            base.OnSpawned();
        }

        public override void OnDespawned()
        {
            OwningScene.Remove(this);
            base.OnDespawned();
        }
        
        public override Shape CullingVolume => null;
        public override void Render() => _mesh?.Render(WorldMatrix, WorldMatrix.GetRotationMatrix3());
        protected override TCollisionShape GetCollisionShape() => _heightFieldShape;
    }
}
