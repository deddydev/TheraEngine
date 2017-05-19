﻿using BulletSharp;
using CustomEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace System
{
    public class ComplexCapsule : Shape
    {
        protected Vec3 _upAxis, _center;
        protected float _topRadius, _bottomRadius, _topHeight, _bottomHeight;

        public Vec3 Center
        {
            get => _center;
            set => _center = value;
        }
        public Vec3 UpAxis
        {
            get => _upAxis;
            set => _upAxis = value;
        }
        public float TopRadius
        {
            get => _topRadius;
            set => _topRadius = value;
        }
        public float BottomRadius
        {
            get => _bottomRadius;
            set => _bottomRadius = value;
        }
        public float TopHeight
        {
            get => _topHeight;
            set => _topHeight = value;
        }
        public float BottomHeight
        {
            get => _bottomHeight;
            set => _bottomHeight = value;
        }

        public ComplexCapsule(
            Vec3 center,
            Vec3 upAxis,
            float topRadius,
            float bottomRadius,
            float topHeight,
            float bottomHeight)
        {
            _topRadius = Math.Abs(topRadius);
            _bottomRadius = Math.Abs(bottomRadius);
            _upAxis = upAxis;
            _topHeight = topHeight;
            _bottomHeight = bottomHeight;
            _center = center;
        }
        public float GetTotalTopHeight() => _topHeight + _topRadius;
        public float GetTotalBottomHeight() => _bottomHeight + _bottomRadius;
        public float GetTotalHeight() => GetTotalTopHeight() + GetTotalBottomHeight();
        public override void Render()
        {
            //Engine.Renderer.RenderCapsule(_center, _upAxis, _topHeight, _topRadius, _bottomHeight, _bottomRadius, Matrix4.Identity, _renderSolid);
        }
        public Sphere GetTopSphere()
            => new Sphere(_topRadius, _upAxis * _topHeight);
        public Sphere GetBottomSphere()
            => new Sphere(_bottomRadius, _upAxis * -_bottomHeight);
        
        public override CollisionShape GetCollisionShape()
        {
            throw new InvalidOperationException("Complex capsule cannot be used for physics.");
        }
        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override void SetTransform(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }

        public override Shape HardCopy()
            => new ComplexCapsule(Center, UpAxis, TopRadius, BottomRadius, TopHeight, BottomHeight);

        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
