using System;

namespace TheraEngine.Physics
{
    public class PhysicsSupportAttribute : Attribute
    {
        public PhysicsLibrary[] PhysicsLibraries { get; }

        public PhysicsSupportAttribute(params PhysicsLibrary[] libraries)
        {
            PhysicsLibraries = libraries;
        }
    }
}