using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Xml;
using System.Runtime.InteropServices;
using System.IO;

namespace CustomEngine.Rendering.Models
{
    public class Skeleton : FileObject, IEnumerable<Bone>, IRenderable
    {
        public override ResourceType ResourceType { get { return ResourceType.Skeleton; } }

        public Skeleton() : base() { }
        public Skeleton(params Bone[] rootBones) : base()
        {
            RootBones = rootBones;
            foreach (Bone b in RootBones)
            {
                b.CalcBindMatrix(true);
                b.CalcFrameMatrix();
            }
            RegenerateBoneCache();
        }
        public Skeleton(Bone rootBone) : base()
        {
            RootBones = new Bone[1] { rootBone };
            rootBone.CalcBindMatrix(true);
            rootBone.CalcFrameMatrix();
            RegenerateBoneCache();
        }

        public Bone this[string name]
        {
            get { return BoneNameCache.ContainsKey(name) ? BoneNameCache[name] : null; }
        }
        public Bone this[int index]
        {
            get { return BoneIndexCache.ContainsKey(index) ? BoneIndexCache[index] : null; }
        }

        private Dictionary<string, Bone> _boneNameCache = new Dictionary<string, Bone>();
        private Dictionary<int, Bone> _boneIndexCache = new Dictionary<int, Bone>();
        
        private SkeletalMeshComponent _owningComponent;
        private Bone[] _rootBones;

        public Bone[] RootBones
        {
            get => _rootBones;
            set
            {
                _rootBones = value;
                RegenerateBoneCache();
            }
        }
        public Dictionary<string, Bone> BoneNameCache => _boneNameCache;
        public Dictionary<int, Bone> BoneIndexCache => _boneIndexCache;
        public SkeletalMeshComponent OwningComponent
        {
            get => _owningComponent;
            set => _owningComponent = value;
        }

        public Bone GetBone(string boneName)
        {
            if (!_boneNameCache.ContainsKey(boneName))
                return RootBones[0];
            return _boneNameCache[boneName];
        }

        public void RegenerateBoneCache()
        {
            _boneNameCache.Clear();
            _boneIndexCache.Clear();
            foreach (Bone b in RootBones)
                b.CollectChildBones(this);
        }
        public void CalcFrameMatrices()
        {
            if (Engine._engineSettings.File.SkinOnGPU)
            {
                foreach (Bone b in RootBones)
                    b.CalcFrameMatrix();
            }
            else
            {
                HashSet<int> modifiedVertexIndices = new HashSet<int>();
                HashSet<int> modifiedBoneIndices = new HashSet<int>();
                foreach (Bone b in RootBones)
                    b.CalcFrameMatrix(modifiedVertexIndices, modifiedBoneIndices);
                //foreach (FacePoint point in modified)
                //    point.UpdatePNTB();
            }
        }

        public IEnumerator<Bone> GetEnumerator() { return ((IEnumerable<Bone>)_boneNameCache.Values).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Bone>)_boneNameCache.Values).GetEnumerator(); }

        bool _visible, _rendering;
        Shape _cullingVolume = new Sphere(1.0f);
        RenderOctree.Node _renderNode;

        public Shape CullingVolume => _cullingVolume;
        public bool IsRendering
        {
            get => _rendering;
            set => _rendering = value;
        }
        public RenderOctree.Node RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        public void Render()
        {
            foreach (Bone b in BoneNameCache.Values)
            {
                Vec3 point = b.WorldMatrix.GetPoint();
                Engine.Renderer.RenderPoint(b.Name + "_Pos", point, 15.0f, b.Parent == null ? Color.Orange : Color.Purple);
                if (b.Parent != null)
                    Engine.Renderer.RenderLine(b.Name + "_Parent", point, b.Parent.WorldMatrix.GetPoint(), 5.0f, Color.Blue);
                float scale = Engine.Renderer.Scene.CurrentCamera.DistanceScale(point, 2.0f);
                Engine.Renderer.RenderLine(b.Name + "_Up", point, Vec3.TransformPosition(Vec3.Up * scale, b.WorldMatrix), 5.0f, Color.Red);
                Engine.Renderer.RenderLine(b.Name + "_Right", point, Vec3.TransformPosition(Vec3.Right * scale, b.WorldMatrix), 5.0f, Color.Green);
                Engine.Renderer.RenderLine(b.Name + "_Forward", point, Vec3.TransformPosition(Vec3.Forward * scale, b.WorldMatrix), 5.0f, Color.Blue);
            }
        }
        public class LiveInfluence
        {
            public int _weightCount;
            public Bone[] _bones = new Bone[4];
            public float[] _weights = new float[4];
            public static LiveInfluence FromInfluence(Influence inf, Skeleton skel)
            {
                LiveInfluence f = new LiveInfluence()
                {
                    _weightCount = inf.WeightCount
                };
                for (int i = 0; i < inf.WeightCount; ++i)
                {
                    BoneWeight w = inf.Weights[i];
                    f._weights[i] = w.Weight;
                    f._bones[i] = skel[w.Bone];
                }
                return f;
            }
        }

        /// <summary>
        /// Precompute pairings of vertex and bone indices with manager for each bone.
        /// </summary>
        public void GenerateInfluences(PrimitiveManager manager)
        {
            //if (manager.Data._influences != null)
            //    for (int i = 0; i < manager.Data._influences.Length; ++i)
            //    {
            //        LiveInfluence inf = LiveInfluence.FromInfluence(manager.Data._influences[i], this);
            //        FacePoint point = manager.Data._facePoints[i];
            //        for (int j = 0; j < inf.WeightCount; ++j)
            //        {
            //            Bone b = _boneNameCache[inf.Weights[j].Bone];
            //            b._influencedVertices.Add(point);
            //        }
            //    }
        }
        public void ClearInfluences(PrimitiveManager manager)
        {
            for (int i = 0; i < manager.Data._influences.Length; ++i)
            {
                Influence inf = manager.Data._influences[i];
                FacePoint point = manager.Data._facePoints[i];
                for (int j = 0; j < inf.WeightCount; ++j)
                {
                    Bone b = _boneNameCache[inf.Weights[j].Bone];
                    b._influencedVertices.Remove(point);
                }
            }
        }

        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("skeleton", true))
                throw new Exception();
            _rootBones = null;
            _boneNameCache = new Dictionary<string, Bone>();
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("childCount", true))
                    _rootBones = new Bone[int.Parse((string)reader.Value)];
            }
            int index = 0;
            while (reader.BeginElement())
            {
                if (reader.Name.Equals("bone", true))
                {
                    Bone b = new Bone(this);
                    b.Read(reader);
                    _rootBones[index++] = b;
                }
                reader.EndElement();
            }
        }
        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header* h = (Header*)address;
            int boneCount = h->_boneCount;
            Bone[] bones = new Bone[boneCount];
            List<Bone> rootBones = new List<Bone>();
            //int[] parentIndices = new int[boneCount];
            for (int i = 0; i < boneCount; ++i)
            {
                Bone.Header* hdr = &h->Bones[i];
                Bone b = new Bone();
                b.Read(hdr, strings);
                int parentIndex = hdr->_parentIndex;
                if (parentIndex < 0)
                    rootBones.Add(b);
                else
                    //Avoid a second loop by assuming the parent was written before the child
                    b.Parent = bones[parentIndex];
                bones[i] = b;
                //parentIndices[i] = hdr->_parentIndex;
            }
            //for (int i = 0; i < boneCount; ++i)
            //    bones[i].Parent = bones[parentIndices[i]];
            CalcFrameMatrices();
        }
        public override void Write(VoidPtr address, StringTable table)
        {

        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("skeleton");
            writer.WriteAttributeString("totalCount", _boneNameCache.Count.ToString());
            writer.WriteAttributeString("childCount", _rootBones.Length.ToString());
            foreach (Bone b in _rootBones)
                b.Write(writer);
            writer.WriteEndElement();
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct Header
        {
            public bint _boneCount;

            public Bone.Header* Bones { get { return (Bone.Header*)Address; } }
            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
    }
}
