﻿using CustomEngine.Rendering;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class GravityVolumeComponent : BoxComponent
    {
        private Vec3 _gravity = new Vec3(0.0f, -9.81f, 0.0f);
        public GravityVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null)
        {
            
        }

        public void OnOverlapEntered(IPhysicsDrivable driver)
        {
            driver.PhysicsDriver.CollisionObject.Gravity = _gravity;
        }
        public void OnOverlapLeft(IPhysicsDrivable driver)
        {
            driver.PhysicsDriver.CollisionObject.Gravity = Engine.World.State.Gravity;
        }
    }
}
