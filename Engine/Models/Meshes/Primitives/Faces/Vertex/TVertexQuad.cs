using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models
{
    public class TVertexQuad : TVertexPolygon
    {
        public TVertex Vertex0 => _vertices[0];
        public TVertex Vertex1 => _vertices[1];
        public TVertex Vertex2 => _vertices[2];
        public TVertex Vertex3 => _vertices[3];
        
        public override FaceType Type => FaceType.Quads;

        /// <summary>
        /// 3--2
        /// |\ |
        /// | \|
        /// 0--1
        /// </summary>
        public TVertexQuad(TVertex v0, TVertex v1, TVertex v2, TVertex v3) 
            : base(v0, v1, v2, v3) { }

        /// <summary>      
        /// 3--2
        /// | /|
        /// |/ |
        /// 0--1
        /// Order: 012 023
        /// </summary>
        /// <returns></returns>
        //public override VertexTriangle[] ToTriangles()
        //{
        //    return new VertexTriangle[]
        //    {
        //        new VertexTriangle(Vertex0.HardCopy(), Vertex1.HardCopy(), Vertex2.HardCopy()),
        //        new VertexTriangle(Vertex0.HardCopy(), Vertex2.HardCopy(), Vertex3.HardCopy()),
        //    };
        //}
        public static TVertexQuad Make(
            Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, bool addAutoNormal = false, bool flipVerticalUVCoord = false)
        {
            if (addAutoNormal)
            {
                Vec3 normal = Vec3.CalculateNormal(bottomLeft, bottomRight, topLeft);
                return Make(bottomLeft, bottomRight, topRight, topLeft, normal, flipVerticalUVCoord);
            }
            else
                return new TVertexQuad(
                    new TVertex(bottomLeft,  new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(bottomRight, new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(topRight,    new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                    new TVertex(topLeft,     new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
        }
        public static TVertexQuad Make(
            Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, Vec3 normal, bool flipVerticalUVCoord = true)
            => new TVertexQuad(
                new TVertex(bottomLeft,  normal, new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(bottomRight, normal, new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(topRight,    normal, new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                new TVertex(topLeft,     normal, new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
        
        /// <summary>
        /// Generates a quad using cubemap-cross texture coordinates.
        /// </summary>
        /// <param name="bottomLeft">The bottom left position of the quad.</param>
        /// <param name="bottomRight">The bottom right position of the quad.</param>
        /// <param name="topRight">The top right position of the quad.</param>
        /// <param name="topLeft">The top left position of the quad.</param>
        /// <param name="normal">The normal value for the quad.</param>
        /// <param name="cubeMapFace">The face to retrieve UV coordinates for.</param>
        /// <param name="widthLarger">If the cubemap cross texture has a width larger than height for a sideways-oriented cross.
        /// Assumes +Y and -Y are on the left half of the image (top of the cross is on the left side).</param>
        /// <param name="bias">How much to shrink the UV coordinates inward into the cross sections
        /// to avoid sampling from the empty parts of the image.
        /// A value of 0 means exact coordinates.</param>
        /// <param name="flipVerticalUVCoord">If true, flips the vertical coordinate upside-down. 
        /// This is true by default because OpenGL uses a top-left UV origin.</param>
        /// <returns>A <see cref="TVertexQuad"/> object defining a quad.</returns>
        public static TVertexQuad Make(
            Vec3 bottomLeft,
            Vec3 bottomRight,
            Vec3 topRight,
            Vec3 topLeft, 
            Vec3 normal,
            ECubemapFace cubeMapFace,
            bool widthLarger,
            float bias = 0.0f,
            bool flipVerticalUVCoord = true)
        {
            Vec2 
                bottomLeftUV, 
                bottomRightUV, 
                topRightUV, 
                topLeftUV;
            
            const float zero = 0.0f;
            const float fourth = 0.25f;
            const float half = 0.5f;
            const float threeFourths = 0.75f;
            const float third = (float)(1.0 / 3.0);
            const float twoThirds = (float)(2.0 / 3.0);
            const float one = 1.0f;

            switch (cubeMapFace)
            {
                case ECubemapFace.NegX:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(zero, third);
                        bottomRightUV = new Vec2(fourth, third);
                        topRightUV = new Vec2(fourth, twoThirds);
                        topLeftUV = new Vec2(zero, twoThirds);
                    }
                    else
                    {
                        bottomLeftUV = new Vec2(zero, half);
                        bottomRightUV = new Vec2(third, half);
                        topRightUV = new Vec2(third, threeFourths);
                        topLeftUV = new Vec2(zero, threeFourths);
                    }
                    break;
                case ECubemapFace.PosX:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(half, third);
                        bottomRightUV = new Vec2(threeFourths, third);
                        topRightUV = new Vec2(threeFourths, twoThirds);
                        topLeftUV = new Vec2(half, twoThirds);
                    }
                    else
                    {
                        bottomLeftUV = new Vec2(twoThirds, half);
                        bottomRightUV = new Vec2(one, half);
                        topRightUV = new Vec2(one, threeFourths);
                        topLeftUV = new Vec2(twoThirds, threeFourths);
                    }
                    break;
                case ECubemapFace.NegY:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(fourth, zero);
                        bottomRightUV = new Vec2(half, zero);
                        topRightUV = new Vec2(half, third);
                        topLeftUV = new Vec2(fourth, third);
                    }
                    else
                    {
                        bottomLeftUV = new Vec2(third, fourth);
                        bottomRightUV = new Vec2(twoThirds, fourth);
                        topRightUV = new Vec2(twoThirds, half);
                        topLeftUV = new Vec2(third, half);
                    }
                    break;
                case ECubemapFace.PosY:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(fourth, twoThirds);
                        bottomRightUV = new Vec2(half, twoThirds);
                        topRightUV = new Vec2(half, one);
                        topLeftUV = new Vec2(fourth, one);
                    }
                    else
                    {
                        bottomLeftUV = new Vec2(third, threeFourths);
                        bottomRightUV = new Vec2(twoThirds, threeFourths);
                        topRightUV = new Vec2(twoThirds, one);
                        topLeftUV = new Vec2(third, one);
                    }
                    break;
                case ECubemapFace.NegZ:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(fourth, third);
                        bottomRightUV = new Vec2(half, third);
                        topRightUV = new Vec2(half, twoThirds);
                        topLeftUV = new Vec2(fourth, twoThirds);
                    }
                    else
                    {
                        bottomLeftUV = new Vec2(third, half);
                        bottomRightUV = new Vec2(twoThirds, half);
                        topRightUV = new Vec2(twoThirds, threeFourths);
                        topLeftUV = new Vec2(third, threeFourths);
                    }
                    break;
                case ECubemapFace.PosZ:
                    if (widthLarger)
                    {
                        bottomLeftUV = new Vec2(threeFourths, third);
                        bottomRightUV = new Vec2(one, third);
                        topRightUV = new Vec2(one, twoThirds);
                        topLeftUV = new Vec2(threeFourths, twoThirds);
                    }
                    else
                    {
                        //Upside-down UVs
                        bottomLeftUV = new Vec2(third, fourth);
                        bottomRightUV = new Vec2(twoThirds, fourth);
                        topRightUV = new Vec2(twoThirds, zero);
                        topLeftUV = new Vec2(third, zero);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cubeMapFace), cubeMapFace, null);
            }

            bottomLeftUV.X += bias;
            topLeftUV.X += bias;
            bottomRightUV.X -= bias;
            topRightUV.X -= bias;

            bottomLeftUV.Y += bias;
            bottomRightUV.Y += bias;
            topLeftUV.Y -= bias;
            topRightUV.Y -= bias;

            if (flipVerticalUVCoord)
            {
                bottomLeftUV.Y = 1.0f - bottomLeftUV.Y;
                bottomRightUV.Y = 1.0f - bottomRightUV.Y;
                topRightUV.Y = 1.0f - topRightUV.Y;
                topLeftUV.Y = 1.0f - topLeftUV.Y;
            }

            return new TVertexQuad(
                new TVertex(bottomLeft, normal, bottomLeftUV),
                new TVertex(bottomRight, normal, bottomRightUV),
                new TVertex(topRight, normal, topRightUV),
                new TVertex(topLeft, normal, topLeftUV));
        }
        public static TVertexQuad Make(
            Vec3 bottomLeft,    InfluenceDef bottomLeftInf,
            Vec3 bottomRight,   InfluenceDef bottomRightInf,
            Vec3 topRight,      InfluenceDef topRightInf,
            Vec3 topLeft,       InfluenceDef topLeftInf,
            Vec3 normal, bool flipVerticalUVCoord = false)
        {
            return new TVertexQuad(
                new TVertex(bottomLeft,  bottomLeftInf,  normal, new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(bottomRight, bottomRightInf, normal, new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(topRight,    topRightInf,    normal, new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                new TVertex(topLeft,     topLeftInf,     normal, new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
        }
        public static TVertexQuad Make(
           Vec3 bottomLeft,     InfluenceDef bottomLeftInf,    Vec3 bottomLeftNormal,
           Vec3 bottomRight,    InfluenceDef bottomRightInf,   Vec3 bottomRightNormal,
           Vec3 topRight,       InfluenceDef topRightInf,      Vec3 topRightNormal,
           Vec3 topLeft,        InfluenceDef topLeftInf,       Vec3 topLeftNormal, bool flipVerticalUVCoord = false)
        {
            return new TVertexQuad(
                new TVertex(bottomLeft,  bottomLeftInf,  bottomLeftNormal,   new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(bottomRight, bottomRightInf, bottomRightNormal,  new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                new TVertex(topRight,    topRightInf,    topRightNormal,     new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                new TVertex(topLeft,     topLeftInf,     topLeftNormal,      new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
        }

        public static TVertexQuad Make(
           Vec3 bottomLeft, InfluenceDef bottomLeftInf,
           Vec3 bottomRight, InfluenceDef bottomRightInf,
           Vec3 topRight, InfluenceDef topRightInf,
           Vec3 topLeft, InfluenceDef topLeftInf,
           bool addAutoNormal = false, bool flipVerticalUVCoord = false)
        {
            if (addAutoNormal)
            {
                Vec3 normal = Vec3.CalculateNormal(bottomLeft, bottomRight, topLeft);
                return new TVertexQuad(
                    new TVertex(bottomLeft,  bottomLeftInf,  normal, new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(bottomRight, bottomRightInf, normal, new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(topRight,    topRightInf,    normal, new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                    new TVertex(topLeft,     topLeftInf,     normal, new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
            }
            else
                return new TVertexQuad(
                    new TVertex(bottomLeft,  bottomLeftInf,  new Vec2(0.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(bottomRight, bottomRightInf, new Vec2(1.0f, flipVerticalUVCoord ? 1.0f : 0.0f)),
                    new TVertex(topRight,    topRightInf,    new Vec2(1.0f, flipVerticalUVCoord ? 0.0f : 1.0f)),
                    new TVertex(topLeft,     topLeftInf,     new Vec2(0.0f, flipVerticalUVCoord ? 0.0f : 1.0f)));
        }

        /// <summary>
        /// Positive Y is facing the sky, like a floor.
        /// </summary>
        public static TVertexQuad PosY(float uniformScale = 1.0f, bool bottomLeftOrigin = false, bool flipVerticalUVCoord = false) 
            => PosY(uniformScale, uniformScale, bottomLeftOrigin, flipVerticalUVCoord);
        /// <summary>
        /// Positive Y is facing the sky, like a floor.
        /// </summary>
        public static TVertexQuad PosY(float xScale, float zScale, bool bottomLeftOrigin, bool flipVerticalUVCoord = false)
        {
            if (bottomLeftOrigin)
            {
                Vec3 v1 = new Vec3(0.0f,    0.0f, 0.0f);
                Vec3 v2 = new Vec3(xScale,  0.0f, 0.0f);
                Vec3 v3 = new Vec3(xScale,  0.0f, -zScale);
                Vec3 v4 = new Vec3(0.0f,    0.0f, -zScale);
                return Make(v1, v2, v3, v4, Vec3.UnitY, flipVerticalUVCoord);
            }
            else
            {
                float xHalf = xScale / 2.0f;
                float zHalf = zScale / 2.0f;
                Vec3 v1 = new Vec3(-xHalf,  0.0f, zHalf);
                Vec3 v2 = new Vec3(xHalf,   0.0f, zHalf);
                Vec3 v3 = new Vec3(xHalf,   0.0f, -zHalf);
                Vec3 v4 = new Vec3(-xHalf,  0.0f, -zHalf);
                return Make(v1, v2, v3, v4, Vec3.UnitY, flipVerticalUVCoord);
            }
        }
        /// <summary>
        /// Positive Y is facing the camera, like a wall.
        /// </summary>
        public static TVertexQuad PosY(BoundingRectangleF region, bool flipVerticalUVCoord = false)
        {
            return Make(
                ((Vec3)region.BottomLeft).Xzy,
                ((Vec3)region.BottomRight).Xzy,
                ((Vec3)region.TopRight).Xzy,
                ((Vec3)region.TopLeft).Xzy,
                Vec3.UnitZ, flipVerticalUVCoord);
        }

        /// <summary>
        /// Positive Z is facing the camera, like a wall.
        /// </summary>
        public static TVertexQuad PosZ(float uniformScale = 1.0f, bool bottomLeftOrigin = false, float z = 0.0f, bool flipVerticalUVCoord = true)
            => PosZ(uniformScale, uniformScale, z, bottomLeftOrigin, flipVerticalUVCoord);
        /// <summary>
        /// Positive Z is facing the camera, like a wall.
        /// </summary>
        public static TVertexQuad PosZ(float xScale, float yScale, float z, bool bottomLeftOrigin, bool flipVerticalUVCoord = true)
        {
            if (bottomLeftOrigin)
            {
                Vec3 v1 = new Vec3(0.0f,    0.0f,   z);
                Vec3 v2 = new Vec3(xScale,  0.0f,   z);
                Vec3 v3 = new Vec3(xScale,  yScale, z);
                Vec3 v4 = new Vec3(0.0f,    yScale, z);
                return Make(v1, v2, v3, v4, Vec3.UnitZ, flipVerticalUVCoord);
            }
            else
            {
                float xHalf = xScale / 2.0f;
                float yHalf = yScale / 2.0f;
                Vec3 v1 = new Vec3(-xHalf,  -yHalf, z);
                Vec3 v2 = new Vec3(xHalf,   -yHalf, z);
                Vec3 v3 = new Vec3(xHalf,   yHalf,  z);
                Vec3 v4 = new Vec3(-xHalf,  yHalf,  z);
                return Make(v1, v2, v3, v4, Vec3.UnitZ, flipVerticalUVCoord);
            }
        }
        /// <summary>
        /// Positive Z is facing the camera, like a wall.
        /// </summary>
        public static TVertexQuad PosZ(BoundingRectangleF region, bool flipVerticalUVCoord = false)
            => Make(
                region.BottomLeft,
                region.BottomRight,
                region.TopRight,
                region.TopLeft,
                Vec3.UnitZ, flipVerticalUVCoord);

        /// <summary>
        /// Negative Z is away from the camera.
        /// </summary>
        public static TVertexQuad NegZ(float uniformScale = 1.0f, bool bottomLeftOrigin = false, bool flipVerticalUVCoord = false)
            => NegZ(uniformScale, uniformScale, bottomLeftOrigin, flipVerticalUVCoord);
        /// <summary>
        /// Negative Z is away from the camera.
        /// </summary>
        public static TVertexQuad NegZ(float xScale, float yScale, bool bottomLeftOrigin, bool flipVerticalUVCoord = false)
        {
            if (bottomLeftOrigin)
            {
                Vec3 v1 = new Vec3(0.0f,    0.0f,   0.0f);
                Vec3 v2 = new Vec3(-xScale, 0.0f,   0.0f);
                Vec3 v3 = new Vec3(-xScale, yScale, 0.0f);
                Vec3 v4 = new Vec3(0.0f,    yScale, 0.0f);
                return Make(v1, v2, v3, v4, -Vec3.UnitZ, flipVerticalUVCoord);
            }
            else
            {
                float xHalf = xScale / 2.0f;
                float yHalf = yScale / 2.0f;
                Vec3 v1 = new Vec3(xHalf,   -yHalf, 0.0f);
                Vec3 v2 = new Vec3(-xHalf,  -yHalf, 0.0f);
                Vec3 v3 = new Vec3(-xHalf,  yHalf,  0.0f);
                Vec3 v4 = new Vec3(xHalf,   yHalf,  0.0f);
                return Make(v1, v2, v3, v4, -Vec3.UnitZ, flipVerticalUVCoord);
            }
        }
    }
}
