using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public class RenderableLOD : TObject
    {
        public float VisibleDistance { get; set; }
        public PrimitiveManager Manager { get; set; }

        public ShaderVar[] Parameters => Manager.Material.Parameters;
    }
    public abstract class BaseRenderableMesh : I3DRenderable
    {
        public BaseRenderableMesh(List<LOD> lods, RenderInfo3D renderInfo, SceneComponent component)
        {
            _component = component;

            LODs = lods.Select(x => new RenderableLOD()
            {
                VisibleDistance = x.VisibleDistance,
                Manager = x.CreatePrimitiveManager(),
            }).ToArray();

            RenderInfo = renderInfo;
            RenderInfo.RenderOrderFunc = GetRenderDistance;

            Visible = false;
        }

        private bool _visible = false;
        protected SceneComponent _component;
        protected RenderableLOD _currentLOD = null;

        public RenderableLOD[] LODs { get; private set; }
        public RenderInfo3D RenderInfo { get; protected set; }

        [Browsable(false)]
        public virtual Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;
                _visible = value;
                if (_visible)
                    Engine.Scene.Add(this);
                else
                    Engine.Scene.Remove(this);
            }
        }
        public bool VisibleInEditorOnly { get; set; } = false;
        public bool HiddenFromOwner { get; set; } = false;
        public bool VisibleToOwnerOnly { get; set; } = false;

        private float GetRenderDistance(bool shadowPass)
        {
            float dist = GetDistance(AbstractRenderer.CurrentCamera);

            if (!shadowPass)
                UpdateLOD(dist);

            return dist;
        }

        public float GetDistance(Camera camera)
        {
            Vec3 camPoint = camera == null ? Vec3.Zero : camera.WorldPoint;
            Vec3 meshPoint = _component.WorldMatrix.GetPoint();
            return meshPoint.DistanceToFast(camPoint);
        }

        private void UpdateLOD(float viewDist)
        {
            _currentLOD = null;

            //Start with the lowest, farthest away LOD and work toward higher quality
            //Most renderables will be farther rather than closer, so this is fastest
            for (int i = LODs.Length - 1; i >= 0; --i)
            {
                _currentLOD = LODs[i];
                if (_currentLOD?.Manager == null || viewDist >= _currentLOD.VisibleDistance)
                    break;
            }

            Visible = _currentLOD?.Manager != null;
        }
        public void Render()
        {
            //Visible will be set to false if the current lod or its manager is null
            //Therefore this code will never be run in those circumstances
            _currentLOD.Manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
        }
    }
}
