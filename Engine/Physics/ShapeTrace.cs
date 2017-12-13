using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.ShapeTracing
{
    public abstract class ShapeTraceResult
    {
        public TCollisionShape Shape { get; private set; }
        public Matrix4 Start { get; private set; }
        public Matrix4 End { get; private set; }
        public ushort CollisionGroup { get; set; }
        public ushort CollidesWith { get; set; }

        public ShapeTraceResult(Matrix4 start, Matrix4 end, TCollisionShape shape, ushort collisionGroup, ushort collidesWith)
        {
            Start = start;
            End = end;
            Shape = shape;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
        }
    }

    public abstract class ShapeTraceResultSingle : ShapeTraceResult
    {
        public PhysicsDriver Result { get; protected set; }

        public ShapeTraceResultSingle(Matrix4 start, Matrix4 end, TCollisionShape shape, ushort collisionGroup, ushort collidesWith)
            : base(start, end, shape, collisionGroup, collidesWith) { }
    }

    public class ShapeTraceResultMulti : ShapeTraceResult
    {
        public PhysicsDriver[] Result { get; protected set; }

        public ShapeTraceResultMulti(Matrix4 start, Matrix4 end, TCollisionShape shape, ushort collisionGroup, ushort collidesWith)
            : base(start, end, shape, collisionGroup, collidesWith) { }
    }

    public class ShapeTraceResultClosest : ShapeTraceResultSingle
    {
        public ShapeTraceResultClosest(Matrix4 start, Matrix4 end, TCollisionShape shape, ushort collisionGroup, ushort collidesWith)
            : base(start, end, shape, collisionGroup, collidesWith) { }
    }
}
