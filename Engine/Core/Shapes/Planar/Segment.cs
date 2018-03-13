using TheraEngine.Rendering.Models;
using System;

namespace TheraEngine.Core.Shapes
{
    public struct Segment
    {
        public Segment(Vec3 startPoint, Vec3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Vec3 StartPoint;
        public Vec3 EndPoint;

        public Vec3 DirectionVector
        {
            get => EndPoint - StartPoint;
            set => EndPoint = StartPoint + value;
        }
        
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
            => PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), new VertexLine(new Vertex(start), new Vertex(end)));

        /// <summary>
        /// Returns the point on this segment that is closest to the given point.
        /// </summary>
        public Vec3 ClosestColinearPointToPoint(Vec3 point)
            => GetClosestColinearPointToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the point on the segment that is closest to the given point.
        /// </summary>
        public static Vec3 GetClosestColinearPointToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = segmentEndPoint - segmentStartPoint;
            Vec3 normDir = dir.NormalizedFast();
            Vec3 colinearDir = ((point - segmentStartPoint).Dot(normDir)) * normDir;
            if (colinearDir.Dot(normDir) < 0)
                return segmentStartPoint;
            if (colinearDir.LengthFast > dir.LengthFast)
                return segmentEndPoint;
            return segmentStartPoint + colinearDir;
        }

        /// <summary>
        /// Returns the shortest segment starting on this segment and ending at the given point.
        /// </summary>
        public Segment ShortestSegmentToPoint(Vec3 point)
            => GetShortestSegmentToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the shortest segment starting from the given point and ending on the segment.
        /// </summary>
        public static Segment GetShortestSegmentToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
            => new Segment(point, GetClosestColinearPointToPoint(segmentStartPoint, segmentEndPoint, point));

        /// <summary>
        /// Returns the shortest distance from the given point to this segment.
        /// </summary>
        public float ShortestDistanceToPoint(Vec3 point)
            => GetShortestDistanceToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the shortest distance from the point to the segment.
        /// </summary>
        public static float GetShortestDistanceToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = (segmentEndPoint - segmentStartPoint).NormalizedFast();
            float perpRayDist = (dir ^ (point - segmentStartPoint)).LengthFast;
            float closestEndDistance = Math.Min(point.DistanceToFast(segmentStartPoint), point.DistanceToFast(segmentEndPoint));
            return Math.Min(closestEndDistance, perpRayDist);
        }

        /// <summary>
        /// Transforms the endpoints of this segment by the given matrix and returns them as a new segment.
        /// </summary>
        public Segment TransformedBy(Matrix4 transform)
            => new Segment(
                Vec3.TransformPosition(StartPoint, transform), 
                Vec3.TransformPosition(EndPoint, transform));

        /// <summary>
        /// Transforms the endpoints of this segment by the given matrix.
        /// </summary>
        public void TransformBy(Matrix4 transform)
        {
            StartPoint = Vec3.TransformPosition(StartPoint, transform);
            EndPoint = Vec3.TransformPosition(EndPoint, transform);
        }

        /// <summary>
        /// Returns a colinear point the given distance from the start point of this segment.
        /// </summary>
        public Vec3 PointAtLineDistance(float distance)
        {
            Vec3 diff = EndPoint - StartPoint;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
    }
}
