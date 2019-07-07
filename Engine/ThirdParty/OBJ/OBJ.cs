using Extensions;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using System;
using System.IO;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public static class OBJ
    {
        public static StaticModel Import(string path, ColladaImportOptions options)
        {
            LoadResult result = new ObjLoaderFactory().Create().Load(path);

            Matrix4 modelMatrix = options.InitialTransform.Matrix;
            Matrix4 normalMatrix = options.InitialTransform.InverseMatrix.Transposed().GetRotationMatrix4();
            string dirPath = Path.GetDirectoryName(path);

            StaticModel model = new StaticModel();
            foreach (var group in result.Groups)
            {
                foreach (var subgroup in group.SubGroups)
                {
                    BoundingBox bounds = BoundingBox.ExpandableBox();
                    PrimitiveData data = PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), 
                        subgroup.Faces.SelectMany(x => CreateTriangles(x, result, false, modelMatrix, normalMatrix, bounds)));

                    var objMat = result.Materials.FirstOrDefault(x => x.Name == subgroup.Material?.Name);
                    RenderInfo3D renderInfo = new RenderInfo3D(true, false) { CullingVolume = bounds };
                    model.RigidChildren.Add(new StaticRigidSubMesh(
                        group.Name + "_" + (subgroup.Material?.Name ?? "NoMaterial"),
                        renderInfo, ERenderPass.OpaqueDeferredLit, data, 
                        CreateMaterial(objMat, dirPath, options.UseForwardShaders)));
                }
            }
            return model;
        }

        private static VertexTriangle[] CreateTriangles(
            Face face, 
            LoadResult result, 
            bool swapWinding,
            Matrix4 modelTransform,
            Matrix4 normalTransform,
            BoundingBox bounds)
        {
            //First convert obj face points to vertex points
            Vertex[] vertices = new Vertex[face.Count];
            for (int i = 0; i < vertices.Length; ++i)
            {
                int vtxIndex = face[i].VertexIndex - 1;
                int nrmIndex = face[i].NormalIndex - 1;
                int texIndex = face[i].TextureIndex - 1;
                
                Vec3 pos;
                Vec2 uv;

                if (result.Vertices.IndexInRange(vtxIndex))
                {
                    pos = Vec3.TransformPosition(result.Vertices[vtxIndex], modelTransform);
                    bounds.Expand(pos);
                }
                else
                    pos = Vec3.Zero;

                Vec3 norm = result.Normals.IndexInRange(nrmIndex) ? Vec3.TransformVector(result.Normals[nrmIndex], normalTransform) : Vec3.Zero;

                if (result.TexCoords.IndexInRange(texIndex))
                {
                    uv = result.TexCoords[texIndex];
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

        private static TMaterial CreateMaterial(ObjLoader.Loader.Data.Material objMat, string dirPath, bool forward)
        {
            GLSLScript shader = GetOBJFragmentShader(forward);
            if (objMat == null)
            {
                ShaderVar[] parameters = new ShaderVar[]
                {
                    //new ShaderVec3(Vec3.Zero, "Ambient"),
                    new ShaderVec3(Vec3.Zero, "Diffuse"),
                    //new ShaderVec3(Vec3.Zero, "Specular"),
                    //new ShaderFloat(0.0f, "SpecularCoef"),
                    //new ShaderFloat(0.0f, "Transparency"),
                };
                RenderingParameters param = new RenderingParameters()
                {
                    Requirements = EUniformRequirements.None
                };
                return new TMaterial("UnnamedMaterial", param, parameters, new TexRef2D[0], shader);
            }
            else
            {
                ShaderVar[] parameters = new ShaderVar[]
                {
                    //new ShaderVec3(objMat.AmbientColor, "Ambient"),
                    new ShaderVec3(objMat.DiffuseColor, "Diffuse"),
                    //new ShaderVec3(objMat.SpecularColor, "Specular"),
                    //new ShaderFloat(objMat.SpecularCoefficient, "SpecularCoef"),
                    //new ShaderFloat(objMat.Transparency, "Transparency"),
                };

                string mapPath = objMat.DiffuseTextureMap;

                TexRef2D[] textures;
                if (mapPath != null)
                {
                    textures = new TexRef2D[]
                    {
                        new TexRef2D(Path.GetFileNameWithoutExtension(mapPath), mapPath.Contains(":") ? mapPath : dirPath + "\\" + mapPath ),
                    };
                }
                else
                    textures = new TexRef2D[0];

                RenderingParameters param = new RenderingParameters()
                {
                    Requirements = EUniformRequirements.None
                };
                return new TMaterial(objMat.Name, parameters, textures, shader);
            }
        }

        private static GLSLScript GetOBJFragmentShader(bool forward)
        {
            if (forward)
                return new GLSLScript(EGLSLType.Fragment, @"
#version 450

layout (location = 0) out vec4 OutColor;

layout (location = 0) in vec3 FragPos;
layout (location = 1) in vec3 FragNorm;
layout (location = 8) in vec2 FragUV0;

uniform vec3 Ambient;
uniform vec3 Diffuse;
uniform vec3 Specular;
uniform vec3 SpecularCoef;
uniform vec3 Transparency;

uniform sampler2D Texture0;

uniform vec3 CameraPosition;

" + ShaderHelpers.LightingDeclBasic() + @"

void main()
{
    vec3 normal = normalize(FragNorm);
    vec3 diffuseColor = texture(Texture0, FragUV0).rgb * Diffuse;

    " + ShaderHelpers.LightingCalcBasic("totalLight", "vec3(0.0)", "normal", "FragPos", "diffuseColor", "0.0", "1.0") + @"
    OutColor = vec4(diffuseColor * totalLight, 1.0);
}");
            else
                return new GLSLScript(EGLSLType.Fragment, @"
#version 450

layout (location = 0) out vec4 AlbedoSpec;
layout (location = 1) out vec3 Normal;
layout (location = 2) out vec4 RMSI;

layout (location = 0) in vec3 FragPos;
layout (location = 1) in vec3 FragNorm;
layout (location = 8) in vec2 FragUV0;

//uniform vec3 Ambient;
uniform vec3 Diffuse;
//uniform vec3 Specular;
//uniform vec3 SpecularCoef;
//uniform vec3 Transparency;

uniform sampler2D Texture0;

void main()
{
    AlbedoSpec = vec4(texture(Texture0, FragUV0).rgb * Diffuse, 0.0f);
    Normal = normalize(FragNorm);
    RMSI = vec4(1.0f, 0.0f, 1.0f, 0.0f);
}");
        }
    }
}
