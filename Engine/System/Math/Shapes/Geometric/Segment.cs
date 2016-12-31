namespace System
{
    public class Segment
    {
        public Segment() { }
        public Segment(Vec3 startPoint, Vec3 endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }
        
        public Vec3 StartPoint { get { return _startPoint; } set { _startPoint = value; } }
        public Vec3 EndPoint { get { return _endPoint; } set { _endPoint = value; } }
        public Vec3 Direction
        {
            get { return _endPoint - _startPoint; }
            set { _endPoint = _startPoint + value; }
        }

        private Vec3 _startPoint;
        private Vec3 _endPoint;

        public Segment TransformedBy(Matrix4 transform)
        {
            return new Segment(Vec3.TransformPosition(StartPoint, transform), Vec3.TransformPosition(EndPoint, transform));
        }
        public Vec3 PointAtLineDistance(float distance)
        {
            Vec3 diff = EndPoint - StartPoint;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
    }
}
