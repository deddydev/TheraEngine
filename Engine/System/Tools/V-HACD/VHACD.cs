using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Tools.VHACD
{
    public static class VHACD
    {
        [DllImport("hacd.dll")]
        static public extern VoidPtr CreateVHACD();
        
        public class ConvexHull
        {
            Vec3 m_points;
            IVec3[] m_triangles;
            uint m_nPoints;
            uint m_nTriangles;
        }

        public class Parameters
        {
            Parameters() { Init(); }
            public void Init()
            {
                m_resolution = 100000;
                m_depth = 20;
                m_concavity = 0.001;
                m_planeDownsampling = 4;
                m_convexhullDownsampling = 4;
                m_alpha = 0.05;
                m_beta = 0.05;
                m_gamma = 0.0005;
                m_pca = 0;
                m_mode = 0; // 0: voxel-based (recommended), 1: tetrahedron-based
                m_maxNumVerticesPerCH = 64;
                m_minVolumePerCH = 0.0001;
                m_convexhullApproximation = 1;
                m_oclAcceleration = 1;
            }
            double m_concavity;
            double m_alpha;
            double m_beta;
            double m_gamma;
            double m_minVolumePerCH;
            VoidPtr m_callback;
            VoidPtr m_logger;
            uint m_resolution;
            uint m_maxNumVerticesPerCH;
            int m_depth;
            int m_planeDownsampling;
            int m_convexhullDownsampling;
            int m_pca;
            int m_mode;
            int m_convexhullApproximation;
            int m_oclAcceleration;
        };

        void Cancel();
    virtual bool Compute(const float* const points,
        const unsigned int stridePoints,
        const unsigned int countPoints,
        const int* const triangles,
        const unsigned int strideTriangles,
        const unsigned int countTriangles,
        const Parameters& params)
        = 0;
        virtual bool Compute(const double* const points,
        const unsigned int stridePoints,
        const unsigned int countPoints,
        const int* const triangles,
        const unsigned int strideTriangles,
        const unsigned int countTriangles,
        const Parameters& params)
        = 0;
        virtual unsigned int GetNConvexHulls() const = 0;
        virtual void GetConvexHull(const unsigned int index, ConvexHull& ch) const = 0;
        virtual void Clean(void) = 0; // release internally allocated memory
    virtual void Release(void) = 0; // release IVHACD
    virtual bool OCLInit(void* const oclDevice,
        IUserLogger* const logger = 0)
        = 0;
        virtual bool OCLRelease(IUserLogger* const logger = 0) = 0;

        protected:
    virtual ~IVHACD(void) {}
    }
}
