using ObjLoader.Loader.Loaders;
using System.Linq;
using ObjLoader.Loader.Data.Elements;
using System;
using TheraEngine.Rendering.Models.Materials;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TheraEngine.Rendering.Models
{
    public static class OBJ
    {
        public static StaticMesh Import(string path, ModelImportOptions options)
        {
            StaticMesh m = new StaticMesh();
            LoadResult result = new ObjLoaderFactory().Create().Load(path);
            Matrix4 modelMatrix = options.InitialTransform.Matrix;
            Matrix4 normalMatrix = options.InitialTransform.InverseMatrix.Transposed().GetRotationMatrix4();
            string dirPath = Path.GetDirectoryName(path);
            foreach (var group in result.Groups)
            {
                foreach (var subgroup in group.SubGroups)
                {
                    BoundingBox b = BoundingBox.ExpandableBox();
                    PrimitiveData data = PrimitiveData.FromTriangleList(Culling.Back, VertexShaderDesc.PosNormTex(), subgroup.Faces.SelectMany(x => CreateTriangles(x, result, false, modelMatrix, normalMatrix, b)));
                    var objMat = result.Materials.FirstOrDefault(x => x.Name == subgroup.Material?.Name);
                    m.RigidChildren.Add(new StaticRigidSubMesh(group.Name + "_" + subgroup.Material.Name, data, b, CreateMaterial(objMat, dirPath, options.UseForwardShaders)));
                }
            }
            return m;
        }

        private static VertexTriangle[] CreateTriangles(Face face, LoadResult result, bool swapWinding, Matrix4 modelTransform, Matrix4 normalTransform, BoundingBox bounds)
        {
            //First convert obj face points to vertex points
            Vertex[] vertices = new Vertex[face.Count];
            for (int i = 0; i < vertices.Length; ++i)
            {
                int v = face[i].VertexIndex - 1;
                int n = face[i].NormalIndex - 1;
                int t = face[i].TextureIndex - 1;
                
                Vec3 pos;
                Vec2 uv;

                if (result.Vertices.IndexInRange(v))
                {
                    pos = result.Vertices[v] * modelTransform;
                    bounds.Expand(pos);
                }
                else
                    pos = Vec3.Zero;

                Vec3 norm = result.Normals.IndexInRange(n) ? result.Normals[n] * normalTransform : Vec3.Zero;

                if (result.TexCoords.IndexInRange(t))
                {
                    uv = result.TexCoords[t];
                    uv.Y = 1.0f - uv.Y;
                }
                else
                    uv = Vec2.Zero;

                vertices[i] = new Vertex(pos, norm, uv);
            }

            //Then group them into triangles
            if (vertices.Length == 3)
                return new VertexTriangle[] { new VertexTriangle(vertices[0], vertices[1], vertices[2]) };
            else if (vertices.Length == 4)
                return new VertexQuad(vertices[0], vertices[1], vertices[2], vertices[3]).ToTriangles();
            else
                return new VertexTriangleFan(vertices).ToTriangles();
        }

        private static Material CreateMaterial(ObjLoader.Loader.Data.Material objMat, string dirPath, bool forward)
        {
            Shader shader = GetOBJFragmentShader(forward);
            if (objMat == null)
            {
                ShaderVar[] parameters = new ShaderVar[]
                {
                    new ShaderVec3(Vec3.Zero, "Ambient"),
                    new ShaderVec3(Vec3.Zero, "Diffuse"),
                    new ShaderVec3(Vec3.Zero, "Specular"),
                    new ShaderFloat(0.0f, "SpecularCoef"),
                    new ShaderFloat(0.0f, "Transparency"),
                };
                return new Material("UnnamedMaterial", parameters, new TextureReference[0], shader)
                {
                    Requirements = forward ? Material.UniformRequirements.NeedsLightsAndCamera : Material.UniformRequirements.None
                };
            }
            else
            {
                ShaderVar[] parameters = new ShaderVar[]
                {
                    new ShaderVec3(objMat.AmbientColor, "Ambient"),
                    new ShaderVec3(objMat.DiffuseColor, "Diffuse"),
                    new ShaderVec3(objMat.SpecularColor, "Specular"),
                    new ShaderFloat(objMat.SpecularCoefficient, "SpecularCoef"),
                    new ShaderFloat(objMat.Transparency, "Transparency"),
                };
                string mapPath = objMat.DiffuseTextureMap;
                TextureReference[] textures = new TextureReference[]
                {
                    new TextureReference(Path.GetFileNameWithoutExtension(mapPath), mapPath.Contains(":") ? mapPath : dirPath + "\\" + mapPath ),
                };
                return new Material(objMat.Name, parameters, textures, shader)
                {
                    Requirements = forward ? Material.UniformRequirements.NeedsLightsAndCamera : Material.UniformRequirements.None
                };
            }
        }

        private static Shader GetOBJFragmentShader(bool forward)
        {
            if (forward)
                return new Shader(ShaderMode.Fragment, @"
#version 450

layout (location = 0) out vec4 OutColor;

in vec3 FragPos;
in vec3 FragNorm;
in vec2 FragUV0;

uniform vec3 Ambient;
uniform vec3 Diffuse;
uniform vec3 Specular;
uniform vec3 SpecularCoef;
uniform vec3 Transparency;

uniform sampler2D Texture0;

uniform vec3 CameraPosition;

" + ShaderHelpers.LightingSetupBasic() + @"

void main()
{
    vec3 normal = normalize(FragNorm);
    vec3 diffuseColor = texture(Texture0, FragUV0).rgb * Diffuse;
    " + ShaderHelpers.LightingCalc("totalLight", "vec3(0.0)", "normal", "FragPos", "diffuseColor", "0.0") + @"
    OutColor = vec4(diffuseColor * totalLight, 1.0);
}");
            else
                return new Shader(ShaderMode.Fragment, @"
#version 450

layout (location = 0) out vec4 AlbedoSpec;
layout (location = 1) out vec3 Normal;

in vec3 FragPos;
in vec3 FragNorm;
in vec2 FragUV0;

uniform vec3 Ambient;
uniform vec3 Diffuse;
uniform vec3 Specular;
uniform vec3 SpecularCoef;
uniform vec3 Transparency;

uniform sampler2D Texture0;

void main()
{
    AlbedoSpec = vec4(texture(Texture0, FragUV0).rgb * Diffuse, 0.0);
    Normal = normalize(FragNorm);
}");
        }
    }
}
