using System.Collections.Generic;

namespace TheraEngine.Physics
{
    public abstract class TGhostBody : TCollisionObject
    {
        protected TGhostBody() : base() { }

        /// <summary>
        /// Creates a new rigid body using the specified physics library.
        /// </summary>
        /// <param name="info">Construction information.</param>
        /// <returns>A new rigid body.</returns>
        public static TGhostBody New(TGhostBodyConstructionInfo info)
            => Engine.Physics.NewGhostBody(info);

        public abstract List<TCollisionObject> CollectOverlappingPairs();
    }
}
