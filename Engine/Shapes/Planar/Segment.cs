using TheraEngine.Rendering.Models;
using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Represents a line in 3D space with specific start and end points.
    /// </summary>
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
        [Browsable(false)]
        public float Length => DirectionVector.Length;
        [Browsable(false)]
        public float LengthFast => DirectionVector.LengthFast;
        [Browsable(false)]
        public float LengthSquared => DirectionVector.LengthSquared;

        public enum ESegmentPart
        {
            StartPoint,
            Line,
            EndPoint
        }
        public static ESegmentPart GetDistancePointToSegmentPart(
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
                    return ESegmentPart.Line;
                }
                closestPartDist = distToEnd;
                return ESegmentPart.EndPoint;
            }
            else
            {
                if (distToStart < distToEnd)
                {
                    closestPartDist = distToStart;
                    return ESegmentPart.StartPoint;
                }
                closestPartDist = distToEnd;
                return ESegmentPart.EndPoint;
            }
        }

        public static Mesh Mesh(Vec3 start, Vec3 end)
            => Rendering.Models.Mesh.Create(VertexShaderDesc.JustPositions(), new VertexLine(start, end));

        #region Closest Colinear Point To Point
        /// <summary>
        /// Returns the point on this segment that is closest to the given point.
        /// </summary>
        public Vec3 ClosestColinearPointToPoint(Vec3 point)
            => ClosestColinearPointToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the point on this segment that is closest to the given point.
        /// </summary>
        public Vec3 ClosestColinearPointToPointFast(Vec3 point)
            => ClosestColinearPointToPointFast(StartPoint, EndPoint, point);

        /// <summary>
        /// Returns the point on the segment that is closest to the given point.
        /// </summary>
        public static Vec3 ClosestColinearPointToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = segmentEndPoint - segmentStartPoint;
            Vec3 normDir = dir.Normalized();
            Vec3 colinearDir = ((point - segmentStartPoint).Dot(normDir)) * normDir;
            if (colinearDir.Dot(normDir) < 0)
                return segmentStartPoint;
            if (colinearDir.Length > dir.Length)
                return segmentEndPoint;
            return segmentStartPoint + colinearDir;
        }
        /// <summary>
        /// Returns the point on the segment that is closest to the given point.
        /// </summary>
        public static Vec3 ClosestColinearPointToPointFast(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
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
        #endregion

        #region Shortest Segment To Point
        /// <summary>
        /// Returns the shortest segment starting on this segment and ending at the given point.
        /// </summary>
        public Segment ShortestSegmentToPoint(Vec3 point)
            => ShortestSegmentToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the shortest segment starting on this segment and ending at the given point.
        /// </summary>
        public Segment ShortestSegmentToPointFast(Vec3 point)
            => ShortestSegmentToPointFast(StartPoint, EndPoint, point);

        /// <summary>
        /// Returns the shortest segment starting from the given point and ending on the segment.
        /// </summary>
        public static Segment ShortestSegmentToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
            => new Segment(point, ClosestColinearPointToPoint(segmentStartPoint, segmentEndPoint, point));

        public static Vec3 PointAtLineDistance(object worldPoint, Vec3 hitPoint, float v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the shortest segment starting from the given point and ending on the segment.
        /// Uses an approximation of square root, so the results are less accurate but the calculation is faster.
        /// </summary>
        public static Segment ShortestSegmentToPointFast(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
            => new Segment(point, ClosestColinearPointToPointFast(segmentStartPoint, segmentEndPoint, point));
        #endregion

        #region Shortest Distance To Point
        /// <summary>
        /// Returns the shortest distance from the given point to this segment.
        /// </summary>
        public float ShortestDistanceToPoint(Vec3 point)
            => ShortestDistanceToPoint(StartPoint, EndPoint, point);
        /// <summary>
        /// Returns the shortest distance from the given point to this segment.
        /// </summary>
        public float ShortestDistanceToPointFast(Vec3 point)
            => ShortestDistanceToPointFast(StartPoint, EndPoint, point);

        /// <summary>
        /// Returns the shortest distance from the point to the segment.
        /// </summary>
        public static float ShortestDistanceToPoint(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = (segmentEndPoint - segmentStartPoint).Normalized();
            float perpRayDist = (dir ^ (point - segmentStartPoint)).Length;
            float closestEndDistance = Math.Min(point.DistanceTo(segmentStartPoint), point.DistanceTo(segmentEndPoint));
            return Math.Min(closestEndDistance, perpRayDist);
        }
        /// <summary>
        /// Returns the shortest distance from the point to the segment.
        /// Uses an approximation of square root, so the results are less accurate but the calculation is faster.
        /// </summary>
        public static float ShortestDistanceToPointFast(Vec3 segmentStartPoint, Vec3 segmentEndPoint, Vec3 point)
        {
            Vec3 dir = (segmentEndPoint - segmentStartPoint).NormalizedFast();
            float perpRayDist = (dir ^ (point - segmentStartPoint)).LengthFast;
            float closestEndDistance = Math.Min(point.DistanceToFast(segmentStartPoint), point.DistanceToFast(segmentEndPoint));
            return Math.Min(closestEndDistance, perpRayDist);
        }
        #endregion

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
        public static Vec3 PointAtLineDistance(Vec3 startPoint, Vec3 endPoint, float distance)
        {
            Vec3 diff = endPoint - startPoint;
            return startPoint + (diff * (distance / diff.LengthFast));
        }
    }
}
