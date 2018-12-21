using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components
{
    public delegate void DelRecalcWorldMatrixOverride(out Matrix4 worldMatrix, out Matrix4 inverseWorldMatrix);
    public class SocketTransform : TObject
    {
        public DelRecalcWorldMatrixOverride RecalcWorldMatrixOverride;

        public SocketTransform()
        {
            //Children = new EventList<SceneTransform>(ChildAdded, ChildRemoved);
        }

        public bool IsRootTransform => _rootTransform == null;
        public SocketTransform RootTransform => _rootTransform ?? this;
        public ISocket Socket { get; internal set; }
        public SocketTransform ParentTransform => Socket?.Parent.Transform;
        //{
        //    get => _parent;
        //    set
        //    {
        //        if (_parent == value)
        //            return;

        //        if (_parent != null)
        //            _parent.Children.Remove(this);

        //        _parent = value;

        //        if (_parent != null)
        //        {
        //            _rootTransform = _parent.RootTransform;
        //            _parent.Children.Add(this);
        //        }
        //        else
        //        {
        //            _rootTransform = null;
        //        }
        //    }
        //}

        private SocketTransform _rootTransform;
        private SocketTransform _parent;

        protected SocketTransform _ancestorSimulatingPhysics;
        protected bool _simulatingPhysics = false;
        
        internal protected BasicTransform _local, _world;

        public event DelSocketTransformChange SocketTransformChanged;

        public void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }

        #region Main Matrices
        //public virtual Matrix4 WorldMatrix
        //{
        //    get => _world;
        //    set => SetWorldMatrices(value, value.Inverted());
        //}
        /// <summary>
        /// Retrieving the inverse world matrix on a component that is simulating physics,
        /// or especially whose ancestor is simulating physics,
        /// is expensive because it must invert the world matrix at this given moment
        /// and also has to follow the parent heirarchy to create the inverse transform tree.
        /// Avoid calling if possible when simulating physics.
        /// </summary>
        //public virtual Matrix4 InverseWorldMatrix
        //{
        //    get => _invWorld;
        //    set => SetWorldMatrices(value.Inverted(), value);
        //}

        public BasicTransform World
        {
            get => _world;
            set
            {
                if (_world != null)
                    _world.MatrixChanged -= WorldMatrixChanged;

                _world = value ?? BasicTransform.GetIdentity();
                _world.MatrixChanged += WorldMatrixChanged;
                WorldMatrixChanged();
            }
        }
        public BasicTransform Local
        {
            get => _local;
            set
            {
                if (_local != null)
                    _local.MatrixChanged -= LocalMatrixChanged;

                _local = value ?? BasicTransform.GetIdentity();
                _local.MatrixChanged += LocalMatrixChanged;
                LocalMatrixChanged();
            }
        }

        private void LocalMatrixChanged()
        {
            RecalcWorldMatrix();
        }
        private void WorldMatrixChanged()
        {
            Matrix4 mtx = ParentInverseWorldMatrix * World.Matrix;
            Matrix4 inv = World.InverseMatrix * ParentWorldMatrix;
            _local.SetMatrices(mtx, inv);

            Socket?.OnWorldTransformChanged();
        }

        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you are able to compute both with the same initial parameters.
        /// </summary>
        public void SetWorldMatrices(Matrix4 matrix, Matrix4 inverse)
        {
            World.SetMatrices(matrix, inverse);
        }

        public void DeriveLocalFromWorld()
        {

        }
        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you are able to compute both with the same initial parameters.
        /// </summary>
        public void SetLocalMatrices(Matrix4 matrix, Matrix4 inverse)
        {
            _local.SetMatrices(matrix, inverse);

            RecalcWorldMatrix();
        }
        public void RecalcWorldMatrix()
        {
            Matrix4 mtx, inv;

            if (RecalcWorldMatrixOverride != null)
                RecalcWorldMatrixOverride(out mtx, out inv);
            else
            {
                mtx = ParentWorldMatrix * Local.Matrix;
                inv = Local.InverseMatrix * ParentInverseWorldMatrix;
            }

            SetWorldMatrices(mtx, inv);
        }

        #endregion

        #region Helper Matrices

        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix4 GetWorldAnisotropicRotation4() => World.Matrix.GetRotationMatrix4();
        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix3 GetWorldAnisotropicRotation3() => World.Matrix.GetRotationMatrix3();

        /// <summary>
        /// Returns the world transform of the parent scene component.
        /// </summary>
        public Matrix4 ParentWorldMatrix => ParentTransform?.World?.Matrix ?? Matrix4.Identity;
        /// <summary>
        /// Returns the inverse of the world transform of the parent scene component.
        /// </summary>
        public Matrix4 ParentInverseWorldMatrix => ParentTransform?.World?.InverseMatrix ?? Matrix4.Identity;

        /// <summary>
        /// Gets the transformation of this component in relation to the root transform.
        /// </summary>
        public Matrix4 GetRootRelativeMatrix() => World.Matrix * RootInverseWorldMatrix;
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the root transform.
        /// </summary>
        public Matrix4 GetInverseRootRelativeMatrix() => World.InverseMatrix * RootWorldMatrix;

        /// <summary>
        /// Gets the first transform in the heirarchy which is relative to the world's origin (has no parent).
        /// </summary>
        public Matrix4 RootWorldMatrix => RootTransform.World.Matrix;
        /// <summary>
        /// Gets the first inverse transform in the heirarchy which is relative to the world's origin (has no parent).
        /// </summary>
        public Matrix4 RootInverseWorldMatrix => RootTransform.World.InverseMatrix;
        
        ///// <summary>
        ///// The world matrix of this transform on the last frame.
        ///// </summary>
        //public Matrix4 PreviousWorldMatrix
        //{
        //    get => _prevWorld;
        //    private set => _prevWorld = value;
        //}
        ///// <summary>
        ///// The inverse world matrix of this transform on the last frame.
        ///// </summary>
        //public Matrix4 PreviousInverseWorldMatrix
        //{
        //    get => _prevInvWorld;
        //    private set => _prevInvWorld = value;
        //}

        #endregion

        #region Physics

        protected bool SimulatingPhysics => _simulatingPhysics;
        protected void PhysicsSimulationStarted()
        {
            _simulatingPhysics = true;
            foreach (ISocket socket in Socket.Children)
                socket.Transform.PhysicsSimulationStarted(this);
        }
        protected void PhysicsSimulationStarted(SocketTransform simulatingAncestor)
        {
            _ancestorSimulatingPhysics = simulatingAncestor;
            foreach (ISocket socket in Socket.Children)
                socket.Transform.PhysicsSimulationStarted(simulatingAncestor);
        }
        protected void StopSimulatingPhysics(bool retainCurrentPosition)
        {
            _simulatingPhysics = false;
            if (retainCurrentPosition)
            {
                World.InverseMatrix = World.Matrix.Inverted();

                Matrix4 mtx = ParentInverseWorldMatrix * World.Matrix;
                Matrix4 inv = World.InverseMatrix * ParentWorldMatrix;
                _local.SetMatrices(mtx, inv);
                
                RecalcWorldMatrix();
            }
            foreach (ISocket socket in Socket.Children)
                socket.Transform.PhysicsSimulationEnded();
        }
        protected void PhysicsSimulationEnded()
        {
            RecalcWorldMatrix();

            _ancestorSimulatingPhysics = null;
            foreach (ISocket socket in Socket.Children)
                socket.Transform.PhysicsSimulationEnded();
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Right direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalRightVec => _local.Matrix.RightVec;
        /// <summary>
        /// Up direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalUpVec => _local.Matrix.UpVec;
        /// <summary>
        /// Forward direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalForwardVec => _local.Matrix.ForwardVec;
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
        public Vec3 WorldRightVec => _world.Matrix.RightVec;
        /// <summary>
        /// Up direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldUpVec => _world.Matrix.UpVec;
        /// <summary>
        /// Forward direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldForwardVec => _world.Matrix.ForwardVec;
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
