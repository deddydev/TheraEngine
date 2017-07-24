using TheraEngine.Rendering.Models;
using System;
using System.ComponentModel;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        public class RenderableMesh : ISubMesh
        {
            private RenderInfo3D _renderInfo;
            public RenderInfo3D RenderInfo => _renderInfo;

            public RenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
            {
                _mesh = mesh;
                _manager = new PrimitiveManager(mesh.Data, mesh.Material);
                _component = component;
                Skeleton = skeleton;
                Visible = false;

                _renderInfo = mesh.RenderInfo;
                _renderInfo.RenderOrderFunc = GetRenderOrderOpaque;
            }

            private float GetRenderOrderOpaque()
            {
                return _component.WorldMatrix.GetPoint().DistanceToFast(AbstractRenderer.CurrentCamera.WorldPoint);
            }
            private float GetRenderOrderTransparent()
            {
                return _component.WorldMatrix.GetPoint().DistanceToFast(AbstractRenderer.CurrentCamera.WorldPoint);
            }

            private PrimitiveManager _manager;
            private bool 
                _isVisible, 
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
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public ISkeletalSubMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
            }

            [Browsable(false)]
            public Shape CullingVolume => _cullingVolume;
            [Browsable(false)]
            public IOctreeNode OctreeNode { get; set; }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Skeleton Skeleton
            {
                get => _skeleton;
                set
                {
                    _skeleton = value;
                    _manager.SkeletonChanged(_skeleton);
                }
            }

            //IsRendering is checked by the octree before calling
            public void Render()
            {
                if (Visible)
                {
                    _manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
                    //_manager.Render(world, world.GetRotationMatrix3());
                }
            }
            public override string ToString()
            {
                return ((ObjectBase)_mesh).Name;
            }
        }
    }
}
