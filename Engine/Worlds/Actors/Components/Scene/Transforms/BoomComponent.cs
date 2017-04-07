using BulletSharp;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;
using System.Drawing;

namespace CustomEngine.Worlds.Actors
{
    public class BoomComponent : RTComponent, IRenderable
    {
        private float _maxLength = 300.0f;
        private float _currentLength = 0.0f;
        private Vec3 _currentEndPoint = Vec3.Zero;
        private bool _isRendering = false;
        private Octree.Node _renderNode;
        private Shape _cullingVolume = new Sphere(1.0f);
        private Vec3 _startPoint = Vec3.Zero;

        public Shape CullingVolume => _cullingVolume;

        public Octree.Node RenderNode { get => _renderNode; set => _renderNode = value; }
        public bool IsRendering { get => _isRendering; set => _isRendering = value; }
        public float MaxLength { get => _maxLength; set => _maxLength = value; }

        public BoomComponent() : base()
        {

        }
        
        protected override void RecalcLocalTransform()
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.GetInverseMatrix();
            Matrix4
                t = Matrix4.CreateTranslation(_translation),
                it = Matrix4.CreateTranslation(-_translation);
            Matrix4 
                translation = Matrix4.CreateTranslation(0.0f, 0.0f, _maxLength),
                invTranslation = Matrix4.CreateTranslation(0.0f, 0.0f, -_maxLength);

            SetLocalTransforms(r * t *  translation, invTranslation * it * ir);
        }

        protected internal override void Tick(float delta)
        {
            Matrix4 startMatrix = GetParentMatrix() * Rotation.GetMatrix() * Translation.GetTranslationMatrix();
            _startPoint = startMatrix.GetPoint();
            Vec3 testEnd = (startMatrix * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, _maxLength))).GetPoint();

            //TODO: use a sphere, not a point
            CustomClosestRayResultCallback result = Engine.RaycastClosest(_startPoint, testEnd, Rendering.CustomCollisionGroup.All, Rendering.CustomCollisionGroup.Characters);
            Vec3 newEndPoint;
            if (result.HasHit)
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = testEnd;
            float length = (newEndPoint - _currentEndPoint).LengthFast;
            if (!length.EqualTo(_currentLength, 0.001f))
            {
                _currentEndPoint = newEndPoint;
                _currentLength = length;
                RecalcLocalTransform();
            }
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
            Engine.Renderer.Scene.AddRenderable(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick();
            Engine.Renderer.Scene.RemoveRenderable(this);
            base.OnDespawned();
        }

        public void Render()
        {
            Engine.Renderer.RenderLine("CameraBoom", _startPoint, _currentEndPoint, Color.LightYellow);
        }
    }
}
