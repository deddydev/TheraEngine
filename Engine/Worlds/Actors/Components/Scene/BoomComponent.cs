using CustomEngine.Collision;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoomComponent : SceneComponent
    {
        private Vec3 _startOffset;
        private Vec3 _endOffset;
        private float _length;

        public BoomComponent()
        {
            //_hiddenInGame = true;
        }

        public void OnTraceHit(HitInfo hit)
        {
            LocalTransform.Translation = hit._location;
            LocalTransform.TranslateRelative(_endOffset);
        }
    }
}
