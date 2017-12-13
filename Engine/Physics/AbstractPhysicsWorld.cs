using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsWorld : IDisposable
    {
        public virtual Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);
        
        public abstract void RayTrace(Vec3 start, Vec3 end, RayTraceResult result);
        public abstract void ShapeTrace(Matrix4 start, Matrix4 end, TCollisionShape shape, ShapeTraceResult result);

        public abstract void StepSimulation(float delta);

        public abstract void AddCollisionObject(TCollisionObject collision);
        public abstract void RemoveCollisionObject(TCollisionObject collision);

        public abstract void Destroy();
        public abstract void Dispose();
    }
    public abstract class RayTraceResult
    {

    }
    public class RayTraceResultSingle : RayTraceResult
    {
        public PhysicsDriver Result { get; protected set; }
    }
    public class RayTraceResultMulti : RayTraceResult
    {
        public PhysicsDriver[] Result { get; protected set; }
    }
    public abstract class ShapeTraceResult
    {

    }
    public class ShapeTraceResultSingle : ShapeTraceResult
    {
        public PhysicsDriver Result { get; protected set; }
    }
    public class ShapeTraceResultMulti : ShapeTraceResult
    {
        public PhysicsDriver[] Result { get; protected set; }
    }
}
