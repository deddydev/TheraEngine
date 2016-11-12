using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class ShapeComponent<T> : PrimitiveComponent<T> where T : Shape
    {
        public T Shape
        {
            get { return _primitive; }
            set { _primitive = value; }
        }

        public virtual void OnBeginOverlap()
        {

        }
        public virtual void OnEndOverlap()
        {
            
        }
    }
}
