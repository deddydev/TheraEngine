using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace TheraEngine.Worlds.Actors
{
    public class PositionLagComponent : SceneComponent, I3DRenderable
    {
        public PositionLagComponent() : base() { }
        public PositionLagComponent(float interpSpeed) : base() { _interpSpeed = interpSpeed; }

        [Serialize("InterpSpeed")]
        private float _interpSpeed = 20.0f;
        [Serialize("MaxLagDistance")]
        private float _maxLagDistance = 2.0f;

        private float _delta;
        private Vec3 _currentPoint;
        private Vec3 _destPoint;
        private Vec3 _interpPoint;
        private float _laggingDistance;

        [Browsable(false)]
        public bool HasTransparency => false;
        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [Browsable(false)]
        public bool IsRendering { get; set; }

        [Category("Position Lag Component")]
        public float InterpSpeed { get => _interpSpeed; set => _interpSpeed = value; }
        [Category("Position Lag Component")]
        public float MaxLagDistance { get => _maxLagDistance; set => _maxLagDistance = value; }

        public float LaggingDistance => _laggingDistance;

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            //    //Matrix4 worldPosMtx = Matrix4.CreateTranslation(_interpPoint);
            //    //Matrix4 invWorldPosMtx = Matrix4.CreateTranslation(-_interpPoint);
            //    //localTransform = worldPosMtx * GetInverseParentMatrix();
            //    //inverseLocalTransform = GetParentMatrix() * invWorldPosMtx;
            localTransform = Matrix4.Identity;
            inverseLocalTransform = Matrix4.Identity;
        }
        internal override void RecalcGlobalTransform()
        {
            _previousWorldTransform = _worldTransform;
            _previousInverseWorldTransform = _inverseWorldTransform;

            _worldTransform = _interpPoint.AsTranslationMatrix();
            _inverseWorldTransform = (-_interpPoint).AsTranslationMatrix();

            OnWorldTransformChanged();
        }
        protected internal void Tick(float delta)
        {
            _delta = delta;
            _currentPoint = _worldTransform.GetPoint();
            _destPoint = GetParentMatrix().GetPoint();
            _laggingDistance = _destPoint.DistanceToFast(_currentPoint);
            if (_laggingDistance > _maxLagDistance)
                _interpPoint = CustomMath.InterpLinearTo(_destPoint, _currentPoint, _maxLagDistance / _laggingDistance);
            else
                _interpPoint = CustomMath.InterpLinearTo(_currentPoint, _destPoint, _delta, _interpSpeed);

            //Debug.WriteLine(_currentPoint.DistanceTo(_destPoint));
            //RecalcLocalTransform();
        }
        public override void OnSpawned()
        {
            _currentPoint = _worldTransform.GetPoint();
            //Engine.Scene.Add(this);
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Logic, Tick);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            //Engine.Scene.Remove(this);
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Logic, Tick);
            base.OnDespawned();
        }

        public void Render()
        {
            //Engine.Renderer.RenderSphere(_currentPoint, 10.0f, false, Color.Magenta);
            //Engine.Renderer.RenderSphere(_interpPoint, 10.0f, false, Color.Pink);
            //Engine.Renderer.RenderSphere(_destPoint, 10.0f, false, Color.White);
        }
    }
}
