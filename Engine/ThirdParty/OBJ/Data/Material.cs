using TheraEngine.Core.Maths.Transforms;

namespace ObjLoader.Loader.Data
{
    public class Material
    {
        public Material(string materialName)
        {
            Name = materialName;
        }

        public string Name { get; set; }

        public Vec3 AmbientColor { get; set; } = Vec3.Zero;
        public Vec3 DiffuseColor { get; set; } = Vec3.One;
        public Vec3 SpecularColor { get; set; } = Vec3.One;
        public float SpecularCoefficient { get; set; }

        public float Transparency { get; set; }

        public int IlluminationModel { get; set; }

        public string AmbientTextureMap { get; set; }
        public string DiffuseTextureMap { get; set; }
        
        public string SpecularTextureMap { get; set; }
        public string SpecularHighlightTextureMap { get; set; }
        
        public string BumpMap { get; set; }
        public string DisplacementMap { get; set; }
        public string StencilDecalMap { get; set; }

        public string AlphaTextureMap { get; set; }
    }
}