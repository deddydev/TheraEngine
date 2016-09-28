using CustomEngine.Rendering.Textures;
using CustomEngine.System.Collision;
using OpenTK;
using eyecm.PhysX;

namespace CustomEngine.World.Actors.Components
{
    public class BoomComponent : SceneComponent
    {
        private Vector3 _startOffset;
        private Vector3 _endOffset;
        private float _length;

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
            eyecm.PhysX.Scene.RaycastAllShapes()
            
        }
    }
}
