using TheraEngine.Rendering.Models;
using TheraEngine.Actors;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine
{
    public delegate void OnTraceHit(HitInfo hit);
    public class HitInfo
    {
        public Vec3 _hitNormal;
        public Vec3 _location;
        public IActor _hitActor;
        public SkeletalModel _hitModel;
    }
}
