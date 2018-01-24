using TheraEngine.Rendering.Models;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene.Mesh
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        /// <summary>
        /// Mesh generated at runtime for internal use.
        /// </summary>
        public class RenderableMesh : BaseRenderableMesh
        {
            public RenderableMesh(ISkeletalSubMesh mesh, Skeleton skeleton, SceneComponent component)
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
}
