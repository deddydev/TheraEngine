using BulletSharp;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class ShapeComponent : PrimitiveComponent
    {
        protected CollisionShape _collisionShape;

        public Shape Shape
        {
            get { return (Shape)Primitive; }
            set
            {
                if (Primitive != null)
                {
                    _collisionShape = null;
                    Shape.AttributeChanged -= UpdateCollisionShape;
                }
                Primitive = value;
                if (value != null)
                {
                    _collisionShape = value.GetCollisionShape();
                    value.AttributeChanged += UpdateCollisionShape;
                }
            }
        }
        protected abstract void UpdateCollisionShape();
        public virtual void OnBeginOverlap(Actor overlappedActor)
        {

        }
        public virtual void OnEndOverlap(Actor overlappedActor)
        {
            
        }
    }
}
