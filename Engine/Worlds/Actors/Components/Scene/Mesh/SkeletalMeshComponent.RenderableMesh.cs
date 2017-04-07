using CustomEngine.Rendering.Models;
using System;

namespace CustomEngine.Worlds.Actors
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        internal class RenderableMesh : IMesh
        {
            public RenderableMesh(ISkeletalMesh mesh, Skeleton skeleton, SceneComponent component)
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
            private ISkeletalMesh _mesh;
            private Octree.Node _renderNode;
            private Bone _singleBind;
            private Skeleton _skeleton;
            private Shape _cullingVolume;
            
            public bool Visible
            {
                get => _isVisible;
                set
                {
                    _isVisible = value;
                    if (_isVisible)
                        Engine.Renderer.Scene.AddRenderable(this);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(this);
                }
            }
            public Bone SingleBind => _singleBind;
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
            public ISkeletalMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
            }
            public Octree.Node RenderNode
            {
                get => _renderNode;
                set => _renderNode = value;
            }
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
            public Shape CullingVolume => _cullingVolume;

            //IsRendering is checked by the octree before calling
            public void Render()
            {
                if (Visible)
                {
                    Matrix4 world, invWorld;
                    if (_singleBind != null)
                    {
                        world = _singleBind.WorldMatrix;
                        invWorld = _singleBind.InverseWorldMatrix;
                    }
                    else
                    {
                        world = _component.WorldMatrix;
                        invWorld = _component.InverseWorldMatrix;
                    }

                    _manager.Render(world, invWorld.Transposed().GetRotationMatrix3());
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
