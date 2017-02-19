using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

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
            get { return BoneCache.ContainsKey(name) ? BoneCache[name] : null; }
        }

        public Dictionary<string, Bone> BoneCache
        {
            get { return _boneCache; }
        }

        private Dictionary<string, Bone> _boneCache = new Dictionary<string, Bone>();
        private SkeletalMeshComponent _owningComponent;
        private Bone[] _rootBones;
        public Bone[] RootBones
        {
            get { return _rootBones; }
            set
            {
                _rootBones = value;
                RegenerateBoneCache();
            }
        }
        public SkeletalMeshComponent OwningComponent
        {
            get { return _owningComponent; }
            set { _owningComponent = value; }
        }

        public Bone GetBone(string boneName)
        {
            if (!_boneCache.ContainsKey(boneName))
                return RootBones[0];
            return _boneCache[boneName];
        }

        public void RegenerateBoneCache()
        {
            _boneCache.Clear();
            foreach (Bone b in RootBones)
                b.CollectChildBones(_boneCache, this);
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
                HashSet<FacePoint> modified = new HashSet<FacePoint>();
                foreach (Bone b in RootBones)
                    b.CalcFrameMatrix(modified);
                //foreach (FacePoint point in modified)
                //    point.UpdatePNTB();
            }
        }
        internal override void Tick(float delta)
        {
            base.Tick(delta);
        }

        public IEnumerator<Bone> GetEnumerator() { return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Bone>)_boneCache.Values).GetEnumerator(); }

        bool _visible, _rendering;
        Shape _cullingVolume = new Sphere(1.0f);
        RenderOctree.Node _renderNode;

        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool IsRendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        public RenderOctree.Node RenderNode
        {
            get { return _renderNode; }
            set { _renderNode = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Render()
        {
            foreach (Bone b in BoneCache.Values)
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
        private class LiveInfluence
        {
            static void FromInfluence(Influence inf)
            {

            }
        }
        public void GenerateInfluences(PrimitiveManager manager)
        {
            if (manager.Data._influences != null)
                for (int i = 0; i < manager.Data._influences.Length; ++i)
                {
                    Influence inf = manager.Data._influences[i];
                    FacePoint point = manager.Data._facePoints[i];
                    for (int j = 0; j < inf.WeightCount; ++j)
                    {
                        Bone b = _boneCache[inf.Weights[j].Bone];
                        b._influencedVertices.Add(point);
                    }
                }
        }
        public void ClearInfluences(PrimitiveManager manager)
        {
            for (int i = 0; i < manager.Data._influences.Length; ++i)
            {
                Influence inf = manager.Data._influences[i];
                FacePoint point = manager.Data._facePoints[i];
                for (int j = 0; j < inf.WeightCount; ++j)
                {
                    Bone b = _boneCache[inf.Weights[j].Bone];
                    b._influencedVertices.Remove(point);
                }
            }
        }
    }
}
