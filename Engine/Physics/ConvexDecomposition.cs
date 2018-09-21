using BulletSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Physics
{
    public static class ConvexDecomposition
    {
        public static TCollisionCompoundShape Calculate(
            IEnumerable<PrimitiveData> primitives,
            int minClusterCount = 2,
            int maxConcavity = 50,
            int maxVerticesPerHull = 20,
            double volumeWeight = 0.0,
            double compacityWeight = 0.1,
            bool addExtraDistPoints = false,
            bool addNeighborsDistPoints = false,
            bool addFacesPoints = false)
        {
            //TODO: finish hacd convex decomposition
            return null;

            Hacd hacd = new Hacd();
            List<int> indices = new List<int>();
            List<Vector3> points = new List<Vector3>();
            int[] meshIndices;
            int baseIndexOffset = 0;
            foreach (PrimitiveData primData in primitives)
            {
                primData[Rendering.Models.EBufferType.Position].GetData(out Vec3[] array, false);
                meshIndices = primData.GetIndices();
                foreach (int i in meshIndices)
                    indices.Add(i + baseIndexOffset);
                baseIndexOffset += array.Length;
                points.AddRange(array.Select(x => (Vector3)x));
            }

            hacd.SetPoints(points);
            hacd.SetTriangles(indices);
            hacd.CompacityWeight = compacityWeight;
            hacd.VolumeWeight = volumeWeight;
            hacd.CallBack = HacdUpdate;

            // Recommended HACD parameters: 2 100 false false false
            hacd.NClusters = minClusterCount; // minimum number of clusters
            hacd.Concavity = maxConcavity; // maximum concavity
            hacd.AddExtraDistPoints = addExtraDistPoints;
            hacd.AddNeighboursDistPoints = addNeighborsDistPoints;
            hacd.AddFacesPoints = addFacesPoints;
            hacd.NumVerticesPerConvexHull = maxVerticesPerHull; // max of 100 vertices per convex-hull

            hacd.Compute();
            
            var shapes = new(Matrix4 localTransform, TCollisionShape shape)[hacd.NClusters];
            for (int c = 0; c < hacd.NClusters; c++)
            {
                hacd.GetCH(c, out Vector3[] meshPoints, out int[] triangles);

                Vector3 centroid = Vector3.Zero;
                foreach (Vector3 vertex in meshPoints)
                    centroid += vertex;
                centroid /= meshPoints.Length;
                for (int i = 0; i < meshPoints.Length; ++i)
                    meshPoints[i] -= centroid;

                TCollisionConvexHull convexShape = TCollisionConvexHull.New(meshPoints.Select(x => new Vec3(x.X, x.Y, x.Z)));
                shapes[c] = (Matrix.Translation(centroid), convexShape);
            }

            return TCollisionCompoundShape.New(shapes);
        }
        private static bool HacdUpdate(string msg, double progress, double globalConcativity, int n)
        {
            Engine.PrintLine(msg);
            return true;
        }
    }
}
