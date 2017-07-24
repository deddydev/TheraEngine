using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheraEngine.Worlds.Actors.Types
{
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate
    }
    public class EditorTransformTool3D : Actor<SkeletalMeshComponent>
    {
        public EditorTransformTool3D(ISocket modified)
        {
            TargetSocket = modified;
        }
        private Material _xAxisMat, _yAxisMat, _zAxisMat, _screenRotMat;
        private Bone _rootBone;
        protected override SkeletalMeshComponent OnConstruct()
        {
            SkeletalMesh mesh = new SkeletalMesh("TransformTool");
            TransformMode = TransformType.Translate;

            //Skeleton
            string rootBoneName = "Root"; //Screen-scaled bone
            string screenBoneName = "Screen"; //Screen-aligned child bone
            _rootBone = new Bone(rootBoneName)
            {
                ScaleByDistance = true,
                DistanceScaleScreenSize = 4.0f,
            };
            Bone screen = new Bone(screenBoneName)
            {
                BillboardType = BillboardType.PerspectiveXY
            };
            _rootBone.ChildBones.Add(screen);
            Skeleton skel = new Skeleton(_rootBone);

            const int circlePrecision = 20;

            _xAxisMat = Material.GetUnlitColorMaterialForward(Color.Red);
            _yAxisMat = Material.GetUnlitColorMaterialForward(Color.Green);
            _zAxisMat = Material.GetUnlitColorMaterialForward(Color.Blue);
            _screenRotMat = Material.GetUnlitColorMaterialForward(Color.LightGray);
            _xAxisMat.RenderParams.DepthTest.Enabled = false;
            _yAxisMat.RenderParams.DepthTest.Enabled = false;
            _zAxisMat.RenderParams.DepthTest.Enabled = false;
            _screenRotMat.RenderParams.DepthTest.Enabled = false;

            //X axis
            PrimitiveData xAxisPrim = Segment.Mesh(Vec3.Zero, Vec3.UnitX);
            xAxisPrim.SingleBindBone = rootBoneName;

            //Y axis
            PrimitiveData yAxisPrim = Segment.Mesh(Vec3.Zero, Vec3.UnitY);
            yAxisPrim.SingleBindBone = rootBoneName;

            //Z axis
            PrimitiveData zAxisPrim = Segment.Mesh(Vec3.Zero, Vec3.UnitZ);
            zAxisPrim.SingleBindBone = rootBoneName;

            //Screen-aligned rotation
            PrimitiveData screenRotPrim = Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, circlePrecision);
            screenRotPrim.SingleBindBone = screenBoneName;

            //X rotation
            PrimitiveData xRotPrim = Circle3D.WireframeMesh(1.0f, Vec3.UnitX, Vec3.Zero, circlePrecision);
            xRotPrim.SingleBindBone = rootBoneName;

            //Y rotation
            PrimitiveData yRotPrim = Circle3D.WireframeMesh(1.0f, Vec3.UnitY, Vec3.Zero, circlePrecision);
            yRotPrim.SingleBindBone = rootBoneName;
            
            //Z rotation
            PrimitiveData zRotPrim = Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, circlePrecision);
            zRotPrim.SingleBindBone = rootBoneName;

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("XAxis", xAxisPrim, _xAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("YAxis", yAxisPrim, _yAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ZAxis", zAxisPrim, _zAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("XRotation", xRotPrim, _xAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("YRotation", yRotPrim, _yAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });
            
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ZRotation", zRotPrim, _zAxisMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ScreenRotation", screenRotPrim, _screenRotMat, true)
            { RenderInfo = new RenderInfo3D(Rendering.RenderPassType3D.OnTopForward, null, false, false) });

            //mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Gray), "ScreenTranslation"));
            SkeletalMeshComponent meshComp = new SkeletalMeshComponent(mesh, skel);
            meshComp.WorldTransformChanged += _transform_WorldTransformChanged;
            return meshComp;
        }
        
        private void _transform_WorldTransformChanged()
        {
            _transformChanged = true;
        }

        private bool _transformChanged = false;
        private TransformType _mode;
        private ISocket _targetSocket = null;

        public TransformType TransformMode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case TransformType.Rotate:
                        _highlightMode = HighlightRotation;
                        break;
                    case TransformType.Translate:
                        _highlightMode = HighlightTranslationScale;
                        break;
                    case TransformType.Scale:
                        _highlightMode = HighlightTranslationScale;
                        break;
                }
            }
        }
        /// <summary>
        /// The socket transform that is being manipulated by this transform tool.
        /// </summary>
        public ISocket TargetSocket
        {
            get => _targetSocket;
            set
            {
                _targetSocket = value;
                if (_targetSocket != null)
                    RootComponent.WorldMatrix = _targetSocket.WorldMatrix;
                else
                    RootComponent.WorldMatrix = Matrix4.Identity;
            }
        }
        
        public static EditorTransformTool3D Instance => _currentInstance;

        public static void DestroyInstance()
        {
            _currentInstance?.Despawn();
            _currentInstance = null;
        }
        public static EditorTransformTool3D GetCurrentInstance(ISocket comp)
        {
            if (_currentInstance == null)
                Engine.World.SpawnActor(new EditorTransformTool3D(comp));
            else
                _currentInstance.TargetSocket = comp;
            return _currentInstance;
        }
        public override void OnSpawnedPreComponentSetup(World world)
        {
            _currentInstance = this;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            _currentInstance = null;
        }

        private static EditorTransformTool3D _currentInstance;
        private bool _hiX, _hiY, _hiZ, _hiCirc, _hiSphere;
        private const float _orbRadius = 1.0f;
        private const float _circRadius = 1.2f;
        private const float _axisSnapRange = 7.0f;
        private const float _selectRange = 0.03f; //Selection error range for orb and circ
        private const float _axisSelectRange = 0.15f; //Selection error range for axes
        private const float _selectOrbScale = _selectRange / _orbRadius;
        private const float _circOrbScale = _circRadius / _orbRadius;
        private const float _axisLDist = 2.0f;
        private const float _axisHalfLDist = 0.75f;
        private const float _apthm = 0.075f;
        private const float _dst = 1.5f;
        private const float _scaleHalf1LDist = 0.8f;
        private const float _scaleHalf2LDist = 1.2f;

        private DelHighlight _highlightMode;
        private delegate void DelHighlight(Camera camera, Vec3 worldPoint, Ray localRay, float radius, bool clamp, ref bool snapFound);
        private void HighlightRotation(Camera camera, Vec3 worldPoint, Ray localRay, float radius, bool clamp, ref bool snapFound)
        {
            if (!localRay.LineSphereIntersect(Vec3.Zero, radius, out Vec3 point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                localRay.LinePlaneIntersect(Vec3.Zero, (camera.WorldPoint - worldPoint).Normalized(), out point);

                //Clamp the point to edge of the sphere
                if (clamp)
                    point = Ray.PointAtLineDistance(worldPoint, point, radius);
            }

            float distance = point.LengthFast;

            //Point lies within orb radius?
            if (Math.Abs(distance - radius) < (radius * _selectOrbScale))
            {
                _hiSphere = true;

                //Determine axis snapping
                Vec3 angles = CustomMath.RadToDeg(point.GetAngles());
                angles.X = Math.Abs(angles.X);
                angles.Y = Math.Abs(angles.Y);
                angles.Z = Math.Abs(angles.Z);

                if (Math.Abs(angles.Y - 90.0f) <= _axisSnapRange)
                    _hiX = true;
                else if (angles.X >= (180.0f - _axisSnapRange) || angles.X <= _axisSnapRange)
                    _hiY = true;
                else if (angles.Y >= (180.0f - _axisSnapRange) || angles.Y <= _axisSnapRange)
                    _hiZ = true;
            }
            //Point lies on circ line?
            else if (Math.Abs(distance - (radius * _circOrbScale)) < (radius * _selectOrbScale))
                _hiCirc = true;

            if (_hiX || _hiY || _hiZ || _hiCirc)
                snapFound = true;
        }
        private void HighlightTranslationScale(Camera camera, Vec3 worldPoint, Ray localRay, float radius, bool clamp, ref bool snapFound)
        {
            Plane yz = new Plane(Vec3.Zero, localRay.StartPoint.X < 0.0f ? -Vec3.UnitX : Vec3.UnitX);
            Plane xz = new Plane(Vec3.Zero, localRay.StartPoint.Y < 0.0f ? -Vec3.UnitY : Vec3.UnitY);
            Plane xy = new Plane(Vec3.Zero, localRay.StartPoint.Z < 0.0f ? -Vec3.UnitZ : Vec3.UnitZ);

            //if (Collision.PlaneIntersectsPoint(_xz, cursor.StartPoint) == EPlaneIntersection.Back)

            Vec3[] intersectionPoints = new Vec3[3];
            bool[] intersects = new bool[3]
            {
                localRay.LinePlaneIntersect(yz, out intersectionPoints[0]),
                localRay.LinePlaneIntersect(xz, out intersectionPoints[1]),
                localRay.LinePlaneIntersect(xy, out intersectionPoints[2]),
            };
            Vec3?[] testDiffs = new Vec3?[3];
            for (int i = 0; i < 3; ++i)
                if (intersects[i])
                {
                    Vec3 diff = intersectionPoints[i] / radius;
                    if (diff.X > -_axisSelectRange && diff.X < (_axisLDist + 0.01f) &&
                        diff.Y > -_axisSelectRange && diff.Y < (_axisLDist + 0.01f) &&
                        diff.Z > -_axisSelectRange && diff.Z < (_axisLDist + 0.01f))
                        testDiffs[i] = diff;
                }

            //Check if point lies on a specific axis
            foreach (Vec3? v in testDiffs)
            {
                if (!v.HasValue)
                    continue;
                Vec3 diff = v.Value;

                float errorRange = _axisSelectRange;

                if (diff.X > _axisHalfLDist &&
                    Math.Abs(diff.Y) < errorRange &&
                    Math.Abs(diff.Z) < errorRange)
                    _hiX = true;
                if (diff.Y > _axisHalfLDist &&
                    Math.Abs(diff.X) < errorRange &&
                    Math.Abs(diff.Z) < errorRange)
                    _hiY = true;
                if (diff.Z > _axisHalfLDist &&
                    Math.Abs(diff.X) < errorRange &&
                    Math.Abs(diff.Y) < errorRange)
                    _hiZ = true;

                if (snapFound = _hiX || _hiY || _hiZ)
                    break;
            }

            if (!snapFound)
            {
                foreach (Vec3? v in testDiffs)
                {
                    if (!v.HasValue)
                        continue;
                    Vec3 diff = v.Value;

                    if (_mode == TransformType.Translate)
                    {
                        if (diff.X < _axisHalfLDist &&
                            diff.Y < _axisHalfLDist &&
                            diff.Z < _axisHalfLDist)
                        {
                            //Point lies inside the double drag areas
                            if (diff.X > _axisSelectRange)
                                _hiX = true;
                            if (diff.Y > _axisSelectRange)
                                _hiY = true;
                            if (diff.Z > _axisSelectRange)
                                _hiZ = true;

                            _hiCirc = !_hiX && !_hiY && !_hiZ;

                            snapFound = true;
                        }
                    }
                    else if (_mode == TransformType.Scale)
                    {
                        //Determine if the point is in the double or triple drag triangles
                        float halfDist = _scaleHalf2LDist;
                        float centerDist = _scaleHalf1LDist;
                        if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, halfDist, 0)))
                            if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, centerDist, 0)))
                                _hiX = _hiY = _hiZ = true;
                            else
                                _hiX = _hiY = true;
                        else if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, 0, halfDist)))
                            if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, 0, centerDist)))
                                _hiX = _hiY = _hiZ = true;
                            else
                                _hiX = _hiZ = true;
                        else if (diff.IsInTriangle(new Vec3(), new Vec3(0, halfDist, 0), new Vec3(0, 0, halfDist)))
                            if (diff.IsInTriangle(new Vec3(), new Vec3(0, centerDist, 0), new Vec3(0, 0, centerDist)))
                                _hiX = _hiY = _hiZ = true;
                            else
                                _hiY = _hiZ = true;

                        snapFound = _hiX || _hiY || _hiZ;
                    }

                    if (snapFound)
                        break;
                }
            }
        }
        /// <summary>
        /// Returns true if intersecting one of the transform tool's various parts.
        /// </summary>
        public bool Highlight(Ray cursor, Camera camera, bool pressed)
        {
            bool clamp = true, snapFound = false;

            Vec3 worldPoint = RootComponent.WorldMatrix.GetPoint();
            Ray localRay = cursor.TransformedBy(_rootBone.InverseWorldMatrix);
            float radius = camera.DistanceScale(worldPoint, 1.0f);

            _highlightMode(camera, worldPoint, localRay, radius, clamp, ref snapFound);

            _xAxisMat.Parameter<ShaderVec4>(0).Value = _hiX ? (ColorF4)Color.Yellow : Color.Red;
            _yAxisMat.Parameter<ShaderVec4>(0).Value = _hiY ? (ColorF4)Color.Yellow : Color.Green;
            _zAxisMat.Parameter<ShaderVec4>(0).Value = _hiZ ? (ColorF4)Color.Yellow : Color.Blue;
            _screenRotMat.Parameter<ShaderVec4>(0).Value = _hiCirc ? (ColorF4)Color.Yellow : Color.LightGray;

            return snapFound;
        }
    }
}
