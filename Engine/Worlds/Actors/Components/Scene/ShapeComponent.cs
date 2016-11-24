using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ShapeComponent<T> : GenericPrimitiveComponent where T : Shape
    {
        public T Shape
        {
            get { return (T)Primitive; }
            set { Primitive = value; }
        }

        public virtual void OnBeginOverlap()
        {

        }
        public virtual void OnEndOverlap()
        {
            
        }
    }
}
