using Extensions;
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
        public MeshRenderer Manager { get; set; }
        public float VisibleDistance { get; set; }
        public ShaderVar[] Parameters => Manager.Material.Parameters;
    }
    /// <summary>
    /// Mesh generated at runtime for internal use.
    /// </summary>
    public abstract class BaseRenderableMesh3D : TObject, I3DRenderable
    {
        public BaseRenderableMesh3D() { }
        public BaseRenderableMesh3D(IList<ILOD> lods, ERenderPass renderPass, IRenderInfo3D renderInfo, ISceneComponent component)
        {
            _component = component;

            LODs = lods.Select(x =>
            {
                RenderableLOD lod = new RenderableLOD()
                {
                    VisibleDistance = x.VisibleDistance,
                    Manager = x.CreatePrimitiveManager(),
                };
                //if (lod.Manager.Material.GeometryShaders.Count == 0)
                //{
                //    lod.Manager.Material.Shaders.Add(Engine.Files.LoadEngineShader("VisualizeNormal.gs", EGLSLType.Geometry));
                //    lod.Manager.Material.Requirements = TMaterial.UniformRequirements.NeedsCamera;
                //}
                return lod;
            }).ToList();

            CurrentLODIndex = LODs.Count - 1;

            RenderInfo = renderInfo;
            RenderCommand.RenderPass = renderPass;
        }

        protected ISceneComponent _component;
        private Matrix4 _initialCullingVolumeMatrix;
        private IRenderInfo3D _renderInfo = new RenderInfo3D();

        [Browsable(false)]
        public RenderableLOD CurrentLOD => LODs.IndexInRange(CurrentLODIndex) ? LODs[CurrentLODIndex] : (LODs.Count > 0 ? LODs[LODs.Count - 1] : null);
        [Browsable(false)]
        public int CurrentLODIndex { get; private set; }
        [Browsable(false)]
        public IScene3D OwningScene3D => _component?.OwningScene3D;
        [Browsable(false)]
        [DisplayName("Levels Of Detail")]
        public List<RenderableLOD> LODs { get; private set; }

        public IRenderInfo3D RenderInfo
        {
            get => _renderInfo;
            protected set
            {
                var old = _renderInfo?.CullingVolume;

                if (_renderInfo != null)
                    _renderInfo.CullingVolumeChanged -= CullingVolumeChanged;

                _renderInfo = value ?? new RenderInfo3D();

                if (_renderInfo != null)
                    _renderInfo.CullingVolumeChanged += CullingVolumeChanged;

                CullingVolumeChanged(old, _renderInfo?.CullingVolume);
            }
        }
        private void CullingVolumeChanged(TShape oldVolume, TShape newVolume)
        {
            if (oldVolume != null)
                _component.SocketTransformChanged -= UpdateCullingVolumeTransform;
            
            if (newVolume != null)
            {
                _initialCullingVolumeMatrix = newVolume.GetTransformMatrix() * _component.InverseWorldMatrix;
                _component.SocketTransformChanged += UpdateCullingVolumeTransform;
            }
            else
                _initialCullingVolumeMatrix = Matrix4.Identity;
        }
        private void UpdateCullingVolumeTransform(ISocket comp)
        {
            RenderInfo.CullingVolume.SetTransformMatrix(comp.WorldMatrix * _initialCullingVolumeMatrix);
            RenderInfo.OctreeNode?.ItemMoved(this);
        }

        private void UpdateLOD(float viewDist)
        {
            while (true)
            {
                var currentLOD = CurrentLOD;
                if (currentLOD is null)
                    break;

                if (viewDist < currentLOD.VisibleDistance)
                {
                    if (CurrentLODIndex - 1 >= 0)
                        --CurrentLODIndex;
                    else
                        break;
                }
                else
                {
                    if (CurrentLODIndex + 1 < LODs.Count && viewDist >= LODs[CurrentLODIndex + 1].VisibleDistance)
                        ++CurrentLODIndex;
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
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            float distance = camera?.DistanceFromScreenPlane(_component?.WorldPoint ?? Vec3.Zero) ?? 0.0f;

            if (!passes.IsShadowPass)
                UpdateLOD(distance);

            RenderCommand.Mesh = CurrentLOD?.Manager;
            RenderCommand.WorldMatrix = _component.WorldMatrix;
            RenderCommand.NormalMatrix = _component.InverseWorldMatrix.Value.Transposed().GetRotationMatrix3();
            RenderCommand.RenderDistance = distance;

            passes.Add(RenderCommand);
        }

        protected void LODs_PostAnythingRemoved(ILOD item)
        {
            LODs.RemoveAt(LODs.Count - 1);
        }
        protected void LODs_PostAnythingAdded(ILOD item)
        {
            LODs.Add(new RenderableLOD()
            {
                VisibleDistance = item.VisibleDistance,
                Manager = item.CreatePrimitiveManager(),
            });
        }
    }
    public class StaticRenderableMesh : BaseRenderableMesh3D
    {
        [Browsable(false)]
        public IStaticSubMesh Mesh { get; set; }

        public StaticRenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            : base(mesh.LODs, mesh.RenderPass, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            Mesh.LODs.PostAnythingAdded += LODs_PostAnythingAdded;
            Mesh.LODs.PostAnythingRemoved += LODs_PostAnythingRemoved;
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
            Mesh.LODs.PostAnythingAdded += LODs_PostAnythingAdded;
            Mesh.LODs.PostAnythingRemoved += LODs_PostAnythingRemoved;
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
