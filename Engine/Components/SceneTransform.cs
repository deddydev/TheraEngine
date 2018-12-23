using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components
{
    public delegate void DelRecalcWorldMatrixOverride(out Matrix4 worldMatrix, out Matrix4 inverseWorldMatrix);
    public class SceneTransform : TObject
    {
        public DelRecalcWorldMatrixOverride RecalcWorldMatrixOverride;

        public SceneTransform()
        {
            Children = new EventList<SceneTransform>(ChildAdded, ChildRemoved);
        }

        public bool IsRootTransform => _rootTransform == null;
        public SceneTransform RootTransform => _rootTransform ?? this;
        public ISocket Socket { get; internal set; }
        public EventList<SceneTransform> Children { get; private set; }
        public SceneTransform Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;

                if (_parent != null)
                    _parent.Children.Remove(this);

                _parent = value;

                if (_parent != null)
                {
                    _rootTransform = _parent.RootTransform;
                    _parent.Children.Add(this);
                }
                else
                {
                    _rootTransform = null;
                }
            }
        }

        private SceneTransform _rootTransform;
        private SceneTransform _parent;

        protected SceneTransform _ancestorSimulatingPhysics;
        protected bool _simulatingPhysics = false;

        protected Matrix4 _prevWorld = Matrix4.Identity;
        protected Matrix4 _prevInvWorld = Matrix4.Identity;

        protected Matrix4 _world = Matrix4.Identity;
        protected Matrix4 _invWorld = Matrix4.Identity;

        internal protected BasicTransform _localTransform;

        private void ChildAdded(SceneTransform item)
        {
            item.Parent = this;
        }
        private void ChildRemoved(SceneTransform item)
        {
            if (item.Parent == this)
                item.Parent = null;
        }

        public event DelSocketTransformChange SocketTransformChanged;

        public void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }

        #region Main Matrices
        public virtual Matrix4 WorldMatrix
        {
            get => _world;
            set => SetWorldMatrices(value, value.Inverted());
        }
        /// <summary>
        /// Retrieving the inverse world matrix on a component that is simulating physics,
        /// or especially whose ancestor is simulating physics,
        /// is expensive because it must invert the world matrix at this given moment
        /// and also has to follow the parent heirarchy to create the inverse transform tree.
        /// Avoid calling if possible when simulating physics.
        /// </summary>
        public virtual Matrix4 InverseWorldMatrix
        {
            get => _invWorld;
            set => SetWorldMatrices(value.Inverted(), value);
        }

        public BasicTransform Local
        {
            get => _localTransform;
            set => _localTransform = value ?? BasicTransform.GetIdentity();
        }

        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you are able to compute both with the same initial parameters.
        /// </summary>
        public void SetWorldMatrices(Matrix4 matrix, Matrix4 inverse)
        {
            _prevWorld = _world;
            _prevInvWorld = _invWorld;

            _invWorld = inverse;
            _world = matrix;

            Matrix4 mtx = ParentInverseWorldMatrix * WorldMatrix;
            Matrix4 inv = InverseWorldMatrix * ParentWorldMatrix;
            _localTransform.SetMatrices(mtx, inv);

            Socket?.OnWorldTransformChanged();
        }
        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you are able to compute both with the same initial parameters.
        /// </summary>
        public void SetLocalMatrices(Matrix4 matrix, Matrix4 inverse)
        {
            _localTransform.SetMatrices(matrix, inverse);

            RecalcWorldMatrix();
        }
        public void RecalcWorldMatrix()
        {
            _prevWorld = _world;
            _prevInvWorld = _invWorld;

            if (RecalcWorldMatrixOverride != null)
                RecalcWorldMatrixOverride(out _world, out _invWorld);
            else
            {
                _world = ParentWorldMatrix * Local.Matrix;
                _invWorld = Local.InverseMatrix * ParentInverseWorldMatrix;
            }

            Socket?.OnWorldTransformChanged();
        }

        #endregion

        #region Helper Matrices

        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix4 GetWorldAnisotropicRotation4() => _world.GetRotationMatrix4();
        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix3 GetWorldAnisotropicRotation3() => _world.GetRotationMatrix3();

        /// <summary>
        /// Returns the world transform of the parent scene component.
        /// </summary>
        public Matrix4 ParentWorldMatrix => Parent?.WorldMatrix ?? Matrix4.Identity;
        /// <summary>
        /// Returns the inverse of the world transform of the parent scene component.
        /// </summary>
        public Matrix4 ParentInverseWorldMatrix => Parent?.InverseWorldMatrix ?? Matrix4.Identity;

        /// <summary>
        /// Gets the transformation of this component in relation to the root transform.
        /// </summary>
        public Matrix4 GetRootRelativeMatrix() => WorldMatrix * RootInverseWorldMatrix;
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the root transform.
        /// </summary>
        public Matrix4 GetInverseRootRelativeMatrix() => InverseWorldMatrix * RootWorldMatrix;

        /// <summary>
        /// Gets the first transform in the heirarchy which is relative to the world's origin (has no parent).
        /// </summary>
        public Matrix4 RootWorldMatrix => RootTransform.WorldMatrix;
        /// <summary>
        /// Gets the first inverse transform in the heirarchy which is relative to the world's origin (has no parent).
        /// </summary>
        public Matrix4 RootInverseWorldMatrix => RootTransform.InverseWorldMatrix;
        
        /// <summary>
        /// The world matrix of this transform on the last frame.
        /// </summary>
        public Matrix4 PreviousWorldMatrix
        {
            get => _prevWorld;
            private set => _prevWorld = value;
        }
        /// <summary>
        /// The inverse world matrix of this transform on the last frame.
        /// </summary>
        public Matrix4 PreviousInverseWorldMatrix
        {
            get => _prevInvWorld;
            private set => _prevInvWorld = value;
        }

        #endregion

        #region Physics

        protected bool SimulatingPhysics => _simulatingPhysics;
        protected void PhysicsSimulationStarted()
        {
            _simulatingPhysics = true;
            foreach (SceneTransform transform in Children)
                transform.PhysicsSimulationStarted(this);
        }
        protected void PhysicsSimulationStarted(SceneTransform simulatingAncestor)
        {
            _ancestorSimulatingPhysics = simulatingAncestor;
            foreach (SceneTransform transform in Children)
                transform.PhysicsSimulationStarted(simulatingAncestor);
        }
        protected void StopSimulatingPhysics(bool retainCurrentPosition)
        {
            _simulatingPhysics = false;
            if (retainCurrentPosition)
            {
                _invWorld = _world.Inverted();

                Matrix4 mtx = ParentInverseWorldMatrix * WorldMatrix;
                Matrix4 inv = InverseWorldMatrix * ParentWorldMatrix;
                _localTransform.SetMatrices(mtx, inv);
                
                RecalcWorldMatrix();
            }
            foreach (SceneTransform transform in Children)
                transform.PhysicsSimulationEnded();
        }
        protected void PhysicsSimulationEnded()
        {
            RecalcWorldMatrix();

            _ancestorSimulatingPhysics = null;
            foreach (SceneTransform transform in Children)
                transform.PhysicsSimulationEnded();
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Right direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalRightVec => _local.RightVec;
        /// <summary>
        /// Up direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalUpVec => _local.UpVec;
        /// <summary>
        /// Forward direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalForwardVec => _local.ForwardVec;
        /// <summary>
        /// The position of this transform relative to the parent transform (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalPoint => _local.Translation;
        /// <summary>
        /// The scale of this transform relative to the parent transform (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalScale => _local.Scale;

        /// <summary>
        /// Right direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldRightVec => _world.RightVec;
        /// <summary>
        /// Up direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldUpVec => _world.UpVec;
        /// <summary>
        /// Forward direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldForwardVec => _world.ForwardVec;
        /// <summary>
        /// The position of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldPoint => _world.Translation;
        /// <summary>
        /// The scale of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldScale => _world.Scale;

        #endregion
    }
}
