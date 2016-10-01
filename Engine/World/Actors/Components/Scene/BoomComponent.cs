using CustomEngine.Rendering.Textures;
using CustomEngine.Collision;
using OpenTK;
using eyecm.PhysX;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoomComponent : SceneComponent
    {
        private Vector3 _startOffset;
        private Vector3 _endOffset;
        private float _length;

        public BoomComponent()
        {
            _hiddenInGame = true;
        }

        public void OnTraceHit(HitInfo hit)
        {
            Transform.Translation = hit._location;
            Transform.ApplyRelativeTranslation(_endOffset);
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        public override void Update()
        {
            base.Update();
            //eyecm.PhysX.Scene.RaycastAllShapes()
        }
    }
}
