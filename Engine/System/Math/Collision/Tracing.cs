﻿using CustomEngine.Rendering.Models;
using CustomEngine.Worlds;
using System;

namespace CustomEngine.Collision
{
    public delegate void OnTraceHit(HitInfo hit);
    public class HitInfo
    {
        public Vec3 _hitNormal;
        public Vec3 _location;
        public Actor _hitActor;
        public Model _hitModel;
    }
}
