using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors
{
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable
    {
        internal class RenderableMesh : IMesh
        {
            public RenderableMesh(IStaticMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                _cullingVolume = _mesh.CullingVolume.HardCopy();
                _manager = new PrimitiveManager(_mesh.Data, _mesh.Material);
                Visible = false;
                IsRendering = true;
                _component.WorldTransformChanged += _component_WorldTransformChanged;
            }

            private void _component_WorldTransformChanged()
            {
                _cullingVolume.SetTransform(_component.WorldMatrix);
            }

            private bool
                _ownerNoSee = false,
                _onlyOwnerSee = false, 
                _visibleInEditorOnly = false,
                _isVisible = true, 
                _isRendering = false;

            private PrimitiveManager _manager;
            private SceneComponent _component;
            private IStaticMesh _mesh;
            private Octree.Node _renderNode;
            private Shape _cullingVolume;

            public Shape CullingVolume => _cullingVolume;
            public bool Visible
            {
                get => _isVisible;
                set
                {
                    _isVisible = value;
                    if (_isVisible)
                    {
                        Engine.Renderer.Scene.AddRenderable(_cullingVolume);
                        Engine.Renderer.Scene.AddRenderable(this);
                    }
                    else
                    {
                        Engine.Renderer.Scene.RemoveRenderable(_cullingVolume);
                        Engine.Renderer.Scene.RemoveRenderable(this);
                    }
                }
            }
            public bool VisibleInEditorOnly
            {
                get => _visibleInEditorOnly;
                set => _visibleInEditorOnly = value;
            }
            public bool HiddenFromOwner
            {
                get => _ownerNoSee;
                set => _ownerNoSee = value;
            }
            public bool VisibleToOwnerOnly
            {
                get => _onlyOwnerSee;
                set => _onlyOwnerSee = value;
            }
            public bool IsRendering
            {
                get => _isRendering;
                set => _isRendering = value;
            }
            public IStaticMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
            }
            public Octree.Node RenderNode
            {
                get => _renderNode;
                set => _renderNode = value;
            }
            public void Render()
            {
                //_manager.Render(_component.WorldMatrix, _component.WorldMatrix.GetRotationMatrix3());
                _manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
            }
            public override string ToString()
            {
                return ((ObjectBase)_mesh).Name;
            }
        }
    }
}
