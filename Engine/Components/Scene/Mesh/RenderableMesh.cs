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
        [Browsable(false)]
        public PrimitiveManager Manager { get; set; }
        public float VisibleDistance { get; set; }
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

            Visible = false;
        }

        private bool _visible = false;
        protected SceneComponent _component;
        protected LinkedListNode<RenderableLOD> _currentLOD;
        public RenderableLOD CurrentLOD => _currentLOD.Value;

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
                //if (_visible == value)
                //    return;
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

        //public delegate void DelPostRender(BaseRenderableMesh mesh, Matrix4 matrix, Matrix3 normalMatrix);
        //public delegate void DelPreRender(BaseRenderableMesh mesh, Matrix4 matrix, Matrix3 normalMatrix, TMaterial material, PreRenderCallback callback);
        //public class PreRenderCallback
        //{
        //    public bool ShouldRender { get; set; } = true;
        //}
        //public event DelPreRender PreRendered;
        //public event DelPostRender PostRendered;

        //private float GetRenderDistance(bool shadowPass)
        //{
        //    float dist = GetDistance(AbstractRenderer.CurrentCamera);

        //    if (!shadowPass)
        //        UpdateLOD(dist);

        //    return dist;
        //}

        public float GetDistance(Camera camera)
        {
            Vec3 camPoint = camera == null ? Vec3.Zero : camera.WorldPoint;
            Vec3 meshPoint = (_component != null ? _component.WorldMatrix.Translation : Vec3.Zero);
            return meshPoint.DistanceToFast(camPoint);
        }

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
            {
                lod.Manager.Destroy();
            }
        }

        //public void Render() => Render(null, true, true);
        //public void Render(bool allowPreRenderEvent = true, bool allowPostRenderEvent = true) => Render(null, allowPreRenderEvent, allowPostRenderEvent);
        //public void Render(TMaterial material = null, bool allowPreRenderEvent = true, bool allowPostRenderEvent = true)
        //{
        //    if (_currentLOD.Value.Manager == null)
        //        return;
        //    Matrix4 mtx = _component.WorldMatrix;
        //    Matrix3 nrm = _component.InverseWorldMatrix.Transposed().GetRotationMatrix3();
        //    PreRenderCallback callback = new PreRenderCallback();
        //    if (allowPreRenderEvent)
        //        PreRendered?.Invoke(this, mtx, nrm, material, callback);
        //    if (callback.ShouldRender)
        //    {
        //        _currentLOD.Value.Manager?.Render(mtx, nrm, material);
        //        if (allowPostRenderEvent)
        //            PostRendered?.Invoke(this, mtx, nrm);
        //    }
        //}

        private RenderCommandMesh3D _renderCommand = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            float distance = GetDistance(camera);
            if (!passes.ShadowPass)
                UpdateLOD(distance);
            _renderCommand.Primitives = _currentLOD.Value.Manager;
            _renderCommand.WorldMatrix = _component.WorldMatrix;
            _renderCommand.NormalMatrix = _component.InverseWorldMatrix.Transposed().GetRotationMatrix3();
            _renderCommand.RenderDistance = distance;
            passes.Add(_renderCommand, RenderInfo.RenderPass);
        }
    }
    public class StaticRenderableMesh : BaseRenderableMesh
    {
        [Browsable(false)]
        public IStaticSubMesh Mesh { get; set; }

        public override Shape CullingVolume => _cullingVolume;

        public void SetCullingVolume(Shape shape)
        {
            if (_cullingVolume != null)
                _component.WorldTransformChanged -= _component_WorldTransformChanged;
            _cullingVolume = shape?.HardCopy();
            if (_cullingVolume != null)
            {
                _initialCullingVolumeMatrix = _cullingVolume.GetTransformMatrix();
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

        private Shape _cullingVolume;
        private Matrix4 _initialCullingVolumeMatrix;
        
        public StaticRenderableMesh(IStaticSubMesh mesh, SceneComponent component)
            : base(mesh.LODs, mesh.RenderInfo, component)
        {
            Mesh = mesh;
            SetCullingVolume(mesh.CullingVolume);
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
