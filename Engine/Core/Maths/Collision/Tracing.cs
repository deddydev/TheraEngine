using TheraEngine.Rendering.Models;
using TheraEngine.Worlds;
using System;

namespace TheraEngine
{
    public delegate void OnTraceHit(HitInfo hit);
    public class HitInfo
    {
        public Vec3 _hitNormal;
        public Vec3 _location;
        public IActor _hitActor;
        public SkeletalMesh _hitModel;
    }
}
