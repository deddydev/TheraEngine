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
        //private SocketTransform _parent;

        protected SocketTransform _ancestorSimulatingPhysics;
        protected bool _simulatingPhysics = false;
        
        internal protected MatrixTransform _local, _world;

        public event DelSocketTransformChange SocketTransformChanged;

        public void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }

        #region Main Matrices
        public MatrixTransform World
        {
            get => _world;
            set
            {
                if (_world != null)
                    _world.MatrixChanged -= WorldMatrixChanged;

                _world = value ?? new MatrixTransform();
                _world.MatrixChanged += WorldMatrixChanged;
                WorldMatrixChanged();
            }
        }
        public MatrixTransform Local
        {
            get => _local;
            set
            {
                if (_local != null)
                    _local.MatrixChanged -= LocalMatrixChanged;

                _local = value ?? new MatrixTransform();
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
            DeriveLocalFromWorld();
            Socket?.OnWorldTransformChanged();
        }

        public void DeriveLocalFromWorld()
        {
            //Derive local matrices using difference between this and parent world matrix
            Matrix4 mtx = ParentInverseWorldMatrix * World.Matrix;
            Matrix4 inv = World.InverseMatrix * ParentWorldMatrix;
            _local.Set(mtx, inv, true);
        }
        public void RecalcWorldMatrix()
        {
            Matrix4 mtx, inv;

            if (RecalcWorldMatrixOverride != null)
                RecalcWorldMatrixOverride(out mtx, out inv);
            else
            {
                //Default: apply local transform after parent world transform to get world transform
                mtx = ParentWorldMatrix * Local.Matrix;
                inv = Local.InverseMatrix * ParentInverseWorldMatrix;
            }

            World.Set(mtx, inv, true);
            Socket?.OnWorldTransformChanged();
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
            foreach (ISocket socket in Socket.ChildComponents)
                socket.Transform.PhysicsSimulationStarted(this);
        }
        protected void PhysicsSimulationStarted(SocketTransform simulatingAncestor)
        {
            _ancestorSimulatingPhysics = simulatingAncestor;
            foreach (ISocket socket in Socket.ChildComponents)
                socket.Transform.PhysicsSimulationStarted(simulatingAncestor);
        }
        protected void StopSimulatingPhysics(bool retainCurrentPosition)
        {
            _simulatingPhysics = false;
            if (retainCurrentPosition)
            {
                //World matrix has been updated by physics.
                //Calculate inverse and derive local transforms
                World.InverseMatrix = World.Matrix.Inverted();
                DeriveLocalFromWorld();
            }
            foreach (ISocket socket in Socket.ChildComponents)
                socket.Transform.PhysicsSimulationEnded();
        }
        protected void PhysicsSimulationEnded()
        {
            //RecalcWorldMatrix();

            _ancestorSimulatingPhysics = null;
            foreach (ISocket socket in Socket.ChildComponents)
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
        public Vec3 LocalPoint => _local.Matrix.Translation;
        /// <summary>
        /// The scale of this transform relative to the parent transform (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalScale => _local.Matrix.Scale;

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
        public Vec3 WorldPoint => _world.Matrix.Translation;
        /// <summary>
        /// The scale of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldScale => _world.Matrix.Scale;

        #endregion
    }
}
