using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Mesh
{
    public class RenderableLOD : TObject
    {
        public float VisibleDistance { get; set; }
        public PrimitiveManager Manager { get; set; }

        public ShaderVar[] Parameters => Manager.Material.Parameters;
    }
    /// <summary>
    /// Mesh generated at runtime for internal use.
    /// </summary>
    public abstract class BaseRenderableMesh : I3DRenderable
    {
        public BaseRenderableMesh(List<LOD> lods, RenderInfo3D renderInfo, SceneComponent component)
        {
            _component = component;

            LODs = new LinkedList<RenderableLOD>(lods.Select(x => new RenderableLOD()
            {
                VisibleDistance = x.VisibleDistance,
                Manager = x.CreatePrimitiveManager(),
            }));

            _currentLOD = LODs.Last;

            RenderInfo = renderInfo;
            RenderInfo.RenderOrderFunc = GetRenderDistance;

            Visible = false;
        }

        private bool _visible = false;
        protected SceneComponent _component;
        protected LinkedListNode<RenderableLOD> _currentLOD;

        public LinkedList<RenderableLOD> LODs { get; private set; }
        public RenderInfo3D RenderInfo { get; protected set; }

        [Browsable(false)]
        public virtual Shape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;
                _visible = value;
                if (_visible)
                    _component?.OwningScene?.Add(this);
                else
                    _component?.OwningScene?.Remove(this);
            }
        }
        public bool VisibleInEditorOnly { get; set; } = false;
        public bool HiddenFromOwner { get; set; } = false;
        public bool VisibleToOwnerOnly { get; set; } = false;

        private float GetRenderDistance(bool shadowPass)
        {
            float dist = GetDistance(AbstractRenderer.CurrentCamera);

            if (!shadowPass)
                UpdateLOD(dist);

            return dist;
        }

        public float GetDistance(Camera camera)
        {
            Vec3 camPoint = camera == null ? Vec3.Zero : camera.WorldPoint;
            Vec3 meshPoint = (_component != null ? _component.WorldMatrix.GetPoint() : Vec3.Zero);
            return meshPoint.DistanceToFast(camPoint);
        }

        private void UpdateLOD(float viewDist)
        {
            while (true)
            {
                if (_currentLOD.Value.Manager == null)
                    break;
                if (viewDist < _currentLOD.Value.VisibleDistance)
                {
                    if (_currentLOD.Previous != null)
                        _currentLOD = _currentLOD.Previous;
                    else
                        break;
                }
                else
                {
                    if (_currentLOD.Next != null && viewDist >= _currentLOD.Next.Value.VisibleDistance)
                        _currentLOD = _currentLOD.Next;
                    else
                        break;
                }
            }

            //Start with the lowest, farthest away LOD and work toward higher quality
            //Most renderables will be farther rather than closer, so this is fastest
            //for (int i = LODs.Length - 1; i >= 0; --i)
            //{
            //    _currentLOD = LODs[i];
            //    if (_currentLOD?.Manager == null || viewDist >= _currentLOD.VisibleDistance)
            //        break;
            //}

            //Visible = _currentLOD.Value.Manager != null;
        }
        public void Render()
        {
            //Visible will be set to false if the current lod or its manager is null
            //Therefore this code will never be run in those circumstances
            _currentLOD.Value.Manager?.Render(_component.WorldMatrix, _component.InverseWorldMatrix.Transposed().GetRotationMatrix3());
        }
    }
    public class StaticRenderableMesh : BaseRenderableMesh
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
        
        public StaticRenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            : base(mesh.LODs, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            SetCullingVolume(mesh.CullingVolume);

            //PrimitiveManager m = LODs.First.Value.Manager;
            //if (m.Data.BufferInfo.HasNormals)
            //    m.Material.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", ShaderMode.Geometry));
        }
        private void _component_WorldTransformChanged()
        {
            _cullingVolume.SetRenderTransform(_component.WorldMatrix * _initialCullingVolumeMatrix);
            OctreeNode?.ItemMoved(this);
        }
        public override string ToString() => Mesh.Name;
    }
    public class SkeletalRenderableMesh : BaseRenderableMesh
    {
        public SkeletalRenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
            : base(mesh.LODs, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            Skeleton = skeleton;
        }
        
        //private Bone _singleBind;
        private Skeleton _skeleton;

        //public Bone SingleBind => _singleBind;

        public ISkeletalSubMesh Mesh { get; set; }

        [Browsable(false)]
        public Skeleton Skeleton
        {
            get => _skeleton;
            set
            {
                _skeleton = value;
                foreach (RenderableLOD m in LODs)
                    m.Manager?.SkeletonChanged(_skeleton);
            }
        }
        public override string ToString() => Mesh.Name;
    }
}
