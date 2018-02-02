﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Actors.Types
{
    [FileDef("Landscape Component")]
    public class LandscapeComponent : ShapeComponent
    {
        private IVec2 _dimensions = new IVec2(100, 100);
        private Vec2 _minMaxHeight = new Vec2(0.0f, 1.0f);
        private TCollisionHeightField _heightFieldCollision;
        private PrimitiveManager _mesh;
        private DataSource _heightData;
        private TCollisionHeightField.EHeightValueType _heightValueType = TCollisionHeightField.EHeightValueType.Single;
        private Matrix4 _heightOffsetTransform, _heightOffsetTransformInv;

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

        public void GenerateHeightFieldCollision(DataSource heightData, int width, int height, float minHeight, float maxHeight, TCollisionHeightField.EHeightValueType valueType)
        {
            _dimensions = new IVec2(width, height);
            _minMaxHeight = new Vec2(minHeight, maxHeight);
            _heightValueType = valueType;

            _heightData = new DataSource(heightData.Address, heightData.Length, true);
            UnmanagedMemoryStream stream = _heightData.AsStream();

            _heightFieldCollision = TCollisionHeightField.New(
                _dimensions.X, _dimensions.Y, stream, 1.0f, _minMaxHeight.X, _minMaxHeight.Y, 1, _heightValueType, false);

            float offset = (_minMaxHeight.X + _minMaxHeight.Y) * 0.5f/* * _heightFieldCollision.LocalScaling.Y*/;
            _heightOffsetTransform = Matrix4.CreateTranslation(0.0f, offset, 0.0f);
            _heightOffsetTransformInv = Matrix4.CreateTranslation(0.0f, -offset, 0.0f);

            InitPhysics(new TRigidBodyConstructionInfo()
            {
                Mass = 0,
                LocalInertia = Vec3.Zero,
            });
        }
        public void GenerateHeightFieldMesh()
        {

        }

        public override void OnSpawned()
        {
            base.OnSpawned();
        }

        public override void OnDespawned()
        {
            base.OnDespawned();
        }
        
        public override Shape CullingVolume => null;
        public override void Render() => _mesh?.Render(WorldMatrix, WorldMatrix.GetRotationMatrix3());
        protected override TCollisionShape GetCollisionShape() => _heightFieldCollision;
    }
}
