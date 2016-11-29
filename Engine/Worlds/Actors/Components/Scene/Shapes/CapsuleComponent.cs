using BulletSharp;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CapsuleComponent : ShapeComponent
    {
        public CapsuleShape CapsuleCollision
        {
            get { return (CapsuleShape)_collisionShape; }
            set { _collisionShape = value; }
        }
        public Capsule Capsule
        {
            get { return (Capsule)Primitive; }
            set { Shape = value; }
        }

        protected override void UpdateCollisionShape()
        {
            CapsuleShape col = CapsuleCollision;
            Capsule cap = Capsule;

            //col.HalfHeight = cap.HalfHeight;
            //col.Radius = cap.Radius;
            //col.UpAxis = _worldTransform.GetRotationMatrix() * Vec3.Up;
        }
    }
}
