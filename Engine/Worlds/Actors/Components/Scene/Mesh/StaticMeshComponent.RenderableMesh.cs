using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable
    {
        public class RenderableMesh : ISubMesh
        {
            public RenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                _cullingVolume = _mesh.CullingVolume?.HardCopy();
                _manager = new PrimitiveManager(_mesh.Data, _mesh.Material);
                Visible = false;
                IsRendering = true;
                _component.WorldTransformChanged += _component_WorldTransformChanged;
            }

            private void _component_WorldTransformChanged()
            {
                if (_cullingVolume != null)
                {
                    _cullingVolume.SetTransform(_component.WorldMatrix);
                    OctreeNode?.ItemMoved(this);
                }
            }

            private bool
                _ownerNoSee = false,
                _onlyOwnerSee = false, 
                _visibleInEditorOnly = false,
                _isVisible = true, 
                _isRendering = false;

            private PrimitiveManager _manager;
            private SceneComponent _component;
            private IStaticSubMesh _mesh;
            private IOctreeNode _renderNode;
            private Shape _cullingVolume;

            public bool HasTransparency => _mesh.Material.HasTransparency;
            public Shape CullingVolume => _cullingVolume;
            public bool Visible
            {
                get => _isVisible;
                set
                {
                    _isVisible = value;
                    if (Engine.Renderer != null && Engine.Scene != null)
                    {
                        if (_isVisible)
                        {
                            if (_cullingVolume != null && Engine.Settings.RenderCullingVolumes)
                                Engine.Scene.Add(_cullingVolume);
                            Engine.Scene.Add(this);
                        }
                        else
                        {
                            if (_cullingVolume != null && Engine.Settings.RenderCullingVolumes)
                                Engine.Scene.Remove(_cullingVolume);
                            Engine.Scene.Remove(this);
                        }
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
            public IStaticSubMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
            }
            public IOctreeNode OctreeNode
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
