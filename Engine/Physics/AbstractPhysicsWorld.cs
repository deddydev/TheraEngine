using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsWorld : IDisposable
    {
        public virtual Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);
        
        public abstract bool RayTrace(RayTraceResult result);
        public abstract bool ShapeTrace(ShapeTraceResult result);

        public abstract void StepSimulation(float delta);

        public abstract void AddCollisionObject(TCollisionObject collision);
        public abstract void RemoveCollisionObject(TCollisionObject collision);
        
        public abstract void Dispose();
    }
}
