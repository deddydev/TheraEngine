using TheraEngine.Rendering.Models;
using System;
using System.ComponentModel;

namespace TheraEngine.Worlds.Actors
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        public class RenderableMesh : ISubMesh
        {
            public RenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
            {
                _mesh = mesh;
                _manager = new PrimitiveManager(mesh.Data, mesh.Material);
                _component = component;
                Skeleton = skeleton;
                Visible = false;
                IsRendering = true;
            }

            private PrimitiveManager _manager;
            private bool 
                _isVisible, 
                _isRendering,
                _visibleInEditorOnly = false,
                _hiddenFromOwner = false,
                _visibleToOwnerOnly = false;

            private SceneComponent _component;
            private ISkeletalSubMesh _mesh;
            private IOctreeNode _renderNode;
            //private Bone _singleBind;
            private Skeleton _skeleton;
            private Shape _cullingVolume;
            public bool HasTransparency => _mesh.Material.HasTransparency;

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
            public bool IsRendering
            {
                get => _isRendering;
                set => _isRendering = value;
            }
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
            public IOctreeNode OctreeNode
            {
                get => _renderNode;
                set => _renderNode = value;
            }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Skeleton Skeleton
            {
                get => _skeleton;
                set
                {
                    _skeleton = value;

                    //TODO: support multi-bone influence as single bind as well
                    //_singleBind = _mesh.SingleBindName != null && _skeleton != null ? _skeleton.GetBone(_mesh.SingleBindName) : null;
                    
                    _manager.SkeletonChanged(_skeleton);
                }
            }
            [Browsable(false)]
            public Shape CullingVolume => _cullingVolume;

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
