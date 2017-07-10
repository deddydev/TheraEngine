using TheraEngine.Rendering.Models;
using System.ComponentModel;

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
        
        public Vec3 StartPoint
        {
            get => _startPoint;
            set => _startPoint = value;
        }
        public Vec3 EndPoint
        {
            get => _endPoint;
            set => _endPoint = value;
        }
        public Vec3 Direction
        {
            get => _endPoint - _startPoint;
            set => _endPoint = _startPoint + value;
        }

        [DefaultValue("0 0 0")]
        [Serialize("StartPoint")]
        private Vec3 _startPoint;
        [DefaultValue("0 0 0")]
        [Serialize("EndPoint")]
        private Vec3 _endPoint;

        public enum Part
        {
            StartPoint,
            Line,
            EndPoint
        }
        public static Part GetDistancePointToSegmentPart(
            Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point, out float closestPartDist)
        {
            Vec3 dir = segmentEndPoint - segmentStartPoint;
            dir.NormalizeFast();
            float perpRayDist = (dir ^ (point - segmentStartPoint)).LengthFast;
            float distToStart = point.DistanceToFast(segmentStartPoint);
            float distToEnd = point.DistanceToFast(segmentEndPoint);
            if (perpRayDist < distToStart)
            {
                if (perpRayDist < distToEnd)
                {
                    closestPartDist = perpRayDist;
                    return Part.Line;
                }
                closestPartDist = distToEnd;
                return Part.EndPoint;
            }
            else
            {
                if (distToStart < distToEnd)
                {
                    closestPartDist = distToStart;
                    return Part.StartPoint;
                }
                closestPartDist = distToEnd;
                return Part.EndPoint;
            }
        }

        public static PrimitiveData Mesh(Vec3 start, Vec3 end)
        {
            return PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), new VertexLine(new Vertex(start), new Vertex(end)));
        }

        public static float GetClosestDistanceToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = segmentEndPoint - segmentStartPoint;
            dir.NormalizeFast();
            float perpRayDist = (dir ^ (point - segmentStartPoint)).LengthFast;
            float closestEndDistance = Math.Min(point.DistanceToFast(segmentStartPoint), point.DistanceToFast(segmentEndPoint));
            return Math.Min(closestEndDistance, perpRayDist);
        }
        public float ClosestDistanceToPoint(Vec3 point)
            => GetClosestDistanceToPoint(StartPoint, EndPoint, point);
        public Segment TransformedBy(Matrix4 transform)
            => new Segment(
                Vec3.TransformPosition(StartPoint, transform), 
                Vec3.TransformPosition(EndPoint, transform));
        public void TransformBy(Matrix4 transform)
        {
            StartPoint = Vec3.TransformPosition(StartPoint, transform);
            EndPoint = Vec3.TransformPosition(EndPoint, transform);
        }
        public Vec3 PointAtLineDistance(float distance)
        {
            Vec3 diff = EndPoint - StartPoint;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
    }
}
