using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IRigidCollidable
    {
        public class RenderableMesh : ISubMesh
        {
            private RenderInfo3D _renderInfo;
            public RenderInfo3D RenderInfo => _renderInfo;

            public class LOD_Internal
            {
                public float VisibleDistance { get; set; }
                public PrimitiveManager Manager { get; set; }

                public ShaderVar[] Parameters => Manager.Material.Parameters;
            }

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

                //Start with the lowest, farthest away LOD and test toward higher quality
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

            private void _component_WorldTransformChanged()
            {
                if (_cullingVolume != null)
                {
                    _cullingVolume.SetRenderTransform(_component.WorldMatrix * _initialCullingVolumeMatrix);
                    OctreeNode?.ItemMoved(this);
                }
            }

            private int _currentLOD = 0;
            private LOD_Internal[] _lods;

            public LOD_Internal[] LODs => _lods;

            private bool
                _ownerNoSee = false,
                _onlyOwnerSee = false,
                _visibleInEditorOnly = false,
                _isVisible = true;

            private Matrix4 _initialCullingVolumeMatrix;
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
                            Engine.Scene.Add(this);
                        }
                        else
                        {
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
                _lods[_currentLOD].Manager.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
            }
            public override string ToString()
            {
                return ((TObject)_mesh).Name;
            }
        }
    }
}
