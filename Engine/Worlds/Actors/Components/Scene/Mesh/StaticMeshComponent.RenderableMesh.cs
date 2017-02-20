using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable
    {
        internal class RenderableMesh : IRenderable
        {
            public RenderableMesh(IStaticMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                _cullingVolume = _mesh.CullingVolume.HardCopy();
                _manager = new PrimitiveManager(_mesh.Data, _mesh.Material);
                Visible = false;
                IsRendering = true;
            }

            private PrimitiveManager _manager;
            private bool _isVisible, _isRendering;
            private SceneComponent _component;
            private IStaticMesh _mesh;
            private RenderOctree.Node _renderNode;
            private Shape _cullingVolume;

            public Shape CullingVolume => _cullingVolume;
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
            public RenderOctree.Node RenderNode
            {
                get => _renderNode;
                set => _renderNode = value;
            }
            public void Render()
            {
                _manager.Render(_component.WorldMatrix, _component.WorldMatrix.GetRotationMatrix3());
                //_manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
            }
            public override string ToString()
            {
                return ((ObjectBase)_mesh).Name;
            }
        }
    }
}
