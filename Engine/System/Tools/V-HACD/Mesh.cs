using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Tools.V_HACD
{
    public class Mesh
    {
        enum AXIS
        {
            AXIS_X = 0,
            AXIS_Y = 1,
            AXIS_Z = 2
        }
        struct Plane
        {
            public float m_a;
            public float m_b;
            public float m_c;
            public float m_d;
            public AXIS m_axis;
            public short m_index;
        }
#if VHACD_DEBUG_MESH
        struct Material
        {

            Vec33 m_diffuseColor;
            double m_ambientIntensity;
            Vec33 m_specularColor;
            Vec33 m_emissiveColor;
            double m_shininess;
            double m_transparency;
            Material(void)
            {
                m_diffuseColor.X() = 0.5;
                m_diffuseColor.Y() = 0.5;
                m_diffuseColor.Z() = 0.5;
                m_specularColor.X() = 0.5;
                m_specularColor.Y() = 0.5;
                m_specularColor.Z() = 0.5;
                m_ambientIntensity = 0.4;
                m_emissiveColor.X() = 0.0;
                m_emissiveColor.Y() = 0.0;
                m_emissiveColor.Z() = 0.0;
                m_shininess = 0.4;
                m_transparency = 0.0;
            }
        }
#endif // VHACD_DEBUG_MESH

        List<Vec3> m_points;
        List<Vec3> m_triangles;
        Vec3 m_minBB;
        Vec3 m_maxBB;
        Vec3 m_center;
        double m_diag;

        public Mesh()
        {
            m_diag = 1.0;
        }

        void AddPoint(Vec3 pt)
        {
            m_points.Add(pt);
        }
        void SetPoint(int index, Vec3 pt)
        {
            m_points[index] = pt;
        }
        Vec3 GetPoint(int index)
        {
            return m_points[index];
        }
        Vec3 GetPoint(int index)
        {
            return m_points[index];
        }
        int GetNPoints()
        {
            return m_points.Count;
        }
        List<Vec3> GetPoints()
        {
            return m_points;
        }
        void AddTriangle(IVec3 tri) 
        {
            m_triangles.Add(tri); 
        }
        void SetTriangle(int index, IVec3 tri) 
        {
            m_triangles[index] = tri;
        }
        IVec3 GetTriangle(int index)
        {
            return m_triangles[index];
        }
        IVec3 GetTriangle(int index)
        {
            return m_triangles[index];
        }
        int GetNTriangles()
        {
            return m_triangles.Count;
        }
        List<Triangle> GetTriangles()
        {
            return m_triangles;
        }
        Vec3 GetCenter()
        {
            return m_center;
        }
        Vec3 GetMinBB()
        {
            return m_minBB;
        }
        Vec3 GetMaxBB()
        {
            return m_maxBB;
        }
        void ClearPoints()
        {
            m_points.Clear();
        }
        void ClearTriangles()
        {
            m_triangles.Clear();
        }
        void Clear()
        {
            ClearPoints();
            ClearTriangles();
        }
        void ResizePoints(int nPts)
        {
            m_points.Resize(nPts);
        }
        void ResizeTriangles(int nTri)
        {
            m_triangles.Resize(nTri);
        }
        void CopyPoints(List<Vec3> points)
        {
            points = m_points;
        }
        float GetDiagBB()
        {
            return m_diag;
        }

        public double ComputeVolume()
        {
            int nV = GetNPoints();
            int nT = GetNTriangles();
            if (nV == 0 || nT == 0)
                return 0.0;

            Vec3 bary = new Vec3(0.0f);
            for (int v = 0; v < nV; v++)
                bary += GetPoint(v);
            bary /= nV;

            Vec3 ver0, ver1, ver2;
            double totalVolume = 0.0;
            for (int t = 0; t<nT; t++)
            {
                IVec3 tri = GetTriangle(t);
                ver0 = GetPoint(tri[0]);
                ver1 = GetPoint(tri[1]);
                ver2 = GetPoint(tri[2]);
                totalVolume += ComputeVolume4(ver0, ver1, ver2, bary);
            }
            return totalVolume / 6.0;
        }
        public void ComputeConvexHull(List<double> pts, int nPts)
        {
            ResizePoints(0);
            ResizeTriangles(0);
            ConvexHullComputer ch = new ConvexHullComputer();
            ch.Compute(pts, 3 * sizeof(double), nPts, -1.0f, -1.0f);
            for (int v = 0; v < ch.vertices.size(); v++)
                AddPoint(new Vec3(ch.vertices[v].getX(), ch.vertices[v].getY(), ch.vertices[v].getZ()));
            int nt = ch.faces.Count;
            for (int t = 0; t < nt; ++t)
            {
                ConvexHullComputer.Edge sourceEdge = ch.edges[ch.faces[t]];
                int a = sourceEdge.getSourceVertex();
                int b = sourceEdge.getTargetVertex();
                const ConvexHullComputer.Edge edge = sourceEdge.getNextEdgeOfFace();
                int c = edge->getTargetVertex();
                while (c != a)
                {
                    AddTriangle(new Vec3(a, b, c));
                    edge = edge->getNextEdgeOfFace();
                    b = c;
                    c = edge->getTargetVertex();
                }
            }
        }
        public void Clip(Plane plane, List<Vec3> positivePart, List<Vec3> negativePart)
        {
            int nV = GetNPoints();
            if (nV == 0)
                return;
            float d;
            for (int v = 0; v < nV; v++)
            {
                Vec3 pt = GetPoint(v);
                d = plane.m_a * pt[0] + plane.m_b * pt[1] + plane.m_c * pt[2] + plane.m_d;
                if (d > 0.0f)
                {
                    positivePart.Add(pt);
                }
                else if (d < 0.0f)
                {
                    negativePart.Add(pt);
                }
                else
                {
                    positivePart.Add(pt);
                    negativePart.Add(pt);
                }
            }
        }
        public bool IsInside(Vec3 pt)
        {
            int nV = GetNPoints();
            int nT = GetNTriangles();
            if (nV == 0 || nT == 0)
                return false;
            Vec3 ver0, ver1, ver2;
            float volume;
            for (int t = 0; t < nT; t++)
            {
                IVec3 tri = GetTriangle(t);
                ver0 = GetPoint(tri[0]);
                ver1 = GetPoint(tri[1]);
                ver2 = GetPoint(tri[2]);
                volume = ComputeVolume4(ver0, ver1, ver2, pt);
                if (volume < 0.0f)
                    return false;
            }
            return true;
        }
        public float ComputeDiagBB()
        {
            int nPoints = GetNPoints();
            if (nPoints == 0)
                return 0.0f;
            Vec3 minBB = m_points[0];
            Vec3 maxBB = m_points[0];
            float x, y, z;
            for (int v = 1; v < nPoints; v++)
            {
                x = m_points[v][0];
                y = m_points[v][1];
                z = m_points[v][2];
                if (x < minBB[0])
                    minBB[0] = x;
                else if (x > maxBB[0])
                    maxBB[0] = x;
                if (y < minBB[1])
                    minBB[1] = y;
                else if (y > maxBB[1])
                    maxBB[1] = y;
                if (z < minBB[2])
                    minBB[2] = z;
                else if (z > maxBB[2])
                    maxBB[2] = z;
            }
            return (m_diag = (maxBB - minBB).GetNorm());
        }

#if VHACD_DEBUG_MESH
        bool Mesh::SaveVRML2(const std::string& fileName) const
        {
            std::ofstream fout(fileName.c_str());
            if (fout.is_open()) {
                const Material material;
                if (SaveVRML2(fout, material)) {
                    fout.close();
                    return true;
                }
                return false;
            }
            return false;
        }
        bool Mesh::SaveVRML2(std::ofstream& fout, const Material& material) const
        {
            if (fout.is_open()) {
                fout.setf(std::ios::fixed, std::ios::floatfield);
        fout.setf(std::ios::showpoint);
                fout.precision(6);
                size_t nV = m_points.Size();
        size_t nT = m_triangles.Size();
        fout << "#VRML V2.0 utf8" << std::endl;
                fout << "" << std::endl;
                fout << "# Vertices: " << nV << std::endl;
                fout << "# Triangles: " << nT << std::endl;
                fout << "" << std::endl;
                fout << "Group {" << std::endl;
                fout << "    children [" << std::endl;
                fout << "        Shape {" << std::endl;
                fout << "            appearance Appearance {" << std::endl;
                fout << "                material Material {" << std::endl;
                fout << "                    diffuseColor " << material.m_diffuseColor[0] << " "
                     << material.m_diffuseColor[1] << " "
                     << material.m_diffuseColor[2] << std::endl;
                fout << "                    ambientIntensity " << material.m_ambientIntensity << std::endl;
                fout << "                    specularColor " << material.m_specularColor[0] << " "
                     << material.m_specularColor[1] << " "
                     << material.m_specularColor[2] << std::endl;
                fout << "                    emissiveColor " << material.m_emissiveColor[0] << " "
                     << material.m_emissiveColor[1] << " "
                     << material.m_emissiveColor[2] << std::endl;
                fout << "                    shininess " << material.m_shininess << std::endl;
                fout << "                    transparency " << material.m_transparency << std::endl;
                fout << "                }" << std::endl;
                fout << "            }" << std::endl;
                fout << "            geometry IndexedFaceSet {" << std::endl;
                fout << "                ccw TRUE" << std::endl;
                fout << "                solid TRUE" << std::endl;
                fout << "                convex TRUE" << std::endl;
                if (nV > 0) {
                    fout << "                coord DEF co Coordinate {" << std::endl;
                    fout << "                    point [" << std::endl;
                    for (size_t v = 0; v<nV; v++) {
                        fout << "                        " << m_points[v][0] << " "
                             << m_points[v][1] << " "
                             << m_points[v][2] << "," << std::endl;
                    }
                    fout << "                    ]" << std::endl;
                    fout << "                }" << std::endl;
                }
                if (nT > 0) {
                    fout << "                coordIndex [ " << std::endl;
                    for (size_t f = 0; f<nT; f++) {
                        fout << "                        " << m_triangles[f][0] << ", "
                             << m_triangles[f][1] << ", "
                             << m_triangles[f][2] << ", -1," << std::endl;
                    }
                    fout << "                ]" << std::endl;
                }
                fout << "            }" << std::endl;
                fout << "        }" << std::endl;
                fout << "    ]" << std::endl;
                fout << "}" << std::endl;
                return true;
            }
            return false;
        }
        bool Mesh::SaveOFF(const std::string& fileName) const
        {
            std::ofstream fout(fileName.c_str());
            if (fout.is_open()) {
                size_t nV = m_points.Size();
        size_t nT = m_triangles.Size();
        fout << "OFF" << std::endl;
                fout << nV << " " << nT << " " << 0 << std::endl;
                for (size_t v = 0; v<nV; v++) {
                    fout << m_points[v][0] << " "
                         << m_points[v][1] << " "
                         << m_points[v][2] << std::endl;
                }
                for (size_t f = 0; f<nT; f++) {
                    fout << "3 " << m_triangles[f][0] << " "
                         << m_triangles[f][1] << " "
                         << m_triangles[f][2] << std::endl;
                }
                fout.close();
                return true;
            }
            return false;
        }

        bool Mesh::LoadOFF(const std::string& fileName, bool invert)
        {
            FILE* fid = fopen(fileName.c_str(), "r");
            if (fid)
            {
                const std::string strOFF("OFF");
                char temp[1024];
                fscanf(fid, "%s", temp);
                if (std::string(temp) != strOFF) {
                    fclose(fid);
                    return false;
                }
                else {
                    int nv = 0;
                    int nf = 0;
                    int ne = 0;
                    fscanf(fid, "%i", &nv);
                    fscanf(fid, "%i", &nf);
                    fscanf(fid, "%i", &ne);
                    m_points.Resize(nv);
                    m_triangles.Resize(nf);
                    Vec33 coord;
                    float x, y, z;
                    for (int p = 0; p < nv; p++)
                    {
                        fscanf(fid, "%f", &x);
                        fscanf(fid, "%f", &y);
                        fscanf(fid, "%f", &z);
                        m_points[p][0] = x;
                        m_points[p][1] = y;
                        m_points[p][2] = z;
                    }
                    int i, j, k, s;
                    for (int t = 0; t < nf; ++t)
                    {
                        fscanf(fid, "%i", &s);
                        if (s == 3)
                        {
                            fscanf(fid, "%i", &i);
                            fscanf(fid, "%i", &j);
                            fscanf(fid, "%i", &k);
                            m_triangles[t][0] = i;
                            if (invert)
                            {
                                m_triangles[t][1] = k;
                                m_triangles[t][2] = j;
                            }
                            else
                            {
                                m_triangles[t][1] = j;
                                m_triangles[t][2] = k;
                            }
                        }
                        else // Fix me: support only triangular meshes
                        {
                            for (int h = 0; h < s; ++h)
                                fscanf(fid, "%i", &s);
                        }
                    }
                    fclose(fid);
                }
            }
            else
            {
                return false;
            }
            return true;
        }
#endif // VHACD_DEBUG_MESH
    }
}