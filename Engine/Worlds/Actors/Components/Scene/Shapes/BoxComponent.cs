using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoxComponent : ShapeComponent
    {
        public Box Box
        {
            get { return (Box)Primitive; }
            set { Shape = value; }
        }

        protected override void UpdateCollisionShape()
        {
            throw new NotImplementedException();
        }
    }
}
