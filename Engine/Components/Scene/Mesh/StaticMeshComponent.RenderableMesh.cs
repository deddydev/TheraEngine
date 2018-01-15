using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models.Materials;
using System.IO;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IRigidCollidable
    {
        public class RenderableMesh : BaseRenderableMesh
        {
            public IStaticSubMesh Mesh { get; set; }
            public override Shape CullingVolume => _cullingVolume;
            
            public void SetCullingVolume(Shape shape)
            {
                if (_cullingVolume != null)
                    _component.WorldTransformChanged -= _component_WorldTransformChanged;
                _cullingVolume = shape.HardCopy();
                if (_cullingVolume != null)
                {
                    _initialCullingVolumeMatrix = _cullingVolume.GetTransformMatrix();
                    _component.WorldTransformChanged += _component_WorldTransformChanged;
                }
                else
                    _initialCullingVolumeMatrix = Matrix4.Identity;
            }

            private Shape _cullingVolume;
            private Matrix4 _initialCullingVolumeMatrix;

            public RenderableMesh(IStaticSubMesh mesh, SceneComponent component) 
                : base(mesh.LODs, mesh.RenderInfo, component)
            {
                Mesh = mesh;
                SetCullingVolume(mesh.CullingVolume);

                //PrimitiveManager m = LODs.First.Value.Manager;
                //if (m.Data.BufferInfo.HasNormals)
                //    m.Material.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", ShaderMode.Geometry));
            }
            private void _component_WorldTransformChanged()
            {
                _cullingVolume.SetRenderTransform(_component.WorldMatrix * _initialCullingVolumeMatrix);
                OctreeNode?.ItemMoved(this);
            }
            public override string ToString() => Mesh.Name;
        }
    }
}
