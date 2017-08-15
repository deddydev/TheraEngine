﻿using BulletSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Worlds.Actors
{
    public delegate void LengthChange(float newLength);
    public class BoomComponent : RTComponent, I3DRenderable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPassType3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;

        public event LengthChange CurrentDistanceChanged;

        private SphereShape _traceShape = new SphereShape(0.3f);
        private float _currentLength = 0.0f;
        private Vec3 _startPoint = Vec3.Zero;
        private CollisionObject _ignoreCast = null;

        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        [DefaultValue(300.0f)]
        [Serialize]
        public float MaxLength { get; set; } = 300.0f;

        public CollisionObject IgnoreCast { get; set; } = null;

        public BoomComponent() : base() { }
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.GetInverseMatrix();
            Matrix4
                t = Matrix4.CreateTranslation(_translation),
                it = Matrix4.CreateTranslation(-_translation);
            Matrix4 
                translation = Matrix4.CreateTranslation(0.0f, 0.0f, _currentLength),
                invTranslation = Matrix4.CreateTranslation(0.0f, 0.0f, -_currentLength);

            localTransform = r * t * translation;
            inverseLocalTransform = invTranslation * it * ir;
        }

        private void Tick(float delta)
        {
            Matrix4 startMatrix = GetParentMatrix() * Rotation.GetMatrix() * Translation.GetTranslationMatrix();
            _startPoint = startMatrix.GetPoint();
            Matrix4 endMatrix = startMatrix * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, MaxLength));
            Vec3 testEnd = endMatrix.GetPoint();

            ClosestNotMeConvexResultCallback result = new ClosestNotMeConvexResultCallback(IgnoreCast)
            {
                CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
                CollisionFilterMask = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
            };

            Engine.ShapeCastClosest(_traceShape, startMatrix, endMatrix, result);
            Vec3 newEndPoint;
            if (result.HasHit)
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = testEnd;
            float newLength = (newEndPoint - _startPoint).LengthFast;
            if (!newLength.EqualTo(_currentLength, 0.001f))
            {
                if (newLength < _currentLength)
                    _currentLength = newLength; //Moving closer to the character, meaning something is obscuring the view. Need to jump to the right position.
                else //Nothing is now obscuring the view, so we can lerp out quickly to give the appearance of a clean camera zoom out
                    _currentLength = CustomMath.InterpLinearTo(_currentLength, newLength, delta, 15.0f);

                RecalcLocalTransform();
                CurrentDistanceChanged?.Invoke(_currentLength);
            }
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            //Engine.Scene.AddRenderable(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            //Engine.Scene.RemoveRenderable(this);
            base.OnDespawned();
        }

        public void Render()
        {
            //Engine.Renderer.RenderLine(_startPoint, _currentEndPoint, Color.LightYellow);
        }
    }
}
