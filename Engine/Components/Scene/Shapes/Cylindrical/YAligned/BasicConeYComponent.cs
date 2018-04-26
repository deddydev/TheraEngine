using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Shapes
{
    public class BasicConeYComponent : ShapeComponent
    {
        public float _radius, _height;

        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public override Shape CullingVolume
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vec3 GetTipPosition()
        {
            return Vec3.TransformPosition(new Vec3(0.0f, _height / 2.0f, 0.0f), WorldMatrix);
        }
        public Vec3 GetBasePosition()
        {
            return Vec3.TransformPosition(new Vec3(0.0f, -_height / 2.0f, 0.0f), WorldMatrix);
        }
        
        public BasicConeYComponent(float radius, float height, TRigidBodyConstructionInfo info) : base()
        {
            _radius = radius;
            _height = height;
            InitPhysicsShape(info);
        }
        public override void Render()
        {
            //Engine.Renderer.RenderCone(_radius, _height, WorldMatrix, false);
        }

        protected override TCollisionShape GetCollisionShape()
        {
            throw new NotImplementedException();
        }

        public override void AddRenderables(RenderPasses passes, Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}
