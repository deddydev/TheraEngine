using System;
using BulletSharp;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors
{
    public class BasicConeYComponent : ShapeComponent
    {
        public float _radius, _height;

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public float Height
        {
            get { return _height; }
            set { _height = value; }
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
        
        public BasicConeYComponent(float radius, float height, PhysicsDriverInfo info) : base()
        {
            _radius = radius;
            _height = height;
            InitPhysics(info);
        }
        public override void Render()
        {
            //Engine.Renderer.RenderCone(_radius, _height, WorldMatrix, false);
        }

        protected override CollisionShape GetCollisionShape()
        {
            throw new NotImplementedException();
        }
    }
}
