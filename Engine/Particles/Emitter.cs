using System;
using CustomEngine.Files;

namespace CustomEngine.Particles
{
    public class Emitter : FileObject
    {
        public override ResourceType ResourceType
        {
            get
            {
                return ResourceType.Emitter;
            }
        }
    }
}
