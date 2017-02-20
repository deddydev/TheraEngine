using CustomEngine.Rendering.Models;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public partial class SkeletalMeshComponent : TRSComponent
    {
        internal class RenderableMesh : IRenderable
        {
            public RenderableMesh(ISkeletalMesh mesh, Skeleton skeleton, SceneComponent component)
            {
                _mesh = mesh;
                _manager = new PrimitiveManager(mesh.Data, mesh.Material);
                _component = component;
                Skeleton = skeleton;
                Visible = false;
                IsRendering = true;
            }

            private PrimitiveManager _manager;
            private bool _isVisible, _isRendering;
            private SceneComponent _component;
            private ISkeletalMesh _mesh;
            private RenderOctree.Node _renderNode;
            private Bone _singleBind;
            private Skeleton _skeleton;
            private Shape _cullingVolume;
            
            public bool Visible
            {
                get { return _isVisible; }
                set
                {
                    _isVisible = value;
                    if (_isVisible)
                        Engine.Renderer.Scene.AddRenderable(this);
                    else
                        Engine.Renderer.Scene.RemoveRenderable(this);
                }
            }
            public Bone SingleBind
            {
                get { return _singleBind; }
            }
            public bool IsRendering
            {
                get { return _isRendering; }
                set { _isRendering = value; }
            }
            public ISkeletalMesh Mesh
            {
                get { return _mesh; }
                set { _mesh = value; }
            }
            public RenderOctree.Node RenderNode
            {
                get { return _renderNode; }
                set { _renderNode = value; }
            }
            public Skeleton Skeleton
            {
                get { return _skeleton; }
                set
                {
                    _skeleton?.ClearInfluences(_manager);
                    _skeleton = value;

                    //TODO: support multi-bone influence as single bind as well
                    _singleBind = _mesh.SingleBindName != null && _skeleton != null ? _skeleton.GetBone(_mesh.SingleBindName) : null;
                    
                    _skeleton?.GenerateInfluences(_manager);
                    _manager.SkeletonChanged(_skeleton);
                }
            }
            public Shape CullingVolume { get { return _cullingVolume; } }

            //IsRendering is checked by the octree before calling
            public void Render()
            {
                if (Visible)
                {
                    Matrix4 world, invWorld;
                    //if (_singleBind != null)
                    //{
                    //    world = _singleBind.WorldMatrix;
                    //    invWorld = _singleBind.InverseWorldMatrix;
                    //}
                    //else
                    //{
                        world = _component.WorldMatrix;
                        //invWorld = _component.InverseWorldMatrix;
                    //}

                    //_manager.Render(world, invWorld.Transposed().GetRotationMatrix3());
                    _manager.Render(world, world.GetRotationMatrix3());
                }
            }
            public override string ToString()
            {
                return ((ObjectBase)_mesh).Name;
            }
        }
    }
}
