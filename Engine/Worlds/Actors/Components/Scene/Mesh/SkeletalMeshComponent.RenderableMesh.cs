using TheraEngine.Rendering.Models;
using System;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using System.Linq;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        public class RenderableMesh : ISubMesh
        {
            private RenderInfo3D _renderInfo;
            public RenderInfo3D RenderInfo => _renderInfo;

            private class LOD_Internal
            {
                public float VisibleDistance { get; set; }
                public PrimitiveManager Manager { get; set; }
            }

            public RenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
            {
                _mesh = mesh;
                _lods = new LOD_Internal[mesh.LODs.Count];
                for (int i = 0; i < _lods.Length; ++i)
                {
                    LOD lod = mesh.LODs[i];
                    _lods[i] = new LOD_Internal()
                    {
                        VisibleDistance = lod.VisibleDistance,
                        Manager = lod.CreatePrimitiveManager(),
                    };
                }
                _component = component;
                Skeleton = skeleton;
                Visible = false;

                _renderInfo = mesh.RenderInfo;
                _renderInfo.RenderOrderFunc = GetRenderOrderOpaque;
            }

            /// <summary>
            /// This method is called DIRECTLY BEFORE rendering, used to sort transparent scene objects.
            /// </summary>
            private float GetRenderOrderOpaque()
            {
                return GetDistance(AbstractRenderer.CurrentCamera);
            }
            /// <summary>
            /// This method is called DIRECTLY BEFORE rendering, used to sort opaque scene objects.
            /// </summary>
            private float GetRenderOrderTransparent()
            {
                return GetDistance(AbstractRenderer.CurrentCamera);
            }

            private float GetDistance(Camera c)
            {
                Vec3 camPoint = c == null ? Vec3.Zero : c.WorldPoint;
                Vec3 meshPoint = _component.WorldMatrix.GetPoint();
                float dist = meshPoint.DistanceToFast(camPoint);
                _currentLOD = -1;

                //Start with the lowest, farthest away LOD and work toward higher quality
                for (int i = _lods.Length - 1; i >= 0; --i)
                {
                    LOD_Internal lod = _lods[i];
                    if (lod.Manager == null)
                        break;

                    _currentLOD = i;

                    if (dist >= lod.VisibleDistance)
                        break;
                }

                Visible = _currentLOD >= 0;

                return dist;
            }

            private int _currentLOD = 0;
            private LOD_Internal[] _lods;

            private bool 
                _isVisible = false, 
                _visibleInEditorOnly = false,
                _hiddenFromOwner = false,
                _visibleToOwnerOnly = false;

            private SceneComponent _component;
            private ISkeletalSubMesh _mesh;
            //private Bone _singleBind;
            private Skeleton _skeleton;
            private Shape _cullingVolume;

            public bool Visible
            {
                get => _isVisible;
                set
                {
                    if (_isVisible == value)
                        return;
                    _isVisible = value;
                    if (_isVisible)
                        Engine.Scene.Add(this);
                    else
                        Engine.Scene.Remove(this);
                }
            }
            //public Bone SingleBind => _singleBind;
            public bool VisibleInEditorOnly
            {
                get => _visibleInEditorOnly;
                set => _visibleInEditorOnly = value;
            }
            public bool HiddenFromOwner
            {
                get => _hiddenFromOwner;
                set => _hiddenFromOwner = value;
            }
            public bool VisibleToOwnerOnly
            {
                get => _visibleToOwnerOnly;
                set => _visibleToOwnerOnly = value;
            }
            
            public ISkeletalSubMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
            }

            [Browsable(false)]
            public Shape CullingVolume => _cullingVolume;
            [Browsable(false)]
            public IOctreeNode OctreeNode { get; set; }

            [Browsable(false)]
            public Skeleton Skeleton
            {
                get => _skeleton;
                set
                {
                    _skeleton = value;
                    foreach (LOD_Internal m in _lods)
                        m.Manager?.SkeletonChanged(_skeleton);
                }
            }

            //IsRendering is checked by the octree before calling
            public void Render()
            {
                _lods[_currentLOD].Manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
                //_manager.Render(world, world.GetRotationMatrix3());
            }
            public override string ToString()
            {
                return ((ObjectBase)_mesh).Name;
            }
        }
    }
}
