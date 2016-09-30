using CustomEngine.Rendering.Meshes;
using CustomEngine.World;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eyecm.PhysX;

namespace CustomEngine.Collision
{
    public delegate void OnTraceHit(HitInfo hit);
    public class HitInfo
    {
        public Vector3 _hitNormal;
        public Vector3 _location;
        public World.Actor _hitActor;
        public Model _hitModel;
    }
}
