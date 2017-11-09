using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    [FileClass("clag", "Position Lag Component")]
    public class PositionLagComponent : SceneComponent, I3DRenderable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPassType3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;

        public PositionLagComponent() : this(20.0f, 2.0f) { }
        public PositionLagComponent(float interpSpeed) : this(interpSpeed, 2.0f) { }
        public PositionLagComponent(float interpSpeed, float maxLagDistance) : base()
        {
            _interpSpeed = interpSpeed;
            _maxLagDistance = maxLagDistance;
        }

        private float _interpSpeed;
        private float _maxLagDistance;

        private float _delta;
        private Vec3 _currentPoint;
        private Vec3 _destPoint;
        private Vec3 _interpPoint;
        private float _laggingDistance;
        
        [Browsable(false)]
        public Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [Browsable(false)]
        public ERenderPassType3D RenderPass => ERenderPassType3D.OpaqueForward;
        [Browsable(false)]
        public float RenderOrder => 0.0f;

        [TSerialize]
        [Category("Position Lag Component")]
        public float InterpSpeed { get => _interpSpeed; set => _interpSpeed = value; }
        [TSerialize]
        [Category("Position Lag Component")]
        public float MaxLagDistance { get => _maxLagDistance; set => _maxLagDistance = value; }
        [Browsable(false)]
        public float LaggingDistance => _laggingDistance;

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            _currentPoint -= newOrigin;
            _destPoint -= newOrigin;
            _interpPoint -= newOrigin;
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
            //if (_laggingDistance > _maxLagDistance)
            //    _interpPoint = CustomMath.InterpLinearTo(_destPoint, _currentPoint, _maxLagDistance / _laggingDistance);
            //else
                _interpPoint = CustomMath.InterpLinearTo(_currentPoint, _destPoint, _delta, _interpSpeed);

            //Engine.DebugPrint(_currentPoint.DistanceTo(_destPoint));
            RecalcGlobalTransform();
        }
        public override void OnSpawned()
        {
            _currentPoint = _worldTransform.GetPoint();
            //Engine.Scene.Add(this);
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, Tick, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            //Engine.Scene.Remove(this);
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, Tick, Input.Devices.InputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }

        public void Render()
        {
            //Engine.Renderer.RenderSphere(_currentPoint, 10.0f, false, Color.Magenta);
            //Engine.Renderer.RenderSphere(_interpPoint, 10.0f, false, Color.Pink);
            //Engine.Renderer.RenderSphere(_destPoint, 10.0f, false, Color.White);
        }

        public override void HandleTranslation(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        public override void HandleScale(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        public override void HandleRotation(Quat delta)
        {
            throw new NotImplementedException();
        }
    }
}
