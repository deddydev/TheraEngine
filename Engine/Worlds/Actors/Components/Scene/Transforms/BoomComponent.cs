using BulletSharp;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoomComponent : TRComponent, IRenderable
    {
        private float _maxLength = 300.0f;
        private float _currentLength = 0.0f;
        private Vec3 _currentEndPoint = Vec3.Zero;
        private bool _isRendering = false;
        private Octree.Node _renderNode;
        private Shape _cullingVolume = new Sphere(1.0f);

        public Shape CullingVolume => _cullingVolume;

        public Octree.Node RenderNode { get => _renderNode; set => _renderNode = value; }
        public bool IsRendering { get => _isRendering; set => _isRendering = value; }

        public BoomComponent() : base()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
        }
        
        protected override void RecalcLocalTransform()
        {
            Matrix4
                r = Matrix4.CreateFromRotator(_rotation),
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());
            Matrix4
                t = Matrix4.CreateTranslation(_translation),
                it = Matrix4.CreateTranslation(-_translation);
            Matrix4 
                translation = Matrix4.CreateTranslation(0.0f, 0.0f, _currentLength),
                invTranslation = Matrix4.CreateTranslation(0.0f, 0.0f, -_currentLength);

            SetLocalTransforms(t * r * translation, invTranslation * ir * it);
        }

        protected internal override void Tick(float delta)
        {
            Matrix4 parentMtx = GetParentMatrix();
            Matrix4 startTrace = parentMtx * Translation.GetTranslationMatrix();
            Vec3 start = startTrace.GetPoint();
            Vec3 end = (startTrace * Rotation.GetMatrix() *
                Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, _maxLength))).GetPoint();

            //TODO: use a sphere, not a point
            ClosestRayResultCallback result = Engine.RaycastClosest(start, end);
            Vec3 newEndPoint;
            if (result.HasHit)
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = end;
            float length = (newEndPoint - _currentEndPoint).LengthFast;
            if (length.CompareEquality(_currentLength, 0.001f))
            {
                _currentEndPoint = newEndPoint;
                _currentLength = length;
                RecalcLocalTransform();
            }
        }

        public override void OnSpawned()
        {
            Engine.Renderer.Scene.AddRenderable(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            Engine.Renderer.Scene.RemoveRenderable(this);
            base.OnDespawned();
        }

        public void Render()
        {
            Engine.Renderer.RenderLine("CameraBoom", GetParentMatrix().GetPoint(), _currentEndPoint, Color.LightYellow);
        }
    }
}
