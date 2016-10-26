namespace CustomEngine.Worlds.Actors.Components
{
    public class ShapeComponent<T> : SceneComponent where T : IShape
    {
        private T _shape;
        public T Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        protected override void OnRender(float delta) { _shape?.Render(delta); }
        public virtual void OnBeginOverlap()
        {

        }
        public virtual void OnEndOverlap()
        {
            
        }
    }
}
