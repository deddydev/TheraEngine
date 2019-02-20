using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Mesh
{
    public class RenderableLOD : TObject
    {
        public PrimitiveManager Manager { get; set; }
        public float VisibleDistance { get; set; }
        public ShaderVar[] Parameters => Manager.Material.Parameters;
    }
    /// <summary>
    /// Mesh generated at runtime for internal use.
    /// </summary>
    public abstract class BaseRenderableMesh3D : I3DRenderable
    {
        public BaseRenderableMesh3D(List<LOD> lods, ERenderPass renderPass, RenderInfo3D renderInfo, SceneComponent component)
        {
            _component = component;

            LODs = new LinkedList<RenderableLOD>(lods.Select(x =>
            {
                RenderableLOD lod = new RenderableLOD()
                {
                    VisibleDistance = x.VisibleDistance,
                    Manager = x.CreatePrimitiveManager(),
                };
                //if (lod.Manager.Material.GeometryShaders.Count == 0)
                //{
                //    lod.Manager.Material.AddShader(Engine.LoadEngineShader("VisualizeNormal.gs", EShaderMode.Geometry));
                //    lod.Manager.Material.Requirements = TMaterial.UniformRequirements.NeedsCamera;
                //}
                return lod;
            }));

            _currentLOD = LODs.Last;

            RenderInfo = renderInfo;
            RenderCommand.RenderPass = renderPass;
        }

        public BaseRenderableMesh3D() { }
        
        protected SceneComponent _component;
        protected LinkedListNode<RenderableLOD> _currentLOD;

        [Browsable(false)]
        public RenderableLOD CurrentLOD => _currentLOD.Value;
        
        [Browsable(false)]
        [DisplayName("Levels Of Detail")]
        public LinkedList<RenderableLOD> LODs { get; private set; }
        public RenderInfo3D RenderInfo { get; protected set; }
        
        [Browsable(false)]
        public Scene3D OwningScene3D => _component?.OwningScene3D;
        
        private void UpdateLOD(float viewDist)
        {
            while (true)
            {
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
        }

        public void Destroy()
        {
            foreach (var lod in LODs)
                lod.Manager.Destroy();
        }
        
        public RenderCommandMesh3D RenderCommand { get; } = new RenderCommandMesh3D(ERenderPass.OpaqueDeferredLit);
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            float distance = camera?.DistanceFromScreenPlane(_component?.WorldPoint ?? Vec3.Zero) ?? 0.0f;
            if (!passes.ShadowPass)
                UpdateLOD(distance);
            RenderCommand.Mesh = _currentLOD.Value.Manager;
            RenderCommand.WorldMatrix = _component.WorldMatrix;
            RenderCommand.NormalMatrix = _component.InverseWorldMatrix.Transposed().GetRotationMatrix3();
            RenderCommand.RenderDistance = distance;
            passes.Add(RenderCommand);
        }
    }
    public class StaticRenderableMesh : BaseRenderableMesh3D
    {
        [Browsable(false)]
        public IStaticSubMesh Mesh { get; set; }
        
        public void SetCullingVolume(TShape shape)
        {
            if (RenderInfo.CullingVolume != null)
                _component.WorldTransformChanged -= _component_WorldTransformChanged;
            RenderInfo.CullingVolume = shape?.HardCopy();
            if (RenderInfo.CullingVolume != null)
            {
                _initialCullingVolumeMatrix = RenderInfo.CullingVolume.GetTransformMatrix();
                _component.WorldTransformChanged += _component_WorldTransformChanged;
                //_cullingVolume.VisibilityChanged += () => 
                //{
                //    if (_cullingVolume.Visible)
                //    {
                //        _component?.OwningScene?.Add(_cullingVolume);
                //    }
                //    else
                //    {
                //        _component?.OwningScene?.Remove(_cullingVolume);
                //    }
                //};  
            }
            else
                _initialCullingVolumeMatrix = Matrix4.Identity;
        }
        
        private Matrix4 _initialCullingVolumeMatrix;
        
        public StaticRenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            : base(mesh.LODs, mesh.RenderPass, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            SetCullingVolume(mesh.RenderInfo.CullingVolume);
        }
        private void _component_WorldTransformChanged(SceneComponent comp)
        {
            RenderInfo.CullingVolume.SetTransformMatrix(_component.WorldMatrix * _initialCullingVolumeMatrix);
            RenderInfo.OctreeNode?.ItemMoved(this);
        }
        public override string ToString() => Mesh.Name;
    }
    public class SkeletalRenderableMesh : BaseRenderableMesh3D
    {
        public SkeletalRenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
            : base(mesh.LODs, mesh.RenderPass, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            Skeleton = skeleton;
        }
        
        //private Bone _singleBind;
        private Skeleton _skeleton;

        //public Bone SingleBind => _singleBind;

        [Browsable(false)]
        public ISkeletalSubMesh Mesh { get; set; }

        [Browsable(false)]
        public Skeleton Skeleton
        {
            get => _skeleton;
            set
            {
                if (_skeleton == value)
                    return;
                _skeleton = value;
                foreach (RenderableLOD m in LODs)
                    m.Manager?.SkeletonChanged(_skeleton);
            }
        }
        public override string ToString() => Mesh.Name;
    }
}
