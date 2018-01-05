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
                
                //if (mesh.Data.BufferInfo.HasNormals && mesh.Data.BufferInfo.HasTexCoords)
                //    _manager._geometryShader = new Shader(ShaderMode.Geometry, @"
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
