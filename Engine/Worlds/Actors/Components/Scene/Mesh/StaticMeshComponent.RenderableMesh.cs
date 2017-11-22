using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IPhysicsDrivable
    {
        public class RenderableMesh : ISubMesh
        {
            private RenderInfo3D _renderInfo;
            public RenderInfo3D RenderInfo => _renderInfo;
            [Browsable(false)]
            public Shape CullingVolume => _cullingVolume;
            [Browsable(false)]
            public IOctreeNode OctreeNode { get; set; }

            public RenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            {
                _mesh = mesh;
                _component = component;
                _cullingVolume = _mesh.CullingVolume.File?.HardCopy();
                _initialCullingVolumeMatrix = _cullingVolume == null ? Matrix4.Identity : _cullingVolume.GetTransformMatrix();
                _manager = new PrimitiveManager(_mesh.Primitives, _mesh.Material);
//                if (_mesh.Data.BufferInfo.HasNormals && _mesh.Data.BufferInfo.HasTexCoords)
//                    _manager._geometryShader = new Shader(ShaderMode.Geometry, @"
//#version 450
//layout(triangles) in;
//layout(line_strip, max_vertices=6) out;

//in vec3 FragPos_geom[];
//in vec3 FragNorm_geom[];
//in vec2 FragUV0_geom[];

//uniform mat4 WorldToCameraSpaceMatrix;
//uniform mat4 ProjMatrix;

//out vec3 FragPos;
//out vec3 FragNorm;
//out vec2 FragUV0;

//in gl_PerVertex
//{
//    vec4  gl_Position;
//    float gl_PointSize;
//    float gl_ClipDistance[];
//} gl_in[];

//in int gl_PrimitiveIDIn;
//in int gl_InvocationID;

//out gl_PerVertex
//{
//    vec4  gl_Position;
//    float gl_PointSize;
//    float gl_ClipDistance[];
//};

//out int gl_PrimitiveID;
//out int gl_Layer;
//out int gl_ViewportIndex; 

//void main()
//{
//    int i;
//    for (i = 0; i < gl_in.length(); i++)
//    {
//        vec3 P = FragPos_geom[i].xyz;
//        vec3 N = FragNorm_geom[i];

//        gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * vec4(P, 1.0f);
//        FragPos = FragPos_geom[i];
//        FragNorm = FragNorm_geom[i];
//        EmitVertex();

//        gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * vec4(P + N * 10.0f, 1.0f);
//        FragPos = FragPos_geom[i];
//        FragNorm = FragNorm_geom[i];
//        EmitVertex();
    
//        EndPrimitive();
//    }
//}
//");
                _component.WorldTransformChanged += _component_WorldTransformChanged;
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

            private void _component_WorldTransformChanged()
            {
                if (_cullingVolume != null)
                {
                    _cullingVolume.SetRenderTransform(_component.WorldMatrix * _initialCullingVolumeMatrix);
                    OctreeNode?.ItemMoved(this);
                }
            }

            private bool
                _ownerNoSee = false,
                _onlyOwnerSee = false,
                _visibleInEditorOnly = false,
                _isVisible = true;

            private Matrix4 _initialCullingVolumeMatrix;
            private PrimitiveManager _manager;
            private SceneComponent _component;
            private IStaticSubMesh _mesh;
            private Shape _cullingVolume;
            
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
            public IStaticSubMesh Mesh
            {
                get => _mesh;
                set => _mesh = value;
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
